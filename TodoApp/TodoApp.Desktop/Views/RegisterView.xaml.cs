using System.Windows.Controls;
using TodoApp.Desktop.ViewModels;

namespace TodoApp.Desktop.Views;

public partial class RegisterView : UserControl
{
    public RegisterView()
    {
        InitializeComponent();
    }

    private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is RegisterViewModel viewModel && sender is PasswordBox passwordBox)
        {
            viewModel.Password = passwordBox.Password;
        }
    }
}
