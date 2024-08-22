using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.API.Models.Requests.Utils;

public class IdSetter : EntityBase
{
	public new int Id
	{
		get => base.Id;
		set => base.SetExistingId(value);
	}
}