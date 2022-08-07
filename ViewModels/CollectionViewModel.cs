using System;
using LinkVault.Context;
using LinkVault.Models;
using Splat;
using ReactiveUI.Fody.Helpers;
using LinkVault.Stores;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using LinkVault.Services;

namespace LinkVault.ViewModels
{
    public class CollectionViewModel : ViewModelBase
    {
        AppDbContext Context { get; }

        [Reactive]
        public LinkCollection? LinkCollection { get; set; }

        [Reactive]
        public bool HasCollection { get; set; }

        [Reactive]
        public ObservableCollection<Link> Links { get; set; }

        [Reactive]
        public Link? SelectedLink { get; set; }

        public CollectionStore CollectionStore;

        public LinkStore LinkStore;

        MessageBusService MessageBusService { get; }

        public CollectionViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<CollectionStore>()!,
                Locator.Current.GetService<LinkStore>()!,
                Locator.Current.GetService<MessageBusService>()!
            )
        {
            this.WhenAnyValue(x => x.LinkCollection).Subscribe(x =>
            {
                if (x is not null)
                    Links = new ObservableCollection<Link>(x.Links.ToList());
                else
                    Links = new();
            });

            this.WhenAnyValue(x => x.LinkCollection).Subscribe(x =>
            {
                this.HasCollection = x is not null;
            });

            this.WhenAnyValue(x => x.SelectedLink).Subscribe(link =>
            {
                LinkStore.ShowLinkCreation(link);
            });
        }

        public CollectionViewModel(AppDbContext context, CollectionStore collectionStore, LinkStore linkStore, MessageBusService messageBusService)
        {
            Context = context;
            CollectionStore = collectionStore;
            LinkStore = linkStore;
            MessageBusService = messageBusService;

            CollectionStore.CollectionSelected += OnCollectionSelected;
            LinkStore.LinkCreated += OnLinkCreated;
            LinkStore.LinkUpdated += OnLinkUpdated;

            // Register events
            MessageBusService.RegisterEvents("LinkCreated", OnLinkCreated);
            MessageBusService.RegisterEvents("LinkUpdated", OnLinkUpdated);
            MessageBusService.RegisterEvents("LinkDeleted", OnLinkDeleted);
        }

        private void OnLinkDeleted(object obj)
        {
            var id = (int)obj;
            var link = Links.Where(link => link.Id == id).FirstOrDefault();

            if (link is not null)
                Links.Remove(link);
        }

        private void OnCollectionSelected(LinkCollection? linkCollection)
        {
            LinkCollection = linkCollection;
        }

        private void OnLinkCreated(object obj)
        {
            var link = (Link)obj;
            if (link.CollectionId == LinkCollection?.Id)
            {
                LinkCollection?.Links.Add(link);
                Links.Add(link);
            }
            else
            {
                LinkCollection = link.Collection ?? Context.Collections.Find(link.CollectionId);
            }

            LinkStore.HideLinkCreation();
        }

        private void OnLinkUpdated(object obj)
        {
            var link = (Link)obj;
            if (link.CollectionId == LinkCollection?.Id)
            {
                var index = Links.ToList().FindIndex(0, Links.Count, x => x.Id == link.Id);

                if (index >= 0)
                {
                    LinkCollection?.Links.RemoveAt(index);
                    LinkCollection?.Links.Insert(index, link);

                    Links.RemoveAt(index);
                    Links.Insert(index, link);
                }
                else
                {
                    OnLinkCreated(link);
                }
            }
            else
            {
                LinkCollection = link.Collection ?? Context.Collections.Find(link.CollectionId);
            }
            LinkStore.HideLinkCreation();
        }

        private void AddLink()
        {
            LinkStore.ShowLinkCreation();
        }
    }
}

