using System.Text;

using IdentityModel;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using Quartz;

using SnowWarden.Backend.API.Jobs;
using SnowWarden.Backend.API.Options.Auth;
using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Members;
using SnowWarden.Backend.Infrastructure.Data;

namespace SnowWarden.Backend.API.Extensions.DI;

public static class ActivatorExtensions
{
	public static IServiceCollection AddApplicationAuthentication(this IServiceCollection services, IConfiguration configuration)
	{
		services
			.AddAuthentication(opt =>
			{
				opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(jwtOptions =>
			{
				jwtOptions.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateIssuerSigningKey = true,
					ValidIssuers = [
						configuration.GetSection("Auth:JwtOptions:ValidIssuer").Value ?? string.Empty
					],
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("Auth:JwtOptions:SigningKey").Value ?? string.Empty)),
					ValidateLifetime = true,
					ValidateAudience = false,
					NameClaimType = JwtClaimTypes.PreferredUserName
				};
			});
		services.AddAuthorization();

		return services;
	}
	public static IServiceCollection AddApplicationIdentities(this IServiceCollection services)
	{
		services.AddIdentity<ApplicationUser, IdentityRole<int>>(opt =>
			{
				opt.Password.RequireDigit = true;
				opt.Password.RequiredLength = 8;
				opt.Password.RequireLowercase = true;
				opt.Password.RequireUppercase = true;
				opt.Password.RequireNonAlphanumeric = true;

				opt.User.RequireUniqueEmail = true;

				opt.SignIn.RequireConfirmedEmail = true;

				opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				opt.Lockout.MaxFailedAccessAttempts = 5;
			})
			.AddEntityFrameworkStores<ApplicationDbContext>()
			.AddDefaultTokenProviders();

		services.AddApplicationIdentity<Guest>();
		services.AddApplicationIdentity<Admin>();
		services.AddApplicationIdentity<Instructor>();

		return services;
	}

	public static IServiceCollection AddMapping(this IServiceCollection services)
	{
		services.AddAutoMapper(typeof(ActivatorExtensions).Assembly);

		return services;
	}

	public static IServiceCollection ConfigureApplicationOptions(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<JwtOptions>(configuration.GetSection("Auth:JwtOptions"));

		return services;
	}

	public static IServiceCollection AddEventProcessor(this IServiceCollection services)
	{
		services.AddHostedService<EventProcessorJob>();

		return services;
	}

	public static IServiceCollection AddReserveCoping(this IServiceCollection services)
	{
		services.AddQuartz(opt =>
		{
			JobKey backupJobName = JobKey.Create(nameof(ReserveCopingBackgroundJob));

			opt
				.AddJob<ReserveCopingBackgroundJob>(backupJobName)
				.AddTrigger(trigger =>
					trigger
						.ForJob(backupJobName)
						.WithSimpleSchedule(
							schedule =>
								schedule
									.WithIntervalInSeconds(15)
									.RepeatForever()));
		});
		services.AddQuartzHostedService();

		return services;
	}

	public static IServiceCollection ConfigureApplicationSwagger(this IServiceCollection services)
	{
		services.AddSwaggerGen(options =>
		{
			options.SwaggerDoc("v1", new OpenApiInfo { Title = "SnowWarden alpha", Version = "v1" });
			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In = ParameterLocation.Header,
				Description = "Please enter a valid token",
				Name = "Authorization",
				Type = SecuritySchemeType.Http,
				BearerFormat = "JWT",
				Scheme = "Bearer"
			});
			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type=ReferenceType.SecurityScheme,
							Id="Bearer"
						}
					},
					[]
                }
			});
		});

		return services;
	}

	#region private
		private static void AddApplicationIdentity<TCoreIdentity>(this IServiceCollection services) where TCoreIdentity : ApplicationUser
		{
			services.AddIdentityCore<TCoreIdentity>(opt =>
				{
					opt.Password.RequireDigit = true;
					opt.Password.RequiredLength = 8;
					opt.Password.RequireLowercase = true;
					opt.Password.RequireUppercase = true;
					opt.Password.RequireNonAlphanumeric = true;

					opt.User.RequireUniqueEmail = true;

					opt.SignIn.RequireConfirmedEmail = true;

					opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
					opt.Lockout.MaxFailedAccessAttempts = 5;
				})
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();
		}
	#endregion
}