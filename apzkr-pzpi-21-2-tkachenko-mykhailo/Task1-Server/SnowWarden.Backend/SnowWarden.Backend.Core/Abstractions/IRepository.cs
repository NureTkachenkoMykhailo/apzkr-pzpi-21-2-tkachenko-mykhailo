using System.Linq.Expressions;

namespace SnowWarden.Backend.Core.Abstractions;

public interface IRepository<TEntity> where TEntity : class, IDbEntity
{
	Task<ICollection<TEntity>> CreateRange(params TEntity[] entities);
	Task<TEntity> CreateAsync(TEntity entity);
	Task<TEntity> UpdateAsync(TEntity entity);
	Task<TEntity> DeleteAsync(TEntity entity);
	Task<TEntity> DeleteByIdAsync(int id);

	Task<TEntity?> GetByIdAsync(int id);
	Task<TEntity?> GetByIdCompleteAsync(int id);
	Task<TEntity?> GetReadonlyByIdCompleteAsync(int id);
	Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync(Expression<Func<TEntity, bool>> predicate);
	Task<IReadOnlyCollection<TEntity>> GetReadonlyCompleteAsync();
	Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync();
	Task<IReadOnlyCollection<TEntity>> GetReadonlyLightweightAsync(Expression<Func<TEntity, bool>> predicate);
	Task<List<TEntity>> GetCompleteAsync(Expression<Func<TEntity, bool>> predicate);
	Task<List<TEntity>> GetCompleteAsync();
	Task<List<TEntity>> GetLightweightAsync(Expression<Func<TEntity, bool>> predicate);
	Task<List<TEntity>> GetLightweightAsync();
}