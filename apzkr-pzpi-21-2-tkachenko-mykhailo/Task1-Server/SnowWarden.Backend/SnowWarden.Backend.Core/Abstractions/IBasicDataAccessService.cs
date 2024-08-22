namespace SnowWarden.Backend.Core.Abstractions;

public interface IBasicDataAccessService<TEntity> : IRepository<TEntity> where TEntity : class, IDbEntity;