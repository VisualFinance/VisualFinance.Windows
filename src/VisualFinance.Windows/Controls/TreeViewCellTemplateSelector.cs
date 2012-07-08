using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace VisualFinance.Windows.Controls
{
    public class TreeViewCellTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ParentTemplate { get; set; }
        public DataTemplate ChildTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var isParent = (item as IEnumerable) != null;
            return isParent ? ParentTemplate : ChildTemplate;
        }
    }
}