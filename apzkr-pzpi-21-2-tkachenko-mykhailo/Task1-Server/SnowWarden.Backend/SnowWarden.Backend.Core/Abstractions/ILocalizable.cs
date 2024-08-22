using SnowWarden.Backend.Core.Utils;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Core.Abstractions;

public interface ILocalizable<TEntity>
{
	public Language Language { get; }

	public TEntity GetLocalizedResult();
}