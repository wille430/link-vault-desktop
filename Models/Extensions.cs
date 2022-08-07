
using LinkVault.Api.Dtos;

namespace LinkVault.Models
{
    public static class Extensions
    {
        public static LinkDto AsDto(this Link link)
        {
            return new LinkDto(link.Id, link.Title, link.URL, link.Description, link.CollectionId, link.CreatedAt, link.UpdatedAt);
        }

        public static LinkCollectionDto AsDto(this LinkCollection col)
        {
            return new LinkCollectionDto(col.Id, col.Name, col.CreatedAt, col.UpdatedAt);
        }
    }
}