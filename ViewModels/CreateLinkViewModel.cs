using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using LinkVault.Context;
using LinkVault.Models;
using LinkVault.Stores;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LinkVault.ViewModels
{
    public class CreateLinkViewModel : ViewModelBase
    {

        [Reactive]
        public bool IsVisible { get; set; } = false;

        [Reactive]
        public string? Title { get; set; }
        [Reactive]
        public string? URL { get; set; }
        [Reactive]
        public string? Description { get; set; }

        [Reactive]
        public LinkCollection? Collection { get; set; }

        public ObservableCollection<LinkCollection> LinkCollections { get; set; }

        public ReactiveCommand<Unit, Link> CreateLinkCommand { get; }
        public ReactiveCommand<Unit, Link?> Cancel { get; }

        public AppDbContext Context;

        public LinkStore LinkStore;

        public CreateLinkViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<LinkStore>()!
            )
        {

            LinkCollections = new();

            var CreateEnabled = this.WhenAnyValue(
                x => x.Title,
                x => x.URL,
                x => x.Collection,
                (Title, URL, Collection) => !string.IsNullOrWhiteSpace(Title)
                                                         && !string.IsNullOrWhiteSpace(URL)
                                                         && Collection != null);

            CreateLinkCommand = ReactiveCommand.Create(() => new Link
            {
                Title = Title!,
                URL = URL!,
                Description = Description!,
                CollectionId = Collection!.Id,
                Collection = Collection!,
                CreatedAt = System.DateTime.Now,
            }, CreateEnabled);

            Cancel = ReactiveCommand.Create(() => (Link)null!);

            CreateLinkCommand.Subscribe(link =>
            {
                LinkStore.CreateLink(link);
            });

            Cancel.Subscribe(x =>
            {
                LinkStore.HideLinkCreation();
            });
        }

        public CreateLinkViewModel(AppDbContext context, LinkStore linkStore)
        {
            Context = context;
            LinkStore = linkStore;

            LinkStore.LinkCreationVisible += OnLinkCreationVisible;
        }

        private void OnLinkCreationVisible(bool isVisible)
        {
            IsVisible = isVisible;
        }

        public void LoadCollections()
        {
            var collections = Context.Collections.ToList();
            foreach (var collection in collections)
            {
                LinkCollections.Add(collection);
            }
        }
    }
}