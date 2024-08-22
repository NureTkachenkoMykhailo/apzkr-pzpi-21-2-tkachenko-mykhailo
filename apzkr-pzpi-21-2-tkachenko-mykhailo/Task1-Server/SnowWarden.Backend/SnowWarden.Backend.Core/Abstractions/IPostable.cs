using SnowWarden.Backend.Core.Features.Identity;

namespace SnowWarden.Backend.Core.Abstractions;

public interface IPostable<TUser> : IDbEntity where TUser : ApplicationUser
{
	public int PosterId { get; set; }
	public string Content { get; set; }
	public TUser Poster { get; set; }
}