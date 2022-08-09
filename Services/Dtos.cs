
using LinkVault.Models;

namespace LinkVault.Services.Dtos
{
    public record ShowLinkCreationDto(bool isVisible, Link? link = null);
}