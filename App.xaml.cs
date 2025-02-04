using ModernSort.ViewModel;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
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

        IDeserializer _jsonDeserializer;
        ISerializer _jsonSerializer;
        protected override void OnStartup(StartupEventArgs e)
        {
            _jsonDeserializer = new JsonDeserializer();
            _jsonSerializer = new JsonSerializer();
            MainWindow = new MainWindow()
            {
                DataContext = new MeinWindowViewModel(_jsonDeserializer, _jsonSerializer)
            };
            MainWindow.Show();
        }

    }

}
