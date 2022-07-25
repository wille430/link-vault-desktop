using System.Reactive;
using LinkVault.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace LinkVault.ViewModels
{
    public class CreateCollectionViewModel : ViewModelBase
    {
        [Reactive]
        public string? Name { get; set; }

        public ReactiveCommand<Unit, LinkCollection> CreateCollectionCommand { get; }
        public ReactiveCommand<Unit, LinkCollection?> Cancel { get; }

        public CreateCollectionViewModel()
        {

            var CreateEnabled = this.WhenAnyValue(x => x.Name, (Name) => !string.IsNullOrWhiteSpace(Name));

            CreateCollectionCommand = ReactiveCommand.Create(() => new LinkCollection
            {
                Name = Name,
                CreatedAt = System.DateTime.Now,
            }, CreateEnabled);

            Cancel = ReactiveCommand.Create(() => (LinkCollection)null);
        }
    }
}

