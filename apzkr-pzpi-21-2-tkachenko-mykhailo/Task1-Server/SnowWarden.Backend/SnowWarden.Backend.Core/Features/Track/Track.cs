using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.Core.Features.Track;

public class Track : IDbEntity
{
	public int Id { get; set; }

	public string Name { get; set; }

	public ICollection<Section> Sections { get; set; } = [];

	public void SetExistingId(int id) => Id = id;
}