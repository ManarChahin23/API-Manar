using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Services;

namespace ProjectNaam.WebApi.Repositories;


public class Object2DRepository : SqlService, IObject2DRepository
{
    public Object2DRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<Object2D>> GetObjectsByEnvironmentIdAsync(string environmentId)
    {
        var query = "SELECT * FROM Object2D WHERE EnvironmentId = @EnvironmentId";
        using var db = CreateConnection();
        return await db.QueryAsync<Object2D>(query, new { EnvironmentId = environmentId });
    }

    public async Task<Object2D> GetObjectByIdAsync(string id)
    {
        var query = "SELECT * FROM Object2D WHERE Id = @Id";
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Object2D>(query, new { Id = id });
    }

    public async Task CreateObjectAsync(Object2D obj)
    {
        var query = "INSERT INTO Object2D (Id, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, EnvironmentId) VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @EnvironmentId)";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, obj);
    }

    public async Task UpdateObjectAsync(Object2D obj)
    {
        var query = "UPDATE Object2D SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer WHERE Id = @Id";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, obj);
    }

    public async Task DeleteObjectAsync(string id)
    {
        var query = "DELETE FROM Object2D WHERE Id = @Id";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, new { Id = id });
    }

    public async Task DeleteObjectsByEnvironmentIdAsync(string environmentId)
    {
        var query = "DELETE FROM Object2D WHERE EnvironmentId = @EnvironmentId";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, new { EnvironmentId = environmentId });
    }
}