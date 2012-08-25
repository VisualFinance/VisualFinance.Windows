using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace VisualFinance.Windows.Sample.TreeListViewSamples
{
    /// <summary>
    /// Interaction logic for SmallDataSet.xaml
    /// </summary>
    public partial class SmallDataSet : Window
    {
        public SmallDataSet()
        {
            var evls = new EventLoopScheduler();
            var ds = new DispatcherScheduler(Dispatcher);
            var myStuff = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            //var folder = new FolderNode(@"C:\Users\Lee\Documents\\", evls, ds);
            var folder = new FolderNode(myStuff, evls, ds);
            InitializeComponent();
            DataContext = folder;
        }
    }
}
