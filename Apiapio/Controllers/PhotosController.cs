using Microsoft.AspNetCore.Mvc;
using Apiapio.Models;
using Apiapio.Services;

namespace Apiapio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetAllPhotos()
        {
            var photos = await _photoService.GetAllPhotosAsync();
            return Ok(photos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PhotoDto>> GetPhotoById(int id)
        {
            var photo = await _photoService.GetPhotoByIdAsync(id);
            
            if (photo == null)
            {
                return NotFound($"Photo with ID {id} not found");
            }

            return Ok(photo);
        }

        [HttpGet("album/{albumId}")]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> GetPhotosByAlbum(int albumId)
        {
            var photos = await _photoService.GetPhotosByAlbumIdAsync(albumId);
            return Ok(photos);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<PhotoDto>>> SearchPhotos([FromQuery] string query)
        {
            var photos = await _photoService.SearchPhotosByTitleAsync(query);
            return Ok(photos);
        }
    }
}
