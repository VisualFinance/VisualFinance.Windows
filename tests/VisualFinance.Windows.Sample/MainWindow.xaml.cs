using System.Windows;

namespace VisualFinance.Windows.Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += Button_Click_1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var w = new TreeListViewSamples.SmallDataSet();
            w.Show();
            Close();
        }
    }
}
