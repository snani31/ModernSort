using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ModernSort.CustomeControls.CustomUIControls
{
    /// <summary>
    /// Логика взаимодействия для CustomButton.xaml
    /// </summary>
    public partial class CustomButton : Button
    {
        public ImageSource MouseHoverImageSource { get; set; }

        public ImageSource DefoultImageSource { get; set; }

        public ImageSource DisabledImageSource { get; set; }

        public HorizontalAlignment TextBlockHorizontalAlignment {  get; set; }

        public Thickness ImageMargin { get; set; }

        public CornerRadius CornerRadius { get; set; }

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

        public override void EndInit()
        {
            base.EndInit();
            IndexOfImageColumnSpan = (Text is not null && Text != String.Empty) ? 1 : 2;
        }

        static CustomButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomButton), new FrameworkPropertyMetadata(typeof(CustomButton)));
        }

    }
}
