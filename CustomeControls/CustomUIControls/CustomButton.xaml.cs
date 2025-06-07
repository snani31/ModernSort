using System.ComponentModel;
using System.Drawing;
using System.Printing.IndexedProperties;
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
    public partial class CustomButton : Button
    {
        public static readonly DependencyProperty BorderColorBrushProperty;
        public static readonly DependencyProperty DisabledStateColorProperty;
        public static readonly DependencyProperty FirstButtonColorProperty;
        public static readonly DependencyProperty SecondButtonColorProperty;
        public static readonly DependencyProperty MouseHoverImageSourceProperty;
        public static readonly DependencyProperty DefoultImageSourceProperty;
        public static readonly DependencyProperty DisabledImageSourceProperty;
        public static readonly DependencyProperty TextProperty;
        public static readonly DependencyProperty ImageFlowDirectionProperty;


        public FlowDirection ImageFlowDirection 
        {
            get
            {
                return (FlowDirection)GetValue(ImageFlowDirectionProperty);
            }
            set
            {
                SetValue(ImageFlowDirectionProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(ImageFlowDirectionProperty, ImageFlowDirection, value));
            }
        }

        public ImageSource MouseHoverImageSource 
        {
            get
            {
                return (ImageSource)GetValue(MouseHoverImageSourceProperty);
            }
            set
            {
                SetValue(MouseHoverImageSourceProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(MouseHoverImageSourceProperty, MouseHoverImageSource, value));
            }
        }

        public ImageSource DefoultImageSource 
        {
            get
            {
                return (ImageSource)GetValue(DefoultImageSourceProperty);
            }
            set
            {
                SetValue(DefoultImageSourceProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(DefoultImageSourceProperty, DefoultImageSource, value));
            }
        }

        public ImageSource DisabledImageSource
        {
            get
            {
                return (ImageSource)GetValue(DisabledImageSourceProperty);
            }
            set
            {
                SetValue(DisabledImageSourceProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(DisabledImageSourceProperty, DisabledImageSource, value));
            }
        }

        public HorizontalAlignment TextBlockHorizontalAlignment {  get; set; }

        public Thickness ImageMargin { get; set; }

        public CornerRadius CornerRadius { get; set; }

        public SolidColorBrush DisabledStateColor
        {
            get 
            { 
                return (SolidColorBrush)GetValue(DisabledStateColorProperty);
            }
            set 
            { 
                SetValue(DisabledStateColorProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(SecondButtonColorProperty, SecondButtonColor, value));
            }
        }

        public SolidColorBrush FirstButtonColor
        {
            get 
            { 
                return (SolidColorBrush)GetValue(FirstButtonColorProperty);
            }
            set 
            { 
                SetValue(FirstButtonColorProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(FirstButtonColorProperty,FirstButtonColor,value));
            }
        }
        public SolidColorBrush SecondButtonColor 
        {
            get
            {
                return (SolidColorBrush)GetValue(SecondButtonColorProperty);
            }
            set
            {
                SetValue(SecondButtonColorProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(SecondButtonColorProperty,SecondButtonColor,value));
            }
        }

        public SolidColorBrush BorderColorBrush 
        {
            get
            {
                return (SolidColorBrush)GetValue(BorderColorBrushProperty);
            }
            set
            {
                SetValue(BorderColorBrushProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(BorderColorBrushProperty,BorderColorBrush,value));
            }
        }

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

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(TextProperty, Text, value));
            }
        }
        static CustomButton()
        {
            BorderColorBrushProperty = DependencyProperty.Register("BorderColorBrush", typeof(SolidColorBrush), typeof(CustomButton));
            FirstButtonColorProperty = DependencyProperty.Register("FirstButtonColor", typeof(SolidColorBrush), typeof(CustomButton));
            SecondButtonColorProperty = DependencyProperty.Register("SecondButtonColor", typeof(SolidColorBrush), typeof(CustomButton));
            DefoultImageSourceProperty = DependencyProperty.Register("DefoultImageSource", typeof(ImageSource), typeof(CustomButton));
            DisabledImageSourceProperty = DependencyProperty.Register("DisabledImageSource", typeof(ImageSource), typeof(CustomButton));
            DisabledStateColorProperty = DependencyProperty.Register("DisabledStateColor", typeof(SolidColorBrush), typeof(CustomButton));
            MouseHoverImageSourceProperty = DependencyProperty.Register("MouseHoverImageSource", typeof(ImageSource), typeof(CustomButton));
            ImageFlowDirectionProperty = DependencyProperty.Register("ImageFlowDirection", typeof(FlowDirection), typeof(CustomButton));
            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomButton));
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButton), new FrameworkPropertyMetadata(typeof(CustomButton)));
        }
        public CustomButton()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();
            IndexOfImageColumnSpan = (Text is not null && Text != String.Empty) ? 1 : 2;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            BorderColorBrush = FirstButtonColor?.CloneCurrentValue() ?? (SolidColorBrush)new BrushConverter().ConvertFrom("#ffaacc");
            BorderColorBrush.Opacity = 0;
        }

        private void Btn_MouseEnter(object sender, MouseEventArgs e)
        {
            var Animation = new ColorAnimation() { From = SecondButtonColor.Color, To = FirstButtonColor.Color, Duration = TimeSpan.FromSeconds(0.3), };
            var Animation2 = new DoubleAnimation() { From = 0, To = 1, Duration = TimeSpan.FromSeconds(0.3) };
            BorderColorBrush.BeginAnimation(SolidColorBrush.OpacityProperty, Animation2);
            BorderColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, Animation);
        }

        private void Btn_MouseLeave(object sender, MouseEventArgs e)
        {
            var Animation = new ColorAnimation() { From = FirstButtonColor.Color, To = SecondButtonColor.Color, Duration = TimeSpan.FromSeconds(0.3), };
            var Animation2 = new DoubleAnimation() { From = 1, To = 0, Duration = TimeSpan.FromSeconds(0.3) };
            BorderColorBrush.BeginAnimation(SolidColorBrush.OpacityProperty, Animation2);
        }
    }
}
