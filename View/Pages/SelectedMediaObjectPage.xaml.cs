using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernSort.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для SelectedMediaObjectPage.xaml
    /// </summary>
    public partial class SelectedMediaObjectPage : UserControl
    {

        ListView ListView1 { get; set; }
        public SelectedMediaObjectPage()
        {
            InitializeComponent();
            ListView1 = MediaFilesList;
            ListView1.SelectionChanged += SelectionShangeClosure();
        }

        private SelectionChangedEventHandler SelectionShangeClosure()
        {

            ListViewItem? ListViewSelectedItemContainer;

            return (object sender, SelectionChangedEventArgs e) =>
            {
                ListViewSelectedItemContainer = ListView1
                .ItemContainerGenerator
                .ContainerFromItem(ListView1.SelectedItem) as ListViewItem;

                ListViewSelectedItemContainer?.Focus();

            };
        }
    }
}
