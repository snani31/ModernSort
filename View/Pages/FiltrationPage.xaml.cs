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
        }

        private void ListView_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset - e.Delta/3);
                e.Handled = true;
            }
        }
    }
}
