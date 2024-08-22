
using SnowWarden.Backend.Core.Utils.Results;

namespace SnowWarden.Backend.Core.Features.Members.Services;

public interface IInstructorService
{
	Task<ApplicationIdentityResult<Instructor>> CreateInstructorAccount(Instructor instructor);
	Task<ApplicationIdentityResult<Instructor>> DisableAsync(Instructor instructor, Admin admin);
	Task<ApplicationIdentityResult<Instructor>> GetByIdAsync(int id);

	Task<IReadOnlyCollection<Instructor>> GetAsync();
}