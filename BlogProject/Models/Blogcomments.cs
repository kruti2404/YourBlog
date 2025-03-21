namespace BlogProject.Models
{
    public class Blogcomments
    {
        public int Id { get; set; }
        public string CommentText { get; set; }
        public int UserID { get; set; }
        public int BlogId { get; set;  }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual Blogs blog { get; set; }
        public virtual User User { get; set; }
    }
}
