using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ModernSort.CustomeControls
{
    public class MenuRadiobutton : RadioButton
    {

        static MenuRadiobutton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MenuRadiobutton), new FrameworkPropertyMetadata(typeof(MenuRadiobutton)));
        }
    }
}
