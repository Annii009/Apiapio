namespace Apiapio.Models
{
    public class AddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string Suite { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public GeoDto? Geo { get; set; }
    }
}