using ModernSort.Services.Dialog;
using ModernSort.Static;
using ModernSort.Stores;
using ModernSort.View.Windows;
using ModernSort.ViewModel;
using ModernSort.ViewModel.Windows;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using System.Configuration;
using System.Data;
using System.IO;
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

        private readonly IDeserializer _jsonDeserializer;
        private readonly  ISerializer _jsonSerializer;
        private readonly IDialogService _dialogService;

        App()
        {
            _dialogService = new DialogService(MainWindow);
            _dialogService.Register<AddNewRankingCategoryViewModel,AddNewRankingCategoryWindowView>();
            _dialogService.Register<SelectedRankingCategoryViewModel,SelectedRankingCategoryWindowView >();
            _dialogService.Register<CreateMediaObjectViewModel, CreateMediaObjectWindowView>();
            _dialogService.Register<EditRankingWindowViewModel, EditRankingWindowView>();
            _jsonDeserializer = new JsonDeserializer();
            _jsonSerializer = new JsonSerializer();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MeinWindowViewModel(_jsonDeserializer, _jsonSerializer, _dialogService)
            };
            MainWindow.Show();
        }
    }

}
