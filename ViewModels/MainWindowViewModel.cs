using System;
using System.Reactive.Linq;
using System.Windows.Input;
using LinkVault.Models;
using LinkVault.Services;
using LinkVault.Services.Dtos;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace LinkVault.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<CreateCollectionViewModel, LinkCollection?> ShowDialog { get; }

        public ICommand CreateCollection { get; }

        [Reactive]
        public CreateLinkViewModel? CreateLinkViewModel { get; set; }

        [Reactive]
        public int ColumnSpan { get; set; } = 1;

        MessageBusService MessageBusService { get; }

        public MainWindowViewModel(MessageBusService messageBusService)
        {
            MessageBusService = messageBusService;

            ShowDialog = new Interaction<CreateCollectionViewModel, LinkCollection?>();
            CreateCollection = ReactiveCommand.CreateFromTask(async () =>
            {
                var collectionCreation = new CreateCollectionViewModel();
                var result = await ShowDialog.Handle(collectionCreation);

                if (result != null)
                    MessageBusService.Emit("CollectionCreated", result);
            });

            this.WhenAnyValue(x => x.CreateLinkViewModel).Subscribe(x =>
            {
                this.ColumnSpan = x is null ? 2 : 1;
            });

            MessageBusService.RegisterEvents("ShowLinkCreation", OnLinkCreationVisible);

        }

        private void OnLinkCreationVisible(object obj)
        {
            var (isVisible, link) = (ShowLinkCreationDto)obj;
            CreateLinkViewModel = isVisible ? new CreateLinkViewModel() : null;

            if (CreateLinkViewModel is not null)
                CreateLinkViewModel.SelectedLink = link;
        }
    }
}
