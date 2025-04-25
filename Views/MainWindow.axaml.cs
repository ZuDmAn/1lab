using Avalonia.Controls;
using AvaloniaLinearListApp.ViewModels;

namespace AvaloniaLinearListApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new LinearListViewModel(); 
        }
    }
}