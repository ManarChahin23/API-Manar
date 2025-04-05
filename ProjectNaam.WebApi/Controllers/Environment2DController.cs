using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Repositories;

namespace ProjectNaam.WebApi.Controllers
{

    [Route("api/environment2d")]
    [ApiController]
    public class Environment2DController : ControllerBase
    {
        private readonly IEnvironment2DRepository _environmentRepository;

        public Environment2DController(IEnvironment2DRepository environmentRepository)
        {
            _environmentRepository = environmentRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var envs = await _environmentRepository.GetAllAsync(userId);
            return Ok(envs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var env = await _environmentRepository.GetByIdAsync(id);
            if (env == null) return NotFound();
            return Ok(env);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Environment2D env)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            env.UserId = userId;

            await _environmentRepository.CreateAsync(env);
            return CreatedAtAction(nameof(GetById), new { id = env.Id }, env);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Environment2D env)
        {
            var existing = await _environmentRepository.GetByIdAsync(id);
            if (existing == null) return NotFound();

            env.Id = id; // Zorg dat ID consistent is
            await _environmentRepository.UpdateAsync(env);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _environmentRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}