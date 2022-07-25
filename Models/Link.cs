using System;

namespace LinkVault.Models
{
    public class Link
    {
        public int? Id { get; set; }
        public string URL { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? CollectionId { get; set; }
        public LinkCollection? Collection { get; set; }

        public Link()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
        }
    }
}