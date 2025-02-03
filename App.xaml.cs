using ModernSort.ViewModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ModernSort
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {


        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MeinWindowViewModel()
            };
            MainWindow.Show();
        }

    }

}
