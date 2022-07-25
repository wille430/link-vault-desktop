using System;
using LinkVault.Context;
using LinkVault.Models;
using Splat;
using ReactiveUI.Fody.Helpers;
using LinkVault.Stores;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace LinkVault.ViewModels
{
    public class CollectionViewModel : ViewModelBase
    {
        AppDbContext Context { get; }

        [Reactive]
        public LinkCollection? LinkCollection { get; set; }

        [Reactive]
        public ObservableCollection<Link> Links { get; set; }

        public CollectionStore CollectionStore;

        public LinkStore LinkStore;

        public CollectionViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<CollectionStore>()!,
                Locator.Current.GetService<LinkStore>()!
            )
        {
            this.WhenAnyValue(x => x.LinkCollection).Subscribe(x =>
            {
                if (x is not null)
                    Links = new ObservableCollection<Link>(x.Links.ToList());
                else
                    Links = new();
            });
        }

        public CollectionViewModel(AppDbContext context, CollectionStore collectionStore, LinkStore linkStore)
        {
            Context = context;
            CollectionStore = collectionStore;
            LinkStore = linkStore;

            CollectionStore.CollectionSelected += OnCollectionSelected;
            LinkStore.LinkCreated += OnLinkCreated;
        }

        private void OnCollectionSelected(LinkCollection? linkCollection)
        {
            LinkCollection = linkCollection;
        }

        private void OnLinkCreated(Link link)
        {
            if (link.CollectionId == LinkCollection?.Id)
            {
                LinkCollection?.Links.Add(link);
                Links.Add(link);
            }
            else
            {
                LinkCollection = link.Collection ?? Context.Collections.Find(link.CollectionId);
            }
        }

        private void AddLink()
        {
            LinkStore.ShowLinkCreation();
        }
    }
}

