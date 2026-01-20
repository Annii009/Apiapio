using System.ComponentModel.DataAnnotations;

namespace Apiapio.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;
        
        public AddressDto? Address { get; set; }
        
        [Phone(ErrorMessage = "Invalid phone format")]
        public string Phone { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "Invalid website URL")]
        public string Website { get; set; } = string.Empty;
        
        public CompanyDto? Company { get; set; }
    }
}