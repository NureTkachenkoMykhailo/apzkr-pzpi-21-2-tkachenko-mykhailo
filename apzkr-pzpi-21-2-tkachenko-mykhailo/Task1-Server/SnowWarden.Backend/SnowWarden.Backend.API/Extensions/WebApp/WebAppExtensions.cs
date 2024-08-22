using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Infrastructure.Data;

namespace SnowWarden.Backend.API.Extensions.WebApp;

public static class WebAppExtensions
{
	public static IApplicationBuilder UseDatabaseMigrations(this WebApplication app)
	{
		using IServiceScope scope = app.Services.CreateScope();

		ApplicationDbContext dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
		List<string> pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

		if (!pendingMigrations.Any()) return app;

		foreach (string pending in pendingMigrations)
		{
			app.Logger.LogInformation("Migration to be applied: {PendingMigrations}", pending);
		}

		dbContext.Database.Migrate();

		return app;
	}

	public static async Task SeedMembers(this WebApplication app)
	{
		await using AsyncServiceScope scope = app.Services.CreateAsyncScope();

		UserManager<Guest> guestManager = scope.ServiceProvider.GetRequiredService<UserManager<Guest>>();
		UserManager<Admin> adminManager = scope.ServiceProvider.GetRequiredService<UserManager<Admin>>();
		UserManager<Instructor> instructorManager = scope.ServiceProvider.GetRequiredService<UserManager<Instructor>>();
		if (await guestManager.FindByEmailAsync("20werasdf+guest@gmail.com") is null)
		{
			Guest seedGuest = new()
			{
				Email = "20werasdf+guest@gmail.com",
				FirstName = "Mykhailo guest",
				LastName = "Tkachenko",
				EmailConfirmed = true
			};
			seedGuest.SetLanguage(Localizator.SupportedLanguages.AmericanEnglish.Code);
			await guestManager.CreateAsync(seedGuest, "ASDF1234asdf#");
		}

		if (await adminManager.FindByEmailAsync("20werasdf+admin@gmail.com") is null)
		{
			Admin seedAdmin = new()
			{
				Email = "20werasdf+admin@gmail.com",
				FirstName = "Mykhailo admin",
				LastName = "Tkachenko",
				EmailConfirmed = true
			};

			seedAdmin.SetLanguage(Localizator.SupportedLanguages.Ukrainian.Code);
			await adminManager.CreateAsync(seedAdmin, "ASDF1234asdf#");
		}

		if (await instructorManager.FindByEmailAsync("20werasdf+instructor@gmail.com") is null)
		{
			Instructor seedInstructor = new()
			{
				Email = "20werasdf+instructor@gmail.com",
				FirstName = "Mykhailo instructor",
				LastName = "Tkachenko",
				EmailConfirmed = true
			};
			seedInstructor.SetLanguage(Localizator.SupportedLanguages.AmericanEnglish.Code);
			await instructorManager.CreateAsync(seedInstructor, "ASDF1234asdf#");
		}
	}
}