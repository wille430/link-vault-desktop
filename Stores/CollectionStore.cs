
using System;
using LinkVault.Context;
using LinkVault.Models;
using Splat;

namespace LinkVault.Stores
{
    public class CollectionStore
    {
        public AppDbContext Context { get; }

        public CollectionStore()
            : this(
                Locator.Current.GetService<AppDbContext>()!
            )
        {
        }

        public CollectionStore(AppDbContext context)
        {
            Context = context;
        }

        public event Action<LinkCollection?> CollectionSelected;
        public void SelectCollection(LinkCollection? linkCollection)
        {
            CollectionSelected?.Invoke(linkCollection);
        }

        public event Action<LinkCollection> CollectionCreated;
        public void CreateCollection(LinkCollection linkCollection)
        {
            CollectionCreated?.Invoke(linkCollection);
            Context.Collections.Add(linkCollection);
        }
    }
}