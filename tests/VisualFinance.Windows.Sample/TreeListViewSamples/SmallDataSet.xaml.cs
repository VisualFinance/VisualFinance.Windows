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
            var folder = new Folder(@"C:\Users\Lee\Documents\GitHub\VisualFinance.Windows\", evls, ds);
            InitializeComponent();
            DataContext = folder;
        }
    }
}
