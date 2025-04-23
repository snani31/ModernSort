using System.Windows;

namespace ModernSort.Services.Dialog
{
    internal interface IDialog
    {
        object DataContext { get; set; }
        bool? DialogResult { get; set; }
        public System.Windows.WindowState WindowState { get; set; }
        Window Owner { get; set; }
        void Close();
        bool? ShowDialog();
    }
}
