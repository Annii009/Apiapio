using Apiapio.Models;
using Apiapio.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PhotosGatewayAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] 
    public class PhotosController : ControllerBase
    {
        private readonly IPhotoService _photoService;
        private readonly ILogger<PhotosController> _logger;

        public PhotosController(
            IPhotoService photoService,
            ILogger<PhotosController> logger)
        {
            _photoService = photoService;
            _logger = logger;
        }

        // Obtiene todas las fotos
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PhotoDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAllPhotos()
        {
            var photos = await _photoService.GetAllPhotosAsync();
            return Ok(photos);
        }


        // Obtiene una foto específica por ID
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PhotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PhotoDto>> GetPhotoById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid photo ID" });
            }

            var photo = await _photoService.GetPhotoByIdAsync(id);
            
            if (photo == null)
            {
                return NotFound(new { message = $"Photo with ID {id} not found" });
            }

            return Ok(photo);
        }

        // Crea una nueva foto
        [HttpPost]
        [ProducesResponseType(typeof(PhotoDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PhotoDto>> CreatePhoto([FromBody] PhotoDto photo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPhoto = await _photoService.CreatePhotoAsync(photo);
                
                return CreatedAtAction(
                    nameof(GetPhotoById),
                    new { id = createdPhoto.Id },
                    createdPhoto
                );
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Actualiza una foto existente
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(PhotoDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PhotoDto>> UpdatePhoto(int id, [FromBody] PhotoDto photo)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid photo ID" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPhoto = await _photoService.UpdatePhotoAsync(id, photo);
                
                if (updatedPhoto == null)
                {
                    return NotFound(new { message = $"Photo with ID {id} not found" });
                }

                return Ok(updatedPhoto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //Elimina una foto
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "Invalid photo ID" });
            }

            var result = await _photoService.DeletePhotoAsync(id);
            
            if (!result)
            {
                return NotFound(new { message = $"Photo with ID {id} not found or could not be deleted" });
            }

            return NoContent();
        }
    }
}
