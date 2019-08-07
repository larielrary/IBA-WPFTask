using System.Windows;

namespace WPFTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new ApplicationViewModel(this);
        }
        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Export exportWindow = new Export(this);
            exportWindow.ShowDialog();
        }
    }
}
