using Windows.UI.Xaml.Controls;

namespace SPLITTR_Uwp.ViewModel.Contracts;

internal interface IMainView
{
    Frame ChildFrame { get; }
}