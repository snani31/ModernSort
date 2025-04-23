using ModernSort.CustomeControls.CustomUIControls;
using System.Windows.Controls;

namespace ModernSort.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditMediaObjectWindowView.xaml
    /// </summary>
    public partial class EditMediaObjectWindowView : BaseWindow
    {
        public EditMediaObjectWindowView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                scrollViewer.ScrollToVerticalOffset(
                scrollViewer.VerticalOffset - e.Delta / 3);
                e.Handled = true;
            }
        }
    }
}
