using ModernSort.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModernSort.CustomeControls.TemplateSelectors
{
    public class EditMediaObjectFunctionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FilesTemplate { get; set; }
        public DataTemplate FiltersTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {

            if(item is FunktionPageEnum presenterType)
            {
                if (presenterType == FunktionPageEnum.FilesPresent)
                {
                    return FilesTemplate;
                }
                else if (presenterType == FunktionPageEnum.FiltersPresent)
                {
                    return FiltersTemplate;
                }
            }

            return null;
        }
    }
}
