using ModernSort.Services.Dialog;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace ModernSort.CustomeControls.CustomUIControls
{
    public class BaseWindow : Window,IDialog, INotifyPropertyChanged
    {
        public BaseWindow()
        {
            TopButtonsBarBorderCornerRadius = new CornerRadius(20,20,0,0);
            TopRightCloseButtonBorderCornerRadius = new CornerRadius(10, 20, 0, 10);
            BottonRightElementBorderCornerRadius = new CornerRadius(60, 0, 20, 0);
            BottonLeftElementBorderCornerRadius = new CornerRadius(0, 60, 0, 20);
            MainBorderCornerRadius = new CornerRadius(20, 20, 20, 20);
        }

        public CornerRadius TopButtonsBarBorderCornerRadius {  get; private set; }
        public CornerRadius TopRightCloseButtonBorderCornerRadius { get; private set; }
        public CornerRadius BottonRightElementBorderCornerRadius { get; private set; }
        public CornerRadius BottonLeftElementBorderCornerRadius { get; private set; }

        public CornerRadius MainBorderCornerRadius { get; private set; }

        static BaseWindow()
        { 
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChenged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void HideButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;

        }

        protected void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
                TopButtonsBarBorderCornerRadius = new CornerRadius(20, 20, 0, 0);
                TopRightCloseButtonBorderCornerRadius = new CornerRadius(10, 20, 0, 10);
                BottonRightElementBorderCornerRadius = new CornerRadius(60, 0, 20, 0);
                BottonLeftElementBorderCornerRadius = new CornerRadius(0, 60, 0, 20);
                MainBorderCornerRadius = new CornerRadius(20, 20, 20, 20);

            }
            else
            {
                WindowState = WindowState.Maximized;
                TopButtonsBarBorderCornerRadius = new CornerRadius(0, 0, 0, 0);
                TopRightCloseButtonBorderCornerRadius = new CornerRadius(10, 0, 0, 10);
                BottonRightElementBorderCornerRadius = new CornerRadius(60, 0, 0, 0);
                BottonLeftElementBorderCornerRadius = new CornerRadius(0, 60, 0, 0);
                MainBorderCornerRadius = new CornerRadius(0,0,0,0);
            }
            OnPropertyChenged(nameof(TopButtonsBarBorderCornerRadius));
            OnPropertyChenged(nameof(TopRightCloseButtonBorderCornerRadius));
            OnPropertyChenged(nameof(BottonRightElementBorderCornerRadius));
            OnPropertyChenged(nameof(BottonLeftElementBorderCornerRadius));
            OnPropertyChenged(nameof(MainBorderCornerRadius));
        }

        protected void Window_DragHandler(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected override void OnActivated(EventArgs e)
        {
            Topmost = true;
            base.OnActivated(e);
            Topmost = false;
        }
    }
}
