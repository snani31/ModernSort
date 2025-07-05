using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ModernSort.CustomeControls.CustomUIControls
{
    /// <summary>
    /// Логика взаимодействия для CustomRadioButton.xaml
    /// </summary>
    public partial class CustomRadioButton : RadioButton, INotifyPropertyChanged
    {

        public enum CustomRadioButtonDirrectionType : byte
        {
            HorizontalDownDirrection = 0,
            VerticalLeftDirrection = 1
        }

        public static readonly DependencyProperty BorderColorBrushProperty;
        public static readonly DependencyProperty MouseHoverImageSourceProperty;
        public static readonly DependencyProperty DefoultImageSourceProperty;
        public static readonly DependencyProperty DisabledImageSourceProperty;
        public static readonly DependencyProperty SelectedImageSourceProperty;

        public static readonly DependencyProperty DefoultColorProperty;
        public static readonly DependencyProperty MouseEnterColorProperty;
        public static readonly DependencyProperty DisabledColorProperty;
        public static readonly DependencyProperty SelectedColorProperty;

        public static readonly DependencyProperty DefoultTextColorProperty;
        public static readonly DependencyProperty SelectedTextColorProperty;

        public static readonly DependencyProperty IndicatorLineColorProperty;

        public static readonly DependencyProperty DirrectionTypeProperty;

        public static readonly DependencyProperty TextProperty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CustomRadioButtonDirrectionType? DirrectionType 
        {
            get
            {
                return (CustomRadioButtonDirrectionType)GetValue(DirrectionTypeProperty);
            }
            set
            {
                SetValue(DirrectionTypeProperty, value);
                OnPropertyChenged(nameof(DirrectionType));
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
                OnPropertyChenged(nameof(MouseHoverImageSource));
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
                OnPropertyChenged(nameof(DefoultImageSource));
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
                OnPropertyChenged(nameof(DisabledImageSource));
            }
        }

        public ImageSource SelectedImageSource
        {
            get
            {
                return (ImageSource)GetValue(SelectedImageSourceProperty);
            }
            set
            {
                SetValue(SelectedImageSourceProperty, value);
                OnPropertyChenged(nameof(SelectedImageSource));
            }
        }

        public SolidColorBrush DefoultColor
        {
            get
            {
                return (SolidColorBrush)GetValue(DefoultColorProperty);
            }
            set
            {
                SetValue(DefoultColorProperty, value);
                OnPropertyChenged(nameof(DefoultColor));
            }
        }
        public SolidColorBrush MouseEnterColor
        {
            get
            {
                return (SolidColorBrush)GetValue(MouseEnterColorProperty);
            }
            set
            {
                SetValue(MouseEnterColorProperty, value);
                OnPropertyChenged(nameof(MouseEnterColor));
            }
        }

        public SolidColorBrush DisabledColor
        {
            get
            {
                return (SolidColorBrush)GetValue(DisabledColorProperty);
            }
            set
            {
                SetValue(DisabledColorProperty, value);
                OnPropertyChenged(nameof(DisabledColor));
            }
        }

        public SolidColorBrush SelectedColor
        {
            get
            {
                return (SolidColorBrush)GetValue(SelectedColorProperty);
            }
            set
            {
                SetValue(SelectedColorProperty, value);
                OnPropertyChenged(nameof(SelectedColor));
            }
        }

        public SolidColorBrush DefoultTextColor
        {
            get
            {
                return (SolidColorBrush)GetValue(DefoultTextColorProperty);
            }
            set
            {
                SetValue(DefoultTextColorProperty, value);
                OnPropertyChenged(nameof(DefoultTextColor));
            }
        }

        public SolidColorBrush SelectedTextColor
        {
            get
            {
                return (SolidColorBrush)GetValue(SelectedTextColorProperty);
            }
            set
            {
                SetValue(SelectedTextColorProperty, value);
                OnPropertyChenged(nameof(SelectedTextColor));
            }
        }

        public SolidColorBrush IndicatorLineColor
        {
            get
            {
                return (SolidColorBrush)GetValue(IndicatorLineColorProperty);
            }
            set
            {
                SetValue(IndicatorLineColorProperty, value);
                OnPropertyChenged(nameof(IndicatorLineColor));
            }
        }

        public SolidColorBrush BorderColorBrush
        {
            get
            {
                return (SolidColorBrush)GetValue(BorderColorBrushProperty);
            }
            private set
            {
                SetValue(BorderColorBrushProperty, value);
                OnPropertyChenged(nameof(BorderColorBrush));
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
                OnPropertyChenged(nameof(Text));
            }
        }

        static CustomRadioButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomRadioButton), new FrameworkPropertyMetadata(typeof(CustomRadioButton)));
            MouseHoverImageSourceProperty = DependencyProperty.Register("MouseHoverImageSource", typeof(ImageSource), typeof(CustomRadioButton));
            DefoultImageSourceProperty = DependencyProperty.Register("DefoultImageSource", typeof(ImageSource), typeof(CustomRadioButton));
            DisabledImageSourceProperty = DependencyProperty.Register("DisabledImageSource", typeof(ImageSource), typeof(CustomRadioButton));
            SelectedImageSourceProperty = DependencyProperty.Register("SelectedImageSource", typeof(ImageSource), typeof(CustomRadioButton));

            DefoultColorProperty = DependencyProperty.Register("DefoultColor", typeof(SolidColorBrush), typeof(CustomRadioButton));
            MouseEnterColorProperty = DependencyProperty.Register("MouseEnterColor", typeof(SolidColorBrush), typeof(CustomRadioButton));
            DisabledColorProperty = DependencyProperty.Register("DisabledColor", typeof(SolidColorBrush), typeof(CustomRadioButton));
            SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(SolidColorBrush), typeof(CustomRadioButton));

            DefoultTextColorProperty = DependencyProperty.Register("DefoultTextColor", typeof(SolidColorBrush), typeof(CustomRadioButton));
            SelectedTextColorProperty = DependencyProperty.Register("SelectedTextColor", typeof(SolidColorBrush), typeof(CustomRadioButton));

            SelectedTextColorProperty = DependencyProperty.Register("IndicatorLineColor", typeof(SolidColorBrush), typeof(CustomRadioButton));
            BorderColorBrushProperty = DependencyProperty.Register("BorderColorBrush", typeof(SolidColorBrush), typeof(CustomRadioButton));

            DirrectionTypeProperty = DependencyProperty.Register("DirrectionType", typeof(CustomRadioButtonDirrectionType), typeof(CustomRadioButton));

            TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(CustomRadioButton));

        }

        public CustomRadioButton()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();
            if (DirrectionType == CustomRadioButtonDirrectionType.HorizontalDownDirrection)
            {
                var selectedTemplate = (ControlTemplate)this.FindResource("HorizonatalOrientationRadioButtonTemplate");
                this.Template = selectedTemplate;

            }
            else if (DirrectionType == CustomRadioButtonDirrectionType.VerticalLeftDirrection)
            {
                var a = (ControlTemplate)this.FindResource("VerticalOrientationRadioButtonTemplate");
                this.Template = a;
            }
        }

        protected void OnPropertyChenged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override void OnApplyTemplate()
        {

            if (DirrectionType == CustomRadioButtonDirrectionType.HorizontalDownDirrection)
            {
                var mouseEnterColorAnimation = new ColorAnimation() { From = DefoultColor.Color, To = MouseEnterColor.Color, Duration = TimeSpan.FromSeconds(0.3), };

                var indicatorExpandAnimation = new DoubleAnimation() { From = 0, To = 5, Duration = TimeSpan.FromSeconds(0.3), };

                var mouseLeaveColorAnimation = new ColorAnimation() { From = MouseEnterColor.Color, To = DefoultColor.Color, Duration = TimeSpan.FromSeconds(0.3), };

                var indicatorCollapseAnimation = new DoubleAnimation() { From = 5, To = 0, Duration = TimeSpan.FromSeconds(0.3), };

                this.MouseEnter += MouseEnterEventClosure(mouseEnterColorAnimation, indicatorExpandAnimation, HeightProperty);

                this.MouseLeave += MouseEnterEventClosure(mouseLeaveColorAnimation, indicatorCollapseAnimation, HeightProperty);
            }
            else if (DirrectionType == CustomRadioButtonDirrectionType.VerticalLeftDirrection)
            {
                var mouseEnterColorAnimation = new ColorAnimation() { From = DefoultColor.Color, To = MouseEnterColor.Color, Duration = TimeSpan.FromSeconds(0.3), };

                var indicatorExpandAnimation = new DoubleAnimation() { From = 0, To = 5, Duration = TimeSpan.FromSeconds(0.3), };

                var mouseLeaveColorAnimation = new ColorAnimation() { From = MouseEnterColor.Color, To = DefoultColor.Color, Duration = TimeSpan.FromSeconds(0.3), };

                var indicatorCollapseAnimation = new DoubleAnimation() { From = 5, To = 0, Duration = TimeSpan.FromSeconds(0.3), };

                this.MouseEnter += MouseEnterEventClosure(mouseEnterColorAnimation, indicatorExpandAnimation, WidthProperty);

                this.MouseLeave += MouseEnterEventClosure(mouseLeaveColorAnimation, indicatorCollapseAnimation, WidthProperty);
            }

            base.OnApplyTemplate();
            BorderColorBrush = DefoultColor?.CloneCurrentValue() ?? (SolidColorBrush)new BrushConverter().ConvertFrom("#ffaacc");
            OnPropertyChenged(nameof(BorderColorBrush));

        }

        private MouseEventHandler MouseEnterEventClosure(ColorAnimation colorAnimation,DoubleAnimation doubleAnimation,
            DependencyProperty dependencyProperty)
        {

            Border indicatorBorder = (Border)this.Template.FindName("indicator", this);


            return (object sender, MouseEventArgs e) =>
            {
                BorderColorBrush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnimation);
                indicatorBorder.BeginAnimation(dependencyProperty, doubleAnimation);
            };
        }
    }
}
