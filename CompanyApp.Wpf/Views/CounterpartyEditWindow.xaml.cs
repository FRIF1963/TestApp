using System.Windows;
using CompanyApp.Wpf.ViewModels;

namespace CompanyApp.Wpf.Views;

public partial class CounterpartyEditWindow : Window
{
    public CounterpartyEditWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is CounterpartyEditViewModel oldViewModel)
        {
            oldViewModel.CloseRequested -= OnCloseRequested;
        }

        if (e.NewValue is CounterpartyEditViewModel newViewModel)
        {
            newViewModel.CloseRequested += OnCloseRequested;
        }
    }

    private void OnCloseRequested(bool? result)
    {
        DialogResult = result;
        Close();
    }
}
