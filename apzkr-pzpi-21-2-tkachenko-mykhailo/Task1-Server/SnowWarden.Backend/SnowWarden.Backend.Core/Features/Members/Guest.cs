using SnowWarden.Backend.Core.Features.Identity;

namespace SnowWarden.Backend.Core.Features.Members;

public class Guest : ApplicationUser
{
	public const string ROLE_NAME = "Guest";

	// public ICollection<Review>? Reviews { get; set; }
	public ICollection<Booking.Booking> Bookings { get; set; }

	public Membership Membership { get; private set; } = Membership.Basic;

	public void UpgradeToAdvanced() => Membership = Membership >= Membership.Advanced
		? Membership
		: Membership.Advanced;
	public void UpgradeToProfessional() => Membership = Membership >= Membership.Professional
		? Membership
		: Membership.Professional;
}