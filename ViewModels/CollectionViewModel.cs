using System;
using LinkVault.Context;
using LinkVault.Models;
using Splat;
using ReactiveUI.Fody.Helpers;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;
using LinkVault.Services;
using LinkVault.Services.Dtos;

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

        MessageBusService MessageBusService { get; }

        public CollectionViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
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
                MessageBusService.Emit("LinkSelected", link);
            });
        }

        public CollectionViewModel(AppDbContext context, MessageBusService messageBusService)
        {
            Context = context;
            MessageBusService = messageBusService;

            // Register events
            MessageBusService.RegisterEvents("LinkCreated", OnLinkCreated);
            MessageBusService.RegisterEvents("LinkUpdated", OnLinkUpdated);
            MessageBusService.RegisterEvents("LinkDeleted", OnLinkDeleted);

            MessageBusService.RegisterEvents("CollectionSelected", OnCollectionSelected);
        }

        private void OnLinkDeleted(object obj)
        {
            var id = (int)obj;
            var link = Links.Where(link => link.Id == id).FirstOrDefault();

            if (link is not null)
                Links.Remove(link);
        }

        private void OnCollectionSelected(object? linkCollection)
        {
            LinkCollection = (LinkCollection?)linkCollection;
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

            MessageBusService.Emit("ShowLinkCreation", new ShowLinkCreationDto(false));
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

            MessageBusService.Emit("ShowLinkCreation", new ShowLinkCreationDto(false));
        }

        private void AddLink()
        {
            MessageBusService.Emit("ShowLinkCreation", new ShowLinkCreationDto(true));
        }
    }
}

