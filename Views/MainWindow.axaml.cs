using System.Threading.Tasks;
using LinkVault.Models;
using LinkVault.ViewModels;
using ReactiveUI;

namespace LinkVault.Views
{
    public partial class MainWindow : BaseWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            InitializeComponent();

            this.WhenActivated(d => d(ViewModel!.ShowDialog.RegisterHandler(DoShowDialogAsync)));
        }

        private async Task DoShowDialogAsync(InteractionContext<CreateCollectionViewModel, LinkCollection?> interaction)
        {
            var dialog = new CreateCollectionWindow();
            dialog.DataContext = interaction.Input;

            var result = await dialog.ShowDialog<LinkCollection?>(this);
            interaction.SetOutput(result);
        }
    }
}