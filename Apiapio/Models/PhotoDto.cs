using System.ComponentModel.DataAnnotations;

namespace Apiapio.Models
{
    public class PhotoDto
    {
        public int AlbumId { get; set; }
        
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "URL is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string Url { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "Invalid thumbnail URL format")]
        public string ThumbnailUrl { get; set; } = string.Empty;

    }
}