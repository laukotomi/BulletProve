using System.ComponentModel.DataAnnotations;

namespace Example.Api.Data
{
    /// <summary>
    /// The post.
    /// </summary>
    public class Post
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        [MaxLength(1000)]
        public string Text { get; set; } = null!;

        /// <summary>
        /// Gets or sets the creator id.
        /// </summary>
        public int CreatorId { get; set; }

        /// <summary>
        /// Gets or sets the creator.
        /// </summary>
        public User Creator { get; set; } = null!;
    }
}
