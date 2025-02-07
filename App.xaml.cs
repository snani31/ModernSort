using ModernSort.Stores;
using ModernSort.ViewModel;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace ModernSort
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly NavigationStore _navigationStore;
        private readonly IDeserializer _jsonDeserializer;
        private readonly  ISerializer _jsonSerializer;
        public Action<object, MouseButtonEventArgs> BorderMouseDown {  get; init; }

        App()
        {
            BorderMouseDown = Border_MouseDown;
            _navigationStore = new NavigationStore();
            _jsonDeserializer = new JsonDeserializer();
            _jsonSerializer = new JsonSerializer();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _navigationStore.CurrentViewModel = new MeinWindowViewModel(_jsonDeserializer,_jsonSerializer);
            MainWindow = new MainWindow()
            {
                DataContext = new MeinWindowViewModel(_jsonDeserializer, _jsonSerializer)
            };
            MainWindow.Show();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                MainWindow.DragMove();
            }
        }

    }

}
