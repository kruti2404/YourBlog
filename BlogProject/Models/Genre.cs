namespace BlogProject.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set;}

        public virtual ICollection<Blogs> Blogs { get; set; }

    }
}
