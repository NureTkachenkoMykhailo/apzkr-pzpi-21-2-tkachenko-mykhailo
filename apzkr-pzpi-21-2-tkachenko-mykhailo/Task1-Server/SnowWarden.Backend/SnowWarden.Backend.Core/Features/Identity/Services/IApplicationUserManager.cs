using Microsoft.AspNetCore.Identity;
using SnowWarden.Backend.Core.Utils.Security;

namespace SnowWarden.Backend.Core.Features.Identity.Services;

public interface IApplicationUserManager<TUser> where TUser : ApplicationUser
{
	Task<IdentityResult> CreateWithTemporaryPasswordAsync(
		TUser user,
		int passwordLength,
		PasswordGenerationProperties? properties = null);
	Task<IdentityResult> CreateAsync(TUser user, string password);
	Task<IdentityResult> DeleteAsync(TUser user);
	Task<TUser?> FindByEmailAsync(string email);
	Task<TUser?> FindByIdAsync(string id);
	Task<IdentityResult> ConfirmEmailAsync(TUser user, string token);
	Task<IdentityResult> UpdateAsync(TUser user);
	Task<TUser?> FindByNameAsync(string name);
	Task<bool> IsLockedOutAsync(TUser user);
	Task<bool> CheckPasswordAsync(TUser user, string password);
	Task<IReadOnlyCollection<TUser>> GetUsers();
	Task<string> GenerateEmailConfirmationTokenUrlQuery(TUser guest);
}