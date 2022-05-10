using System.ComponentModel.DataAnnotations;

namespace IntegTest.Data
{
    public class Post
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string Text { get; set; } = null!;

        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;
    }
}
