using SnowWarden.Backend.Core.Features.Identity;
using SnowWarden.Backend.Core.Features.Members.Events;

namespace SnowWarden.Backend.Core.Features.Members;

public class Instructor : ApplicationUser
{
	public const string ROLE_NAME = "Instructor";

	public int ExperienceMonths { get; private set; }
	public int ExperienceYears => ExperienceMonths / 12;
	public ICollection<Training.TrainingSession>? Trainings { get; set; }

	public void RaiseInstructorAccountDisabledEvent(Admin disabler)
	{
		DomainEvents.Add(new InstructorAccountDisabledEvent(disabler.Email));
	}

	public void AddExperience(int months)
	{
		// Маємо додавати тільки валідні місяці до стажу
		ExperienceMonths += months > 0 ? months : 0;
	}
}