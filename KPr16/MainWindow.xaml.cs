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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KPr16
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow instance;
        public MainWindow()
        {
            InitializeComponent();
            instance = this;
        }
        public static void new_game()
        {
            instance.the_frame.Navigate(new PageGame());
        }
        public static void lose()
        {
            instance.the_frame.Navigate(new PageLose());
        }
    }
}
