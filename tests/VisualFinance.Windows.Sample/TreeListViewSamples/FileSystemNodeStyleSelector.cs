using System;
using System.Windows;
using System.Windows.Controls;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    public class FileSystemNodeStyleSelector : StyleSelector
    {
        public Style FolderStyle { get; set; }
        public Style FileStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is FolderNode) return FolderStyle;
            if (item is FileNode) return FileStyle;
            throw new InvalidOperationException();
        }
    }
}