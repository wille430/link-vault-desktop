using System.Reactive.Linq;
using System.Windows.Input;
using LinkVault.Models;
using LinkVault.Stores;
using ReactiveUI;

namespace LinkVault.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<CreateCollectionViewModel, LinkCollection?> ShowDialog { get; }

        public ICommand CreateCollection { get; }

        public CollectionStore CollectionStore { get; }

        public MainWindowViewModel(CollectionStore collectionStore)
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
        }
    }
}
