using System.Collections.ObjectModel;
using LinkVault.Context;
using LinkVault.Models;
using System.Linq;
using Splat;
using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LinkVault.Stores;
using LinkVault.Services;

namespace LinkVault.ViewModels
{
    public class CollectionExplorerViewModel : ViewModelBase
    {
        public ObservableCollection<LinkCollection> LinkCollections { get; } = new();

        public CollectionStore CollectionStore;

        [Reactive]
        public LinkCollection? SelectedCollection { get; set; }

        [Reactive]
        public string SearchText { get; set; } = "";

        AppDbContext _context { get; }

        MessageBusService MessageBusService { get; }

        public CollectionExplorerViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<CollectionStore>()!,
                Locator.Current.GetService<MessageBusService>()!
            )
        {
            this.WhenAnyValue(x => x.SearchText).Subscribe(searchText =>
            {
                GetSearchResult();
            });

            this.WhenAnyValue(x => x.SelectedCollection).Subscribe(selectedCollection =>
            {
                CollectionStore.SelectCollection(selectedCollection);
            });
        }

        public CollectionExplorerViewModel(AppDbContext context, CollectionStore collectionStore, MessageBusService messageBusService)
        {
            _context = context;
            CollectionStore = collectionStore;
            MessageBusService = messageBusService;

            CollectionStore.CollectionCreated += OnCollectionCreated;

            // Register events
            MessageBusService.RegisterEvents("CollectionCreated", OnCollectionCreated);
            MessageBusService.RegisterEvents("CollectionUpdated", OnCollectionUpdated);
        }

        private void OnCollectionUpdated(object obj)
        {
            var collection = (LinkCollection)obj;

            var originalCollection = LinkCollections.Where(col => col.Id == collection.Id).FirstOrDefault();

            // Collection should already exist. But in case not, append it.
            if (originalCollection is null)
            {
                LinkCollections.Add(collection);
            }
            else
            {
                var index = LinkCollections.IndexOf(originalCollection);
                LinkCollections[index] = collection;
            }

        }

        private void OnCollectionCreated(object obj)
        {
            LinkCollections.Add((LinkCollection)obj);
        }


        public void GetSearchResult()
        {
            LinkCollections.Clear();

            foreach (LinkCollection linkCollection in _context.Collections.Where(GetSearchFilter()).ToList())
            {
                _context.Entry(linkCollection).Collection(c => c.Links).Load();
                Console.WriteLine(linkCollection);
                LinkCollections.Add(linkCollection);
            }
        }

        public Func<LinkCollection, bool> GetSearchFilter()
        {
            return col =>
            {
                if (string.IsNullOrWhiteSpace(SearchText))
                    return true;

                return col.Name.Contains(SearchText, StringComparison.InvariantCultureIgnoreCase);
            };
        }
    }
}

