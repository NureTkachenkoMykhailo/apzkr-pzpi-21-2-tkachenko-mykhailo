using Microsoft.EntityFrameworkCore;
using SnowWarden.Backend.Core.Features.Inventory;
using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public class InventoryRepository : RepositoryBase<Inventory>
{
	public InventoryRepository(ApplicationDbContext context, AttachMaster attachMaster) : base(context, attachMaster)
	{
	}

	protected override Task UpdateInternalAsync(Inventory source, Inventory compare)
	{
		DetachNavigations(source);

		return Task.CompletedTask;
	}

	protected override Task CreateInternalAsync(Inventory source)
	{
		DetachNavigations(source);

		return Task.CompletedTask;
	}

	protected override IQueryable<Inventory> IncludeComplete(DbSet<Inventory> set) => set.Include(i => i.InventoryItems);
	protected override IQueryable<Inventory> IncludeLightweight(DbSet<Inventory> set) => set;

	private void DetachNavigations(Inventory inventory)
	{
		if (!(inventory.InventoryItems?.Any() ?? false)) return;
		Context.Entry(inventory.InventoryItems).State = EntityState.Detached;
		inventory.InventoryItems = null;
	}
}