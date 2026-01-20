using Microsoft.AspNetCore.Mvc;
using Apiapio.Models;
using Apiapio.Services;
using Microsoft.AspNetCore.Authorization;

namespace Apiapio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] 
    public class AlbumController : ControllerBase
    {
        private readonly IAlbumService _albumService;
        private readonly ILogger<AlbumController> _logger;

        public AlbumController(
            IAlbumService albumService,
            ILogger<AlbumController> logger)
        {
            _albumService = albumService;
            _logger = logger;
        }

        // Obtiene todos los álbumes
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AlbumDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAllAlbums()
        {
            var albums = await _albumService.GetAllAlbumsAsync();
            return Ok(albums);
        }

        //Obtiene un álbum por ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlbumDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlbumDto>> GetAlbumById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid album ID" });
            }

            var album = await _albumService.GetAlbumByIdAsync(id);

            if (album == null)
            {
                return NotFound(new { message = $"Album with ID {id} not found" });
            }

            return Ok(album);
        }

        // Obtiene álbumes de un usuario específico
        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<AlbumDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AlbumDto>>> GetAlbumsByUserId(int userId)
        {
            var albums = await _albumService.GetAlbumsByUserIdAsync(userId);
            return Ok(albums);
        }

        //Crea un nuevo álbum
        [HttpPost]
        [ProducesResponseType(typeof(AlbumDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AlbumDto>> CreateAlbum([FromBody] AlbumDto album)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdAlbum = await _albumService.CreateAlbumAsync(album);

                return CreatedAtAction(
                    nameof(GetAlbumById),
                    new { id = createdAlbum.Id },
                    createdAlbum
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Actualiza un álbum existente
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AlbumDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlbumDto>> UpdateAlbum(int id, [FromBody] AlbumDto album)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid album ID" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedAlbum = await _albumService.UpdateAlbumAsync(id, album);

                if (updatedAlbum == null)
                {
                    return NotFound(new { message = $"Album with ID {id} not found" });
                }

                return Ok(updatedAlbum);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Elimina un álbum
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid album ID" });
            }

            var result = await _albumService.DeleteAlbumAsync(id);

            if (!result)
            {
                return NotFound(new { message = $"Album with ID {id} not found or could not be deleted" });
            }

            return NoContent();
        }
    }
}
