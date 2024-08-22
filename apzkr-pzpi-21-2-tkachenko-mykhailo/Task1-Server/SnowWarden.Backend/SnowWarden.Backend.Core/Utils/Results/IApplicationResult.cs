using SnowWarden.Backend.Core.Abstractions;
using SnowWarden.Backend.Core.Utils.Localization;

namespace SnowWarden.Backend.Core.Utils.Results;

public interface IApplicationResult : ILocalizable<IApplicationResult>
{
	public bool Succeeded { get;}

	public IEnumerable<LocalizedContent> ErrorsWithLocalization { get; }
	public IEnumerable<LocalizedContent> WarningsWithLocalization { get; }

	public IEnumerable<string> Errors { get; }
	public IEnumerable<string> Warnings { get; }
}