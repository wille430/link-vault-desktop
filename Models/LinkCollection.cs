using System;
using System.Collections.Generic;

namespace LinkVault.Models
{
    public class LinkCollection
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public List<Link> Links { get; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public LinkCollection()
        {
            this.CreatedAt = DateTime.Now;
            this.UpdatedAt = DateTime.Now;
        }
    }
}