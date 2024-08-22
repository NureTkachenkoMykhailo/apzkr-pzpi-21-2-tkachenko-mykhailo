using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Quartz;

namespace SnowWarden.Backend.API.Jobs;

[DisallowConcurrentExecution]
public class ReserveCopingBackgroundJob : IJob
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<ReserveCopingBackgroundJob> _logger;

	public ReserveCopingBackgroundJob(IConfiguration configuration, ILogger<ReserveCopingBackgroundJob> logger)
	{
		_configuration = configuration;
		_logger = logger;
	}
	public async Task Execute(IJobExecutionContext context)
	{
		string connectionString = _configuration.GetConnectionString("Postgres");
		(string Username, string Password, string DbName, string Host) creds = RetreiveCredentials(connectionString);
		string fileName = $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
		string backupDir = _configuration.GetSection("Backup:dir")?.Value ?? string.Empty;
		if (backupDir == string.Empty)
		{
			return;
		}
		if (!Directory.Exists(backupDir))
		{
			Directory.CreateDirectory(backupDir);
			_logger.LogInformation("Created backup directory: {BackupDir}", backupDir);
		}
		string exportVar = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "set" : "export"));
		string pgDump =
			$"export PATH=$PATH:/Library/PostgreSQL/16/bin && pg_dump --dbname={creds.Username}://postgres:{creds.Password}@{creds.Host}/{creds.DbName} > {backupDir}/{fileName}";
		await Execute(pgDump);
	}

	private (string Username, string Password, string DbName, string Host) RetreiveCredentials(string connectionString)
	{

		string[] parts = connectionString.Split(';');

		string? password = null;
		string? host = null;
		string? username = null;
		string? database = null;

		foreach (string part in parts)
		{
			string[] keyValue = part.Split('=');

			if (keyValue.Length != 2) continue;
			string key = keyValue[0].Trim();

			string value = keyValue[1].Trim();

			if (key.Equals("Password", StringComparison.OrdinalIgnoreCase))
			{
				password = value;
			}
			else if (key.Equals("Host", StringComparison.OrdinalIgnoreCase))
			{
				host = value;
			}
			else if (key.Equals("User ID", StringComparison.OrdinalIgnoreCase))
			{
				username = value;
			}
			else if (key.Equals("Database", StringComparison.OrdinalIgnoreCase))
			{
				database = value;
			}
		}

		return (Username: username, Password: password, DbName:database, Host: host);
	}

	private Task Execute(string dumpCommand)
	{
		return Task.Run(() =>
		{

			string batFilePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}." + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "bat" : "sh"));
			try
			{
				string batchContent = "";
				batchContent += $"{dumpCommand}";

				File.WriteAllText(batFilePath, batchContent, Encoding.ASCII);

				ProcessStartInfo info = ProcessInfoByOS(batFilePath);

				using Process proc = System.Diagnostics.Process.Start(info);

				proc.WaitForExit();
				var exit = proc.ExitCode;
				_logger.LogInformation("Executed backup command with exit code: {ExitCode}", exit);
				proc.Close();
			}
			catch (Exception e)
			{
				// Your exception handler here.

			}
			finally
			{
				if (File.Exists(batFilePath)) File.Delete(batFilePath);
			}
		});
	}

	private static ProcessStartInfo ProcessInfoByOS(string batFilePath)
	{
		ProcessStartInfo info;
		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
		{
			info = new ProcessStartInfo(batFilePath)
			{
			};
		}
		else
		{
			info = new ProcessStartInfo("sh")
			{
				Arguments = $"{batFilePath}"
			};
		}

		info.CreateNoWindow = true;
		info.UseShellExecute = false;
		info.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
		info.RedirectStandardError = true;

		return info;
	}
}