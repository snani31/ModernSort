using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernSort.CustomeControls.CustomUIControls
{
    /// <summary>
    /// Логика взаимодействия для CustomButton.xaml
    /// </summary>
    public partial class CustomButton : Button, INotifyPropertyChanged
    {
        public ImageSource MouseHoverImageSource { get; set; }

        public ImageSource DefoultImageSource { get; set; }

        public ImageSource DisabledImageSource { get; set; }

        public HorizontalAlignment TextBlockHorizontalAlignment {  get; set; }

        public Thickness ImageMargin { get; set; }

        public CornerRadius CornerRadius { get; set; }

        public Brush FirstButtonColor { get; set; }
        public Brush SecondButtonColor { get; set; }

        public Brush BorderColorBrush 
        { get; private set; }

        public Thickness ImageMarginMouseHover
        {
            get
            {
                
                double top = this.BorderThickness.Top + ImageMargin.Top;
                double bottom = this.BorderThickness.Bottom + ImageMargin.Bottom;
                double left = this.BorderThickness.Left+ ImageMargin.Left;
                double right = this.BorderThickness.Right + ImageMargin.Right;

                return new Thickness(left, top,right,bottom);
            }
        }

        public int IndexOfImageColumnSpan { get;private set; } = 1;

        public string Text {  get; set; }

        public CustomButton()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChenged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void EndInit()
        {
            base.EndInit();
            IndexOfImageColumnSpan = (Text is not null && Text != String.Empty) ? 1 : 2;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BorderColorBrush = SecondButtonColor?.CloneCurrentValue() ?? (SolidColorBrush)new BrushConverter().ConvertFrom("#ffaacc");
            OnPropertyChenged(nameof(BorderColorBrush));
        }

        static CustomButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButton), new FrameworkPropertyMetadata(typeof(CustomButton)));
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush borderColorBrush = (SolidColorBrush)BorderColorBrush;
            SolidColorBrush firstColorBrush = (SolidColorBrush)FirstButtonColor;
            SolidColorBrush secondColorBrush = (SolidColorBrush)SecondButtonColor;


            var Animation = new ColorAnimation() { From = secondColorBrush.Color,To = firstColorBrush.Color, Duration = TimeSpan.FromSeconds(0.3), };
            borderColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, Animation);

        }


        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            SolidColorBrush borderColorBrush = (SolidColorBrush)BorderColorBrush;
            SolidColorBrush firstColorBrush = (SolidColorBrush)FirstButtonColor;
            SolidColorBrush secondColorBrush = (SolidColorBrush)SecondButtonColor;

            var Animation = new ColorAnimation() { From = firstColorBrush.Color, To = secondColorBrush.Color, Duration = TimeSpan.FromSeconds(0.3), };
            borderColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, Animation);
        }
    }
}
