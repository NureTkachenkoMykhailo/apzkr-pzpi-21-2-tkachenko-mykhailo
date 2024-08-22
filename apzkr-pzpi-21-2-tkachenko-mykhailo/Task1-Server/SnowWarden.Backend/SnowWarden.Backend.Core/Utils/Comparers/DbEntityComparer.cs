using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.Core.Utils.Comparers;

public class DbEntityComparer<T> : IEqualityComparer<T> where T : IDbEntity
{
	public bool Equals(T? x, T? y)
	{
		if (ReferenceEquals(x, y)) return true;
		if (ReferenceEquals(x, null)) return false;
		if (ReferenceEquals(y, null)) return false;
		if (x.GetType() != y.GetType()) return false;

		int tempXId = 0;
		int tempYId = 0;
		if (x.Id == 0) tempXId = new Random().Next(0, 10000);
		if (y.Id == 0) tempYId = new Random().Next(0, 10000);

		return tempXId == tempYId;
	}

	public int GetHashCode(T obj)
	{
		return obj.Id.GetHashCode();
	}
}