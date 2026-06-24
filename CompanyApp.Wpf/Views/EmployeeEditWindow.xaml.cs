using System.Windows;
using CompanyApp.Wpf.ViewModels;

namespace CompanyApp.Wpf.Views;

public partial class EmployeeEditWindow : Window
{
    public EmployeeEditWindow()
    {
        InitializeComponent();
        DataContextChanged += OnDataContextChanged;
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is EmployeeEditViewModel oldViewModel)
        {
            oldViewModel.CloseRequested -= OnCloseRequested;
        }

        if (e.NewValue is EmployeeEditViewModel newViewModel)
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
