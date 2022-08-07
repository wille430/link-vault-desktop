using System;
using System.Collections.Generic;
using LinkVault.Api.Dtos;

namespace LinkVault.Models
{
    public class LinkCollection : ModelBase<LinkCollectionDto>
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

        public override LinkCollectionDto AsDto()
        {
            return new LinkCollectionDto(Id, Name, CreatedAt, UpdatedAt);
        }
    }
}