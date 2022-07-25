
using System;
using LinkVault.Context;
using LinkVault.Models;
using Splat;

namespace LinkVault.Stores
{
    public class LinkStore
    {
        public AppDbContext Context { get; }

        public LinkStore()
            : this(
                Locator.Current.GetService<AppDbContext>()!
            )
        {
        }

        public LinkStore(AppDbContext context)
        {
            Context = context;
        }

        public event Action<bool> LinkCreationVisible;
        public void ShowLinkCreation()
        {
            LinkCreationVisible?.Invoke(true);
        }

        public void HideLinkCreation()
        {
            LinkCreationVisible?.Invoke(false);
        }

        public event Action<Link> LinkCreated;
        public void CreateLink(Link link)
        {
            var createdLink = Context.Links.Add(link);
            LinkCreated?.Invoke(createdLink.Entity);
            Context.SaveChanges();
        }
    }
}