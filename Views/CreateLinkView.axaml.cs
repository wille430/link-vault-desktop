using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LinkVault.ViewModels;
using ReactiveUI;

namespace LinkVault.Views
{
    public partial class CreateLinkView : BaseController<CreateLinkViewModel>
    {
        public CreateLinkView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                ViewModel!.LoadCollections();
            });
        }
    }
}