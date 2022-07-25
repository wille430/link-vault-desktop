using System;
using LinkVault.ViewModels;
using ReactiveUI;

namespace LinkVault.Views
{
    public partial class CreateCollectionWindow : BaseWindow<CreateCollectionViewModel>
    {
        public CreateCollectionWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.CreateCollectionCommand.Subscribe(Close)));
            this.WhenActivated(d => d(ViewModel!.Cancel.Subscribe(Close)));
        }
    }
}