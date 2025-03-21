using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogProject.Models
{
    public class Blogs
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required ")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Content is required ")]
        public string Content { get; set; }

        [ForeignKey("User")]
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public virtual User? user { get; set; }
        public byte[]? Image { get; set; }

        public virtual ICollection<Genre> Genres { get; set; }
        public virtual ICollection<Likes>? Likes { get; set; }
        public virtual ICollection<Blogcomments>? Comments { get; set; }

    }
}
