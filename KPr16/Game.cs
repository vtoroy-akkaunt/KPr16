using KPr16;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KPr16 {
    public class EntityLiving : EntityNamed {
        public int hp { get; set { if (value < 0) field = 0; else field = value; } }
        public bool is_alive { get { return hp > 0; } }
        public bool is_frozen = false;
        private int protect_once = 0;
        public override string description => Convert.ToString(hp) + " хэпэ" + (is_alive ? "" : " (мёртв)");
        public override void proccess_event(ref Event e) {
            if (!is_alive) {
                Game.event_log.Add($"{this.name} же мёртв, что ему будет");
                return;
            }
            if (e is EventItemUse) {
                if (is_frozen)
                {
                    Game.event_log.Add($"{name} заморожен и не может воспользоваться предметом");
                    is_frozen = false;
                    Game.event_log.Add($"{name} разморожен");
                    e = new EventIgnore();
                }
            }
            else if (e is EventDamage ee) {
                if (protect_once > 0 && ee.ignores_armor)
                {
                    Game.event_log.Add($"{ee.src.name} игнорирует защиту игрока!");
                    protect_once = 0;
                }
                if (protect_once > 0)
                {
                    Game.event_log.Add($"{this.name} поглотил {protect_once} урона!");
                    ee.hp = Math.Max(0, Math.Min(this.hp, ee.hp - protect_once));
                    protect_once = 0;
                }
                Game.event_log.Add($"{this.name} получает {ee.hp} урона!");
                this.hp -= ee.hp;
                if (!is_alive)
                    Game.event_log.Add($"{this.name} помер!");
            }
            else if (e is EventHealing e2) {
                Game.event_log.Add($"{this.name} восстанавливает {e2.hp} здоровья!");
                this.hp += e2.hp;
            } else if (e is EventFreeze e3) {
                Game.event_log.Add($"{this.name} заморожен! на один ход");
                is_frozen = true;
            } else if (e is EventOnceProtection e4) {
                protect_once = e4.hp;
            }
        }
        public virtual void ai() {}
    }
    class NW {
        public static Random random = new();
        
        public static T choice<T>(List<T> list) {
            return list[NW.random.Next(0, list.Count)];
        }
        public static EntityLiving /* null */ select_random_enemy_of(List<EntityLiving> enemies) {
            if (!enemies.Where(e => e.is_alive).Any())
                return null;
            var ret = enemies.First();
            do {
                ret = enemies[NW.random.Next(0, enemies.Count)];
            } while (!ret.is_alive);
            return ret;
        }
    }
    public class Entity: EntityLiving
    {
        public EntityNamed weapon = Repo.random_weapon();
        public EntityNamed armor; // maybe null
        public List<EntityLiving> enemies = new(); // Чепуха
        public List<EntityNamed> items { get {
            var ret = new List<EntityNamed> { weapon };
            if (armor != null)
                ret.Add(armor);
            return ret;
        } }
        public void obtain_item(EntityNamed item)
        {
            if (item.is_weapon)
            {
                weapon = item;
                return;
            }
            if (item.is_armor)
            {
                armor = item;
                return;
            }
            if (!item.usable)
                return;
            Game.event_log.Add($"{name} использует {item.name}");
            Event ee = new EventItemUse();
            ee.src = this;
            ee.dst = this;
            this.proccess_event(ref ee);
            item.proccess_event(ref ee);
            this.proccess_event(ref ee);
        }
        public void use_item(EntityNamed item, Event ee) {
            if (!is_alive)
                return;
            /*if (is_frozen) {
                Game.event_log.Add($"{name} заморожен и не может воспользоваться {item.name}");
                return;
            }*/
            if (item.usable_against && NW.select_random_enemy_of(enemies) == null)
                return;
            Game.event_log.Add($"{name} использует {item.name}");
            EntityLiving dst = this;
            if (item.usable_against) {
                dst = NW.select_random_enemy_of(enemies);
                Game.event_log[Game.event_log.Count - 1] += $" на {dst.name}";
            }
            this.proccess_event(ref ee);
            item.proccess_event(ref ee);
            dst .proccess_event(ref ee);
        }
        public override string description => base.description + $", оружие: {weapon.name}, {weapon.description}, броня: {(armor != null ? armor.name + ", " + armor.description : "-")}";
    }
    public class EntityHasAI : Entity {
        public override void ai() {
            EventItemUse ee = new() {
                src = this,
                dst = null
            };
            use_item((NW.random.NextDouble() > 0.75 && armor != null ? armor : weapon), ee);
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
        public enum PlayerAction {
            Use, Skip, Take
        }
        public List<PlayerAction> available_actions() {
            if (is_item_now) {
                return new List<PlayerAction> { PlayerAction.Take, PlayerAction.Skip };
            } else {
                if (player.is_frozen)
                    return new List<PlayerAction> { PlayerAction.Skip };
                else
                    return new List<PlayerAction> { PlayerAction.Use, PlayerAction.Skip };
            }
        }
        private void enemies_turn() {
            foreach (var enemy in front) {
                if (enemy is EntityLiving wai)
                    wai.ai();
            }
        }
        public void process_player_action(PlayerAction action, EntityNamed item /* maybe null */) {
            if (action == PlayerAction.Use) {
                var ee = new EventItemUse();
                ee.src = player;
                ee.dst = null;
                player.use_item(item, ee);
            } else if (action == PlayerAction.Take && is_item_now) {
                player.obtain_item(front.First());
                front.Remove(front.First());
            }
            if (!is_item_now) {
                enemies_turn();
            }
            if (is_item_now || front.Where(e => (e as EntityLiving).is_alive).Count() == 0)
                next_stage();
        }
        private void next_stage()
        {
            turn_nr++;
            event_log.Add($"Ход {turn_nr}");
            front.Clear();
            player.enemies.Clear();
            is_item_now = NW.random.NextDouble() > 0.33;
            if (is_item_now)
            {
                front.Add(Repo.random_item());
            } else {
                if (turn_nr % 10 == 0)
                {
                    player.enemies.Add(Repo.random_enemy(true));
                }
                else
                {
                    for (int i = 0; i < NW.random.Next(1, 3); i++)
                    {
                        player.enemies.Add(Repo.random_enemy(false));
                    }
                }
                foreach (var enemy in player.enemies)
                {
                    (enemy as Entity).enemies.Add(player);
                    front.Add(enemy);
                }
            }
        }
    }
}
