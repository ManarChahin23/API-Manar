using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using ProjectNaam.WebApi.Models;
using ProjectNaam.WebApi.Repositories;

namespace ProjectNaam.WebApi.Controllers;

    [Route("api/environment2d/{environmentId}/object2d")]
    [ApiController]
    public class Object2DController : ControllerBase
    {
        private readonly IObject2DRepository _objectRepository;

        public Object2DController(IObject2DRepository objectRepository)
        {
            _objectRepository = objectRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string environmentId)
        {
            var objs = await _objectRepository.GetObjectsByEnvironmentIdAsync(environmentId);
            return Ok(objs);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var obj = await _objectRepository.GetObjectByIdAsync(id);
            if (obj == null) return NotFound();
            return Ok(obj);
        }

        [HttpPost]
        public async Task<IActionResult> Create(string environmentId, [FromBody] Object2D obj)
        {
            obj.EnvironmentId = environmentId;
            await _objectRepository.CreateObjectAsync(obj);
            return CreatedAtAction(nameof(GetById), new { environmentId, id = obj.Id }, obj);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] Object2D obj)
        {
            obj.Id = id;
            await _objectRepository.UpdateObjectAsync(obj);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _objectRepository.DeleteObjectAsync(id);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll(string environmentId)
        {
            await _objectRepository.DeleteObjectsByEnvironmentIdAsync(environmentId);
            return NoContent();
        }
    }
