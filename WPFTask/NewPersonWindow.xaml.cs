using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFTask
{
    /// <summary>
    /// Логика взаимодействия для NewPersonWindow.xaml
    /// </summary>
    public partial class NewPersonWindow : Window
    {
        public Person Person { get; private set; }
        public NewPersonWindow( Person person )
        {
            InitializeComponent();
            Person = person;
            DataContext = Person;
        }
        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
