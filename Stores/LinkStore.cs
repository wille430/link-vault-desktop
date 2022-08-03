
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

        public event Action<bool, Link?> LinkCreationVisible;
        public void ShowLinkCreation(Link? link = null)
        {
            LinkCreationVisible?.Invoke(true, link);
        }

        public void HideLinkCreation()
        {
            LinkCreationVisible?.Invoke(false, null);
        }

        public event Action<Link> LinkCreated;
        public event Action<Link> LinkUpdated;
        public void CreateLink(Link link)
        {
            if (link.Id is null)
            {
                // Create
                var createdLink = Context.Links.Add(link);
                LinkCreated?.Invoke(createdLink.Entity);
            }
            else
            {
                // Update
                var entity = Context.Links.Find(link.Id);

                if (entity is not null)
                {
                    entity.Title = link.Title;
                    entity.URL = link.URL;
                    entity.Description = link.Description;
                    entity.CollectionId = link.CollectionId;

                    var updatedLink = Context.Links.Update(entity);
                    LinkUpdated?.Invoke(updatedLink.Entity);
                }

            }
            Context.SaveChanges();
        }

        public event Action<Link> LinkDeleted;
        public void DeleteLink(Link link)
        {
            Context.Links.Remove(link);
            LinkDeleted?.Invoke(link);
            Context.SaveChanges();
        }
    }
}