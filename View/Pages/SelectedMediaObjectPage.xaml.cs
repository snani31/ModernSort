using System.Windows.Controls;

namespace ModernSort.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для SelectedMediaObjectPage.xaml
    /// </summary>
    public partial class SelectedMediaObjectPage : UserControl
    {
        public SelectedMediaObjectPage()
        {
            InitializeComponent();
            MediaFilesList.SelectionChanged += SelectionChangeClosure();
        }

        private SelectionChangedEventHandler SelectionChangeClosure()
        {

            ListViewItem? ListViewSelectedItemContainer;

            return (object sender, SelectionChangedEventArgs e) =>
            {
                ListViewSelectedItemContainer = MediaFilesList
                .ItemContainerGenerator
                .ContainerFromItem(MediaFilesList.SelectedItem) as ListViewItem;

                ListViewSelectedItemContainer?.Focus();

            };
        }
    }
}
