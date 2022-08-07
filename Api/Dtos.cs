
using System;

namespace LinkVault.Api.Dtos
{

    public interface IPagination
    {
        int Page { get; set; }
        int Limit { get; set; }
    }

    public class GetColsDto : IPagination
    {
        public string? Keyword { get; set; }
        public int Page { get; set; } = 1;
        public int Limit { get; set; } = 20;
    }

    public class CreateColDto
    {
        public string? Name { get; set; }
    }

    public class GetLinksDto
    {
        public int? CollectionId { get; set; }
    }

    public class CreateLinkDto
    {
        public string? Title { get; set; }
        public string? URL { get; set; }
        public string? Description { get; set; }
        public int CollectionId { get; set; }
    }

    public partial class UpdateLinkDto : CreateLinkDto
    {
        public new int? CollectionId { get; set; }
    }

    public record LinkDto(
        int? Id,
        string Title,
        string URL,
        string Description,
        int? CollectionId,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);

    public record LinkCollectionDto(
        int? Id,
        string Name,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt);
}