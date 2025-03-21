namespace BlogProject.Models
{
    public class Likes
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BlogId { get; set; }
        public DateTime LikedAt { get; set; }
        public bool? IsActive {get; set;} = true;
        public virtual User User { get; set; }
        public virtual Blogs Blog { get; set; }
    }
}
