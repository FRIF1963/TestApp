using System.Windows;
using CompanyApp.Wpf.ViewModels;

namespace CompanyApp.Wpf.Views;

public partial class OrderEditWindow : Window
{
    public OrderEditWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is OrderEditViewModel oldViewModel)
        {
            oldViewModel.CloseRequested -= OnCloseRequested;
        }

        if (e.NewValue is OrderEditViewModel newViewModel)
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
