namespace SnowWarden.Backend.Core.Abstractions;

public abstract class EntityBase : IDbEntity
{
	public int Id { get; private set; }
	public void SetExistingId(int id)
	{
		Id = id;
	}
}