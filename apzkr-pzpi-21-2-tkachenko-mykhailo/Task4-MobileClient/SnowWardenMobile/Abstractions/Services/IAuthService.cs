using System.Threading.Tasks;

namespace SnowWardenMobile.Abstractions.Services;

public interface IAuthService : IApiCallerService
{
	public Task<bool> AuthenticateAsync(string contact, string password);
	public Task<bool> IsAuthenticatedAsync();
}