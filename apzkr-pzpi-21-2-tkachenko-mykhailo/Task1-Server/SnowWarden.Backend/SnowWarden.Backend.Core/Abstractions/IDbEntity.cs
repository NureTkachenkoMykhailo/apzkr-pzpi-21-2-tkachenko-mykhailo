namespace SnowWarden.Backend.Core.Abstractions;

public interface IDbEntity
{
	public int Id { get; }

	public void SetExistingId(int id);
}