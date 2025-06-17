using ModernSort.Enums;
using ModernSort.Services;
using ModernSort.Services.Dialog;
using ModernSort.Services.Operations;
using ModernSort.Services.RankingContent;
using ModernSort.Services.UITheme;
using ModernSort.Static;
using ModernSort.Stores.Catalog;
using ModernSort.View.Windows;
using ModernSort.ViewModel;
using ModernSort.ViewModel.Windows;
using RankingEntityes.Filters;
using RankingEntityes.IO_Entities.Classes;
using RankingEntityes.IO_Entities.Interfaces;
using RankingEntityes.Json.Converters;
using RankingEntityes.Ranking_Entityes.MediaObjacts;
using RankingEntityes.Ranking_Entityes.Ranking_Categories;
using System;
using System.IO;
using System.Windows;

namespace ModernSort
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly IDeserializer _jsonDeserializer;
        private readonly  ISerializer _jsonSerializer;
        private IDialogService _dialogService;
        private readonly CatalogStore _catalogStore;
        private readonly OperationService _operationService ;
        private readonly OutputContentService _outputContentService;
        private readonly UIThemeService _themeService;


        App()
        {
            
            _jsonDeserializer = new JsonDeserializer();
            _jsonSerializer = new JsonSerializer();
            

            string applicationExecutableFile = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            _catalogStore = new CatalogStore()
            {
                filtersCriterionFileName = "FiltersCriterions.json",
                rankingCategoriesFileName = "RankingCategories.json",
                coreResourcesCatalogPath = applicationExecutableFile + "\\UserResources",
                GUIDsFileName = "ProjactGUIDSFile.txt",
                mediaFilesCatalogName = "Media",
                mediaObjectsFileName = "MediaObjacts.json",
                rankingCategoryIconNameNoExtention = "Ranking_Icon",
                SelectedUIThemeFileName = "SelectedUITheme.txt"
            };

            var jsonFilterConverter = new FiltersConverterJson();
            jsonFilterConverter.Register<ConditionFilter>(nameof(ConditionFilter));

            var filterCriterionsConverterJson = new FilterCriterionsConverterJson(jsonFilterConverter);
            filterCriterionsConverterJson.Register<ConditionFilterCriterion>(nameof(ConditionFilterCriterion));

            IDeserializer jsonDeserializerWithFilterConvertation = new JsonDeserializer(jsonFilterConverter);
            IDeserializer jsonDeserializerWithFilterCriterionConverter = new JsonDeserializer(filterCriterionsConverterJson);

            var filterCriterionsContentService = new FilterCriterionsContentService(jsonDeserializerWithFilterCriterionConverter, _catalogStore);
            var mediaObjectsContentService = new MediaObjectContentService(jsonDeserializerWithFilterConvertation, _catalogStore);

            _operationService = new OperationService(_jsonSerializer,_catalogStore);
            _operationService.RegistrateDeserializer(typeof(MediaObject), jsonDeserializerWithFilterConvertation);
            _operationService.RegistrateDeserializer(typeof(FilterCriterion), jsonDeserializerWithFilterCriterionConverter);
            _operationService.RegistrateDeserializer(typeof(RankingCategory), _jsonDeserializer);

            _outputContentService = new OutputContentService(_catalogStore, _jsonDeserializer, filterCriterionsContentService,mediaObjectsContentService);


            List<string> pathsOfRequiredFiles = new List<string>() { _catalogStore.GUIDsFilePath,
                _catalogStore.RankingCategoriesFilePath,_catalogStore.SelectedUIThemeFilePath};
            ProjactIoWorker.ControlRequiredFilesExistence(new List<string>() { _catalogStore.coreResourcesCatalogPath },
                pathsOfRequiredFiles
                );

            _themeService = new UIThemeService(_catalogStore, UIThemes.DeepPurple, new Uri("AppResources/ApplicationThemes/DeepPurpleTheme.xaml", UriKind.Relative));
            _themeService.ThemeRegister(UIThemes.DarkestGreen, new Uri("AppResources/ApplicationThemes/DarkestGreen.xaml", UriKind.Relative));
            _themeService.ThemeRegister(UIThemes.Pinapple, new Uri("AppResources/ApplicationThemes/PinappleTheme.xaml", UriKind.Relative));
            _themeService.ThemeRegister(UIThemes.SeeRed, new Uri("AppResources/ApplicationThemes/SeeRed.xaml", UriKind.Relative));


        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _themeService.SetApplicationThemeToSelected();

            MainWindow = new MainWindow();

            _dialogService = new DialogService(MainWindow);
            _dialogService.Register<AddNewRankingCategoryViewModel, AddNewRankingCategoryWindowView>();
            _dialogService.Register<SelectedRankingCategoryViewModel, SelectedRankingCategoryWindowView>();
            _dialogService.Register<CreateMediaObjectViewModel, CreateMediaObjectWindowView>();
            _dialogService.Register<EditRankingWindowViewModel, EditRankingWindowView>();
            _dialogService.Register<EditMediaObjectViewModel, EditMediaObjectWindowView>();
            _dialogService.Register<CreateFilterCriterionViewModel, CreateFilterCriterionWindowView>();
            _dialogService.Register<EditFilterCriterionViewModel, EditFilterCriterionWindowView>();

            var a = new MeinWindowViewModel(_outputContentService, _operationService, _catalogStore, _dialogService,_themeService);

            MainWindow.DataContext = a;

            MainWindow.Show();


        }

    }

}
