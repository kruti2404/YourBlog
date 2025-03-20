using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Models
{
    public class BlogsGenreDetails
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public byte[]? Image { get; set; }
        public string? GenreName { get; set; }
    }
}
