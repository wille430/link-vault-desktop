using System.Collections.ObjectModel;
using LinkVault.Context;
using LinkVault.Models;
using System.Linq;
using Splat;
using System;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using LinkVault.Stores;

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

        public CollectionExplorerViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<CollectionStore>()!
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

        public CollectionExplorerViewModel(AppDbContext context, CollectionStore collectionStore)
        {
            _context = context;
            CollectionStore = collectionStore;

            CollectionStore.CollectionCreated += OnCollectionCreated;
        }

        private void OnCollectionCreated(LinkCollection obj)
        {
            LinkCollections.Add(obj);
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

