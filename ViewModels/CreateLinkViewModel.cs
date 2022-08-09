using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using LinkVault.Context;
using LinkVault.Models;
using LinkVault.Services;
using LinkVault.Services.Dtos;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LinkVault.ViewModels
{
    public class CreateLinkViewModel : ViewModelBase
    {
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

        [Reactive]
        public string Heading { get; set; }

        [Reactive]
        public Link? SelectedLink { get; set; }

        MessageBusService MessageBusService { get; }

        public CreateLinkViewModel()
            : this(
                Locator.Current.GetService<AppDbContext>()!,
                Locator.Current.GetService<MessageBusService>()!
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
                Id = SelectedLink?.Id,
                Title = Title!,
                URL = URL!,
                Description = Description!,
                CollectionId = Collection!.Id,
                Collection = Collection!,
                CreatedAt = System.DateTime.Now,
            }, CreateEnabled);

            Cancel = ReactiveCommand.Create(() => (Link?)null);

            CreateLinkCommand.Subscribe(link =>
            {
                MessageBusService.Emit("LinkCreated", link);
            });

            Cancel.Subscribe(x =>
            {
                MessageBusService.Emit("ShowLinkCreation", new ShowLinkCreationDto(false));
            });

            this.WhenAnyValue(x => x.SelectedLink).Subscribe(link =>
            {
                if (link is null)
                {
                    Heading = "Create Link";

                    Title = "";
                    URL = "";
                    Description = "";
                    Collection = null;
                }
                else
                {
                    Heading = $"Update {link.Title}";

                    Title = link.Title;
                    URL = link.URL;
                    Description = link.Description;
                    Collection = link.Collection;
                }
            });
        }

        public CreateLinkViewModel(AppDbContext context, MessageBusService messageBusService)
        {
            Context = context;
            MessageBusService = messageBusService;
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