using System.Windows;
using System.Windows.Controls;

namespace ModernSort.CustomeControls.CustomUIControls
{
    /// <summary>
    /// Логика взаимодействия для CustomTooltip.xaml
    /// </summary>
    public partial class CustomTooltip : ToolTip
    {

        public static readonly DependencyProperty HeaderTextProperty;
        public static readonly DependencyProperty ContentTextProperty;
        public static readonly DependencyProperty TemplateTypeProperty;

        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(HeaderTextProperty, HeaderText, value));
            }
        }


        public string ContentText
        {
            get
            {
                return (string)GetValue(ContentTextProperty);
            }
            set
            {
                SetValue(ContentTextProperty, value);
                OnPropertyChanged(new DependencyPropertyChangedEventArgs(ContentTextProperty, ContentText, value));
            }
        }


        static CustomTooltip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomTooltip), new FrameworkPropertyMetadata(typeof(CustomTooltip)));
            HeaderTextProperty = DependencyProperty.Register("HeaderText", typeof(string), typeof(CustomTooltip));
            ContentTextProperty = DependencyProperty.Register("ContentText", typeof(string), typeof(CustomTooltip));
        }

        public CustomTooltip()
        {
            InitializeComponent();
        }

        public override void EndInit()
        {
            base.EndInit();
            if (!String.IsNullOrEmpty(HeaderText))
            {
                var selectedTemplate = (ControlTemplate)this.FindResource("WithHedderTooltipTemplate");
                this.Template = selectedTemplate;
            }
            else
            {
                var selectedTemplate = (ControlTemplate)this.FindResource("NoHedderTooltipTemplate");
                this.Template = selectedTemplate;
            }
        }
    }
}
