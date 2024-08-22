using System.Text.Json.Serialization;
using SnowWarden.Backend.Core.Utils.Localization;
using SnowWarden.Backend.Core.Utils.Localization.Extensions;

namespace SnowWarden.Backend.Core.Utils.Results;

public class ApplicationResult : IApplicationResult
{
	public bool Succeeded { get; protected init; }
	[JsonIgnore] public IEnumerable<LocalizedContent> ErrorsWithLocalization { get; protected init; } = [];
	[JsonIgnore] public IEnumerable<LocalizedContent> WarningsWithLocalization { get; protected init; } = [];

	public IEnumerable<string> Errors { get; private set; } = [];
	public IEnumerable<string> Warnings { get; private set; } = [];
	[JsonIgnore] public Language Language { get; private set; }

	public IApplicationResult GetLocalizedResult()
	{
		Errors = ErrorsWithLocalization.Select(localizedErr => localizedErr.ToLocalizedString(Language));
		Warnings = WarningsWithLocalization.Select(localizedWarning => localizedWarning.ToLocalizedString(Language));

		return this;
	}

	protected ApplicationResult() { }

	public IApplicationResult WithLanguage(Language language)
	{
		Language = language;

		return this;
	}
}