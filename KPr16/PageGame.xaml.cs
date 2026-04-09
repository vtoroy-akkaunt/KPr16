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
        Game game = new();
        public PageGame()
        {
            InitializeComponent();
            redraw();
        }
        private void redraw()
        {
            c_front.ItemsSource = game.front.Union(new List<EntityNamed> { game.player }).ToList();
            var aa = game.available_actions();
            btn_take.IsEnabled = aa.Contains(Game.PlayerAction.Take);
            btn_skip.IsEnabled = aa.Contains(Game.PlayerAction.Skip);
        }

        private void c_items_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (!game.available_actions().Contains(Game.PlayerAction.Use))
                return;
            if (c_items.SelectedItem != null) {
                var item = (c_items.SelectedItem as EntityNamed);
                game.process_player_action(Game.PlayerAction.Use, item);
            }
            redraw();
        }

        private void btn_skip_Click(object sender, RoutedEventArgs e) {
            game.process_player_action(Game.PlayerAction.Skip, null);
            redraw();
        }

        private void btn_take_Click(object sender, RoutedEventArgs e) {
            game.process_player_action(Game.PlayerAction.Take, null);
            redraw();
        }
    }
}
