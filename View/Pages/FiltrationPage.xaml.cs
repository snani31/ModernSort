using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ModernSort.View.Pages
{
    /// <summary>
    /// Логика взаимодействия для FiltrationPage.xaml
    /// </summary>
    public partial class FiltrationPage : UserControl
    {
        public FiltrationPage()
        {
            InitializeComponent();
            ExpandCheckBoxUncheckHandler = ExpandCheckBoxClosure(Visibility.Collapsed);
            ExpandCheckBoxCheckHandler = ExpandCheckBoxClosure(Visibility.Visible);
        }

        private Action<object, RoutedEventArgs> ExpandCheckBoxUncheckHandler {  get; init; }
        private Action<object, RoutedEventArgs> ExpandCheckBoxCheckHandler { get; init; }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset - e.Delta/3);
                e.Handled = true;
            }
        }

        private Action<object, RoutedEventArgs> ExpandCheckBoxClosure(Visibility visibility)
        {
            return (object sender, RoutedEventArgs e) =>
            {
                if (sender is CheckBox checkBox)
                {
                    ListView ancestorListView = checkBox.FindName("FiltersListView") as ListView;
                    ancestorListView.Visibility = visibility;
                }
            };
        }
    }
}
