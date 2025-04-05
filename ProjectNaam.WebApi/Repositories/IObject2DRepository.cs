using System.Collections.Generic;
using System.Threading.Tasks;

using ProjectNaam.WebApi.Models;

namespace ProjectNaam.WebApi.Repositories;


public interface IObject2DRepository
{
    Task<IEnumerable<Object2D>> GetObjectsByEnvironmentIdAsync(string environmentId);
    Task<Object2D> GetObjectByIdAsync(string id);
    Task CreateObjectAsync(Object2D object2D);
    Task UpdateObjectAsync(Object2D object2D);
    Task DeleteObjectAsync(string id);
    Task DeleteObjectsByEnvironmentIdAsync(string environmentId);
}