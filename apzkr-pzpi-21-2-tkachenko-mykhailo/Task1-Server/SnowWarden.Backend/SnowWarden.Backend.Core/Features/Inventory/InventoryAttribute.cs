namespace SnowWarden.Backend.Core.Features.Inventory;

public class InventoryAttribute
{
	public string Title { get; set; }
	public string Value { get; set; }

	public override bool Equals(object? obj)
	{
		if (ReferenceEquals(this, obj)) return true;
		if (ReferenceEquals(this, null)) return false;
		if (ReferenceEquals(obj, null)) return false;
		if (GetType() != obj.GetType()) return false;

		if (obj is InventoryAttribute attr)
		{
			return
				Title == attr.Title &&
				Value == attr.Title;
		}

		return false;
	}

	public override int GetHashCode()
	{
		return 0;
	}
}