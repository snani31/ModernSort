using ModernSort.Services.Dialog;
using ModernSort.View.Windows;
using ModernSort.ViewModel;
using ModernSort.ViewModel.Windows;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using System.IO;
using System.Windows;
using ModernSort.Stores.Catalog;
using ModernSort.Services.Operations;
using ModernSort.Services;

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
        private readonly CatalogStore _catalogStore;
        private readonly OperationService _operationService ;
        private readonly OutputContentService _outputContentService;


        App()
        {
            _dialogService = new DialogService(MainWindow);
            _dialogService.Register<AddNewRankingCategoryViewModel,AddNewRankingCategoryWindowView>();
            _dialogService.Register<SelectedRankingCategoryViewModel,SelectedRankingCategoryWindowView >();
            _dialogService.Register<CreateMediaObjectViewModel, CreateMediaObjectWindowView>();
            _dialogService.Register<EditRankingWindowViewModel, EditRankingWindowView>();
            _dialogService.Register<EditMediaObjectViewModel, EditMediaObjectWindowView>();
            _jsonDeserializer = new JsonDeserializer();
            _jsonSerializer = new JsonSerializer();
            

            string applicationExecutableFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _catalogStore = new CatalogStore()
            {
                rankingCategoriesFileName = "RankingCategories.json",
                coreResourcesCatalogPath = applicationExecutableFile + "\\UserResources",
                GUIDsFileName = "ProjactGUIDSFile.txt",
                mediaFilesCatalogName = "Media",
                mediaObjectsFileName = "MediaObjacts.json",
                rankingCategoryIconNameNoExtention = "Ranking_Icon"
            };

            _operationService = new OperationService(_jsonSerializer,_jsonDeserializer,_catalogStore);
            _outputContentService = new OutputContentService(_catalogStore, _jsonDeserializer);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            MainWindow = new MainWindow()
            {
                DataContext = new MeinWindowViewModel(_outputContentService,_operationService, _catalogStore, _jsonDeserializer, _jsonSerializer, _dialogService)
            };
            MainWindow.Show();
        }

    }

}
