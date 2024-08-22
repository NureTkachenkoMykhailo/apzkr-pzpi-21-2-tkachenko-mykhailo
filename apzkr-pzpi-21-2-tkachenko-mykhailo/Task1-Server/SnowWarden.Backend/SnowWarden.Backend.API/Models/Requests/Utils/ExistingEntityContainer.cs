using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.API.Models.Requests.Utils;

public class ExistingEntityContainer<TEntity> where TEntity : IDbEntity
{
	public int? Id { get; set; }
	public TEntity Source { get; set; }

	public static implicit operator TEntity(ExistingEntityContainer<TEntity> setter)
	{
		if (setter.Id is not null)
		{
			setter.Source.SetExistingId((int)setter.Id);
		}

		return setter.Source;
	}
}