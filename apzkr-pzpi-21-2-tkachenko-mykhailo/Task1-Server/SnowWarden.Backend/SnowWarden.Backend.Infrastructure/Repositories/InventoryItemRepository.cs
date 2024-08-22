using Microsoft.EntityFrameworkCore;

using SnowWarden.Backend.Core.Features.Inventory;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class InventoryItemRepository : RepositoryBase<InventoryItem>
{
	public InventoryItemRepository(ApplicationDbContext context, AttachMaster attachMaster)
		: base(context, attachMaster) { }

	private class AttributeComparer : IEqualityComparer<InventoryAttribute>
	{
		public bool Equals(InventoryAttribute? x, InventoryAttribute? y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;
			return x.Title == y.Title && x.Value == y.Value;
		}

		public int GetHashCode(InventoryAttribute obj)
		{
			return HashCode.Combine(obj.Title, obj.Value);
		}
	}

	protected override async Task UpdateInternalAsync(InventoryItem source, InventoryItem compare)
	{
		await ValidateSingleAttachment(compare, s => s.Inventory, s => s.InventoryId);

		source.Attributes = compare.Attributes?.ToList() ?? source.Attributes;

		// IEnumerable<InventoryAttribute> toDetach = source.Attributes?
		// 	.Except(compare.Attributes ?? [], new AttributeComparer())
		// 	.ToList() ?? [];
		// IEnumerable<InventoryAttribute> toAttach = compare.Attributes?
		// 	.Except(source.Attributes ?? [], new AttributeComparer())
		// 	.ToList() ?? [];
		//
		// foreach (InventoryAttribute attributeToRemove in toDetach)
		// {
		// 	source.Attributes?.Remove(attributeToRemove);
		// }
		// foreach (InventoryAttribute attributeToAdd in toAttach)
		// {
		// 	source.Attributes?.Add(attributeToAdd);
		// }
	}

	protected override async Task CreateInternalAsync(InventoryItem source)
	{
		await ValidateSingleAttachment(source, s => s.Inventory, s => s.InventoryId);
	}

	protected override IQueryable<InventoryItem> IncludeComplete(DbSet<InventoryItem> set) =>
		set
			.Include(ii => ii.Inventory)
			.Include(ii => ii.TrainingSessions);
	protected override IQueryable<InventoryItem> IncludeLightweight(DbSet<InventoryItem> set) => set;
}