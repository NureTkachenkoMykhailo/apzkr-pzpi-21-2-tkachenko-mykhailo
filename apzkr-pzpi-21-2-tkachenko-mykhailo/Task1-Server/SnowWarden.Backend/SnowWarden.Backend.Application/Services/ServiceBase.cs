using System.Linq.Expressions;
using SnowWarden.Backend.Core.Abstractions;

namespace SnowWarden.Backend.Application.Services;

public abstract class ServiceBase<TEntity>(IRepository<TEntity> repository) : IBasicDataAccessService<TEntity> where TEntity : class, IDbEntity
{
	public virtual Task<ICollection<TEntity>> CreateRange(params TEntity[] entities) => repository.CreateRange(entities);

	public virtual Task<TEntity> CreateAsync(TEntity entity) => repository.CreateAsync(entity);

	public Task<TEntity> UpdateAsync(TEntity entity) => repository.UpdateAsync(entity);

	public Task<TEntity> DeleteAsync(TEntity entity) => repository.DeleteAsync(entity);
	public Task<TEntity> DeleteByIdAsync(int id) => repository.DeleteByIdAsync(id);
	public Task<TEntity?> GetByIdAsync(int id) => repository.GetByIdAsync(id);

	public Task<TEntity?> GetByIdCompleteAsync(int id) => repository.GetByIdCompleteAsync(id);

	public Task<TEntity?> GetReadonlyByIdCompleteAsync(int id) => repository.GetReadonlyByIdCompleteAsync(id);

	public Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync(Expression<Func<TEntity, bool>> predicate) =>
		repository.GetReadonlyCompleteAsync(predicate);

	public Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync() => repository.GetReadonlyCompleteAsync();

	public Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync() => repository.GetReadonlyLightweightAsync();

	public Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync(Expression<Func<TEntity, bool>> predicate) =>
		repository.GetReadonlyLightweightAsync(predicate);

	public Task<List<TEntity>> GetCompleteAsync(Expression<Func<TEntity, bool>> predicate) =>
		repository.GetCompleteAsync(predicate);

	public Task<List<TEntity>> GetCompleteAsync() => repository.GetCompleteAsync();

	public Task<List<TEntity>> GetLightweightAsync(Expression<Func<TEntity, bool>> predicate) =>
		repository.GetLightweightAsync(predicate);

	public Task<List<TEntity>> GetLightweightAsync() => repository.GetLightweightAsync();
}