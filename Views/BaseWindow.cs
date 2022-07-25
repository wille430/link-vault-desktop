
using System.ComponentModel;
using Avalonia.ReactiveUI;

namespace LinkVault.Views
{
    public class BaseWindow<TViewModel> : ReactiveWindow<TViewModel>
        where TViewModel : class
    {
    }
}