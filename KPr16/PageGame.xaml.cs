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
    /// Логика взаимодействия для PageGame.xaml
    /// </summary>
    public partial class PageGame : Page
    {
        Game game;
        public PageGame()
        {
            InitializeComponent();
        }
        private void redraw()
        {
            c_front.ItemsSource = game.front.Union(new List<EntityNamed> { game.player }).ToList();
        }

        private void c_items_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (!game.available_actions().Contains(Game.PlayerAction.Use))
                return;
            if (c_items.SelectedItem != null) {
                var item = (c_items.SelectedItem as EntityNamed);
                game.process_player_action(Game.PlayerAction.Use, item);
            }
        }
    }
}
