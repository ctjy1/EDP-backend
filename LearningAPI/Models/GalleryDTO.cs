namespace UPlay.Models
{
    public class GalleryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string? ImageFile { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int UserId { get; set; }
        public UserDTO? User { get; set; }
    }
}
