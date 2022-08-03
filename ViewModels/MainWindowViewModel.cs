using System;
using System.Reactive.Linq;
using System.Windows.Input;
using LinkVault.Models;
using LinkVault.Stores;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LinkVault.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<CreateCollectionViewModel, LinkCollection?> ShowDialog { get; }

        public ICommand CreateCollection { get; }

        public CollectionStore CollectionStore { get; }
        public LinkStore LinkStore { get; }

        [Reactive]
        public CreateLinkViewModel? CreateLinkViewModel { get; set; }

        [Reactive]
        public int ColumnSpan { get; set; } = 1;


        public MainWindowViewModel(CollectionStore collectionStore)
            : this(Locator.Current.GetService<LinkStore>()!)
        {
            CollectionStore = collectionStore;

            ShowDialog = new Interaction<CreateCollectionViewModel, LinkCollection?>();
            CreateCollection = ReactiveCommand.CreateFromTask(async () =>
            {
                var collectionCreation = new CreateCollectionViewModel();
                var result = await ShowDialog.Handle(collectionCreation);

                if (result != null)
                    CollectionStore.CreateCollection(result);
            });

            this.WhenAnyValue(x => x.CreateLinkViewModel).Subscribe(x =>
            {
                this.ColumnSpan = x is null ? 2 : 1;
            });

        }

        public MainWindowViewModel(LinkStore linkStore)
        {
            LinkStore = linkStore;
            LinkStore.LinkCreationVisible += OnLinkCreationVisible;
        }

        private void OnLinkCreationVisible(bool isVisible, Link? link)
        {
            CreateLinkViewModel = isVisible ? new CreateLinkViewModel() : null;

            if (CreateLinkViewModel is not null)
                CreateLinkViewModel.SelectedLink = link;
        }
    }
}
