using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;

using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Services;

namespace ProjectNaam.WebApi.Repositories;


public class Environment2DRepository : SqlService, IEnvironment2DRepository
{
    public Environment2DRepository(string connectionString) : base(connectionString) { }

    public async Task<IEnumerable<Environment2D>> GetAllAsync(string userId)
    {
        var query = "SELECT * FROM Environment2D WHERE UserId = @UserId";
        using var db = CreateConnection();
        return await db.QueryAsync<Environment2D>(query, new { UserId = userId });
    }

    public async Task<Environment2D> GetByIdAsync(string id)
    {
        var query = "SELECT * FROM Environment2D WHERE Id = @Id";
        using var db = CreateConnection();
        return await db.QueryFirstOrDefaultAsync<Environment2D>(query, new { Id = id });
    }

    public async Task CreateAsync(Environment2D env)
    {
        await ValidateEnvironment(env, true);
        var query = "INSERT INTO Environment2D (Id, Name, MaxHeight, MaxWidth, UserId) VALUES (@Id, @Name, @MaxHeight, @MaxWidth, @UserId)";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, env);
    }

    public async Task UpdateAsync(Environment2D env)
    {
        await ValidateEnvironment(env, false);
        var query = "UPDATE Environment2D SET Name = @Name, MaxHeight = @MaxHeight, MaxWidth = @MaxWidth WHERE Id = @Id";
        using var db = CreateConnection();
        await db.ExecuteAsync(query, env);
    }

    public async Task DeleteAsync(string id)
    {
        using var db = CreateConnection();
        await db.ExecuteAsync("DELETE FROM Object2D WHERE EnvironmentId = @Id", new { Id = id });
        await db.ExecuteAsync("DELETE FROM Environment2D WHERE Id = @Id", new { Id = id });
    }

    public async Task ValidateEnvironment(Environment2D env, bool isCreation)
    {
        if (string.IsNullOrWhiteSpace(env.Name) || env.Name.Length > 25)
            throw new ArgumentException("Name must be between 1 and 25 characters.");

        if (env.MaxWidth < 20 || env.MaxWidth > 200 || env.MaxHeight < 10 || env.MaxHeight > 100)
            throw new ArgumentException("Width must be 20-200, Height 10-100.");

        using var db = CreateConnection();

        if (isCreation)
        {
            var exists = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Environment2D WHERE Name = @Name AND UserId = @UserId", new { env.Name, env.UserId });
            if (exists > 0) throw new ArgumentException("Environment with same name exists.");

            var count = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Environment2D WHERE UserId = @UserId", new { env.UserId });
            if (count >= 5) throw new InvalidOperationException("Max 5 environments allowed.");
        }
        else
        {
            var exists = await db.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Environment2D WHERE Name = @Name AND UserId = @UserId AND Id != @Id", new { env.Name, env.UserId, env.Id });
            if (exists > 0) throw new ArgumentException("Environment with same name exists.");
        }
    }
}