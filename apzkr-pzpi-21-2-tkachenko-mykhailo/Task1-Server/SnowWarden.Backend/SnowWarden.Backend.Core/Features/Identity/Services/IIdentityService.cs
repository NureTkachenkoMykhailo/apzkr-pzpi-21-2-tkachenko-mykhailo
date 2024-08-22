using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.Core.Features.Identity.Services;

public interface IIdentityService<TUser> where TUser : ApplicationUser
{
	public Task<ApplicationIdentityResult<TUser>> SignIn(string username, string password);
	Task<ApplicationIdentityResult<TUser>> Register(TUser user, string password);
	Task<ApplicationIdentityResult<TUser>> RegisterWithTemporaryPassword(TUser user);
	public Task<ApplicationIdentityResult<TUser>> ConfirmEmail(int userId, string token);
	public Task<ApplicationIdentityResult<TUser>> FindAsync(string contact);
	public Task<ApplicationIdentityResult<TUser>> UpdateAsync(TUser user);
}