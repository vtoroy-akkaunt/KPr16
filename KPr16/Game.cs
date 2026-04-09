using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KPr16;

namespace KPr16 {
    public class EntityLiving : EntityNamed {
        public int hp { get; set { if (value < 0) field = 0; } }
        public bool is_alive { get { return hp > 0; } }
        public override string description => Convert.ToString(hp) + " хэпэ";
        public override void proccess_event(ref Event e) {
            if (!is_alive) {
                Game.event_log.Add($"{this.name} же мёртв, что ему будет");
                return;
            }
            if (e is EventDamage ee) {
                Game.event_log.Add($"{this.name} получает {ee.hp} урона!");
                if (!is_alive)
                    Game.event_log.Add($"{this.name} помер!");
            }
        }
    }
    public class EventItemUse : Event { }
    public class EventHealing : Event {
        public int hp;
    }
    public class ItemCHealing : EntityNamed {
        private int hp = 50;
        public ItemCHealing() {
            base.name = "Аптэчка";
        }
        public override string description => "Восстанавливает " + Convert.ToString(hp) + " HP";
        public override void proccess_event(ref Event e) {
            if (e is EventItemUse ee) {
                e = new EventHealing();
                (e as EventHealing).hp = hp;
            }
        }
        public override bool usable => true;
    }
    public class EventDamage : Event {
        public int hp;
    }
    public class ItemCWeapon : EntityNamed {
        private int hp = 66;
        public ItemCWeapon() {
            base.name = "Меч им. Альшаковой";
        }
        public override string description => $"Наносит {hp} урона";
        public override bool usable => true;
        public override bool usable_against => true;
        public override void proccess_event(ref Event e) {
            if (e is EventItemUse ee) {
                e = new EventDamage();
                (e as EventDamage).hp = hp;
            }
        }
    }
    class NW {
        public static EntityNamed random_weapon() {
            return new ItemCWeapon();
        }
        public static EntityLiving create_enemy(bool is_boss = false) {
            if (is_boss)
                return new EntityLiving() { name = "The ___", hp = 69 };
            else
                return new EntityLiving() { name = "___", hp = 37 };
        }
        public static EntityNamed create_item() {
            return new ItemCHealing();
        }
    }
    public class Entity: EntityLiving
    {
        public EntityNamed weapon;
        public EntityNamed armor; // maybe null
        public List<EntityNamed> enemies;
        public void item_use_helper(EntityNamed item, Event ee) {
            if (!item.usable)
                return;
            Game.event_log.Add($"{name.ToUpper()} использует {item.name.ToUpper()}");
            EntityNamed dst = this;
            if (item.usable_against) {
                dst = enemies[new Random().Next(0, enemies.Count)];
                Game.event_log[Game.event_log.Count -1 ] += $" на {dst}";
            }
            item.proccess_event(ref ee);
            dst.proccess_event(ref ee);
        }
    }
    public class EnemyGoblin: Entity
    {
        public EnemyGoblin()
        {
            base.name = "Гоблин";
            base.hp = 88;
            base.weapon = NW.random_weapon();
        }
    }
    
    internal class Game
    {
        public static List<string> event_log = new List<string>();
        public List<EntityNamed> front = new List<EntityNamed>();
        public Entity player;
        bool is_item_now = true;
        int turn_nr = 0;
        public Game()
        {
            player = new Entity();
            player.name = "Игрок";
            player.hp = 1337;
        }
        public enum PlayerAction
        {
            Skip, Use
        }
        public void proccess_player_action(PlayerAction action)
        {

        }
        private void next_stage()
        {
            turn_nr++;
            front.Clear();
            if (turn_nr % 10 == 0) {
                front.Add(NW.create_enemy(true));
            }
            else if (is_item_now)
            {
                front.Add(NW.create_item());
            } else {
                front.Add(NW.create_enemy(false));
            }
        }
    }
}
