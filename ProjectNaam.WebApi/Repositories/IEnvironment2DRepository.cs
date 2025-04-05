using System.Collections.Generic;
using System.Threading.Tasks;

using ProjectNaam.WebApi.Models;

namespace ProjectNaam.WebApi.Repositories;


public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2D>> GetAllAsync(string userId);
    Task<Environment2D> GetByIdAsync(string id);
    Task CreateAsync(Environment2D environment);
    Task UpdateAsync(Environment2D environment);
    Task DeleteAsync(string id);
    Task ValidateEnvironment(Environment2D environment, bool isCreation);
}
