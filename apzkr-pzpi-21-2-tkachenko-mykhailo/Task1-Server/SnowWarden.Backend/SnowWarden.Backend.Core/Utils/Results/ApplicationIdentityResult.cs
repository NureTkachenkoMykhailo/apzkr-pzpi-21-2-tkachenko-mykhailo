using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Core.Utils.Results;

public class ApplicationIdentityResult : ApplicationResult
{
	public ApplicationUser? User { get; protected init; }
	public IdentityResultType ResultType { get; protected init; }
}

public class ApplicationIdentityResult<TUser> : ApplicationIdentityResult where TUser : ApplicationUser
{
	public new TUser? User
	{
		get => base.User as TUser;
		private init => base.User = value;
	}

	public static ApplicationIdentityResult<TUser> Successful(
		IdentityResultType type,
		TUser? user = null,
		IEnumerable<LocalizedContent>? warnings = null)
	{
		return new ApplicationIdentityResult<TUser>
		{
			ResultType = type,
			User = user,
			Succeeded = true,
			WarningsWithLocalization = warnings ?? []
		};
	}

	public static ApplicationIdentityResult<TUser> Failure(
		IdentityResultType type,
		IEnumerable<LocalizedContent>? errors = null,
		IEnumerable<LocalizedContent>? warnings = null,
		TUser? user = null)
	{
		return new ApplicationIdentityResult<TUser>
		{
			ResultType = type,
			Succeeded = false,
			ErrorsWithLocalization = errors ?? [],
			WarningsWithLocalization = warnings ?? []
		};
	}
}