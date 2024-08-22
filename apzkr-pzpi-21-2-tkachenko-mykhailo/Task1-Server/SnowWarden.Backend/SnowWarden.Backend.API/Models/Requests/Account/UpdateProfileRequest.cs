using SnowWarden.Backend.Core.Features.Identity;

namespace SnowWarden.Backend.API.Models.Requests.Account;

public class UpdateProfileRequest
{
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Phone { get; set; }
	public string Language { get; set; }

	public void CopyTo(ApplicationUser user)
	{
		user.FirstName = FirstName;
		user.LastName = LastName;
		user.PhoneNumber = Phone;
		user.SetLanguage(Language);
	}
}