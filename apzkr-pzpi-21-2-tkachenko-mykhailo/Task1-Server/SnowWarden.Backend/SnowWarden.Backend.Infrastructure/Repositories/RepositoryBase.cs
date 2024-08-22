using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Services;

using SnowWarden.Backend.Infrastructure.Data;
using SnowWarden.Backend.Infrastructure.Exceptions;
using SnowWarden.Backend.Infrastructure.Services;

namespace SnowWarden.Backend.Infrastructure.Repositories;

public abstract class RepositoryBase<TEntity> : IRepository<TEntity> where TEntity : class, IDbEntity
{
	protected readonly ApplicationDbContext Context;
	protected readonly AttachMaster AttachMaster;

	protected RepositoryBase(ApplicationDbContext context, AttachMaster attachMaster)
	{
		Context = context;
		AttachMaster = attachMaster;
	}

	public async Task<ICollection<TEntity>> CreateRange(params TEntity[] entities)
	{
		foreach (TEntity entity in entities)
		{
			await CreateInternalAsync(entity);
			Context.Set<TEntity>().Add(entity);
		}

		await Context.SaveChangesAsync();

		return entities;
	}
	public async Task<TEntity> CreateAsync(TEntity entity)
	{
		await CreateInternalAsync(entity);
		Context.Set<TEntity>().Add(entity);
		await Context.SaveChangesAsync();

		return entity;
	}
	public async Task<TEntity> UpdateAsync(TEntity entity)
	{
		TEntity source = await IncludeComplete(Context.Set<TEntity>()).FirstOrDefaultAsync(e => e.Id == entity.Id)
			?? throw new InfrastructureException(new LocalizedContent
			{
				Translations = new LocalizationDictionary
				{
					{
						Localizator.SupportedLanguages.AmericanEnglish,
						$"Could not find entity {typeof(TEntity).Name} to update with id {entity.Id}"
					},
					{
						Localizator.SupportedLanguages.Ukrainian,
						$"Не вийшло знайти сутність {typeof(TEntity).Name} з ідентифікатором {entity.Id}"
					}
				}
			});

		Context.Entry(source).CurrentValues.SetValues(entity);
		await UpdateInternalAsync(source, entity);
		Context.Update(source);
		await Context.SaveChangesAsync();

		return source;
	}
	public async Task<TEntity> DeleteAsync(TEntity entity)
	{
		EntityEntry<TEntity> removed = Context.Set<TEntity>().Remove(entity);
		await Context.SaveChangesAsync();

		return removed.Entity;
	}
	public async Task<TEntity> DeleteByIdAsync(int id)
	{
		TEntity? entity = await Context.Set<TEntity>().FindAsync(id);
		Context.Set<TEntity>().Remove(entity);

		await Context.SaveChangesAsync();

		return entity;
	}

	public async Task<TEntity?> GetReadonlyByIdCompleteAsync(int id) =>
		await IncludeComplete(Context.Set<TEntity>()).AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
	public async Task<TEntity?> GetByIdAsync(int id) =>
		await IncludeLightweight(Context.Set<TEntity>()).FirstOrDefaultAsync(e => e.Id == id);
	public async Task<TEntity?> GetByIdCompleteAsync(int id) =>
		await IncludeComplete(Context.Set<TEntity>()).FirstOrDefaultAsync(e => e.Id == id);

	public async Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync(Expression<Func<TEntity, bool>> predicate) =>
		await IncludeComplete(Context.Set<TEntity>()).AsNoTracking().Where(predicate).ToListAsync();
	public async Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync(Expression<Func<TEntity, bool>> predicate) =>
		await IncludeLightweight(Context.Set<TEntity>()).AsNoTracking().Where(predicate).ToListAsync();
	public async Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync() =>
		await IncludeComplete(Context.Set<TEntity>()).AsNoTracking().ToListAsync();
	public async Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync() =>
		await IncludeLightweight(Context.Set<TEntity>()).AsNoTracking().ToListAsync();
	public Task<List<TEntity>> GetCompleteAsync(Expression<Func<TEntity, bool>> predicate) =>
		IncludeComplete(Context.Set<TEntity>()).Where(predicate).ToListAsync();
	public Task<List<TEntity>> GetCompleteAsync() => IncludeComplete(Context.Set<TEntity>()).ToListAsync();
	public Task<List<TEntity>> GetLightweightAsync() =>
		IncludeLightweight(Context.Set<TEntity>()).ToListAsync();
	public Task<List<TEntity>> GetLightweightAsync(Expression<Func<TEntity, bool>> predicate) =>
		IncludeLightweight(Context.Set<TEntity>()).Where(predicate).ToListAsync();

	protected abstract Task UpdateInternalAsync(TEntity source, TEntity compare);
	protected abstract Task CreateInternalAsync(TEntity addedEntity);

	protected abstract IQueryable<TEntity> IncludeComplete(DbSet<TEntity> set);
	protected abstract IQueryable<TEntity> IncludeLightweight(DbSet<TEntity> set);

	protected virtual async Task ValidateSingleAttachment<TAttachment>(
        TEntity source, Func<TEntity, TAttachment?> validationDelegate,
        Func<TEntity, int> foreignKeySelector)
        where TAttachment : class, IDbEntity, new()
	{
		TAttachment? attachment = validationDelegate(source);
		if (attachment is null)
		{
			attachment = new TAttachment();
			attachment.SetExistingId(foreignKeySelector(source));
		}
		_ = await Context.Set<TAttachment>().FindAsync(attachment.Id)
			?? throw new InvalidAttachmentEntityException(attachment);
	}

	protected virtual async Task ValidateManyAttachments<TAttachment>(TEntity source, Func<TEntity, ICollection<TAttachment>> validationDelegate) where TAttachment : class, IDbEntity
	{
		ICollection<TAttachment> attachments = validationDelegate(source);
		List<TAttachment> dbAttachments = await Context.Set<TAttachment>().ToListAsync();
		foreach (TAttachment attachment in attachments)
		{
			if (dbAttachments.Select(da => da.Id).Contains(attachment.Id) is false)
			{
				throw new InvalidAttachmentEntityException(attachment);
			}
		}
	}
}