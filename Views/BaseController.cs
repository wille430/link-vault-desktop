
using Avalonia.ReactiveUI;

namespace LinkVault.Views
{
    public class BaseController<TViewModel> : ReactiveUserControl<TViewModel>
        where TViewModel : class
    {
    }
}