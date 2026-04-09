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
        public int hp { get; set { if (value < 0) field = 0; } }
        public bool is_alive { get { return hp > 0; } }
        public bool is_frozen = false;
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
            else if (e is EventHealing e2) {
                Game.event_log.Add($"{this.name} восстанавливает {e2.hp} здоровья!");
            } else if (e is EventFreeze e3) {
                Game.event_log.Add($"{this.name} заморожен! на один ход");
                is_frozen = true;
            }

        }
        public virtual void ai() {}
    }
    public class EventItemUse : Event { }
    public class EventHealing : Event {
        public int hp;
    }
    public class EventFreeze : Event { }
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
    public class ItemCGenericWeapon : EntityNamed {
        private int hp;
        public ItemCGenericWeapon(int hp) {
            base.name = "Меч";
            this.hp = hp;
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
            return new ItemCGenericWeapon(new Random().Next(10, 20));
        }
        public static T choice<T>(List<T> list) {
            return list[new Random().Next(0, list.Count)];
        }
        public static EntityLiving random_enemy(bool is_boss = false) {
            if (is_boss) {
                return choice(new List<EntityLiving> { new EntityHasAI() { name = "The ___", hp = 137 } });
            } else
                return choice(new List<EntityLiving> { new EntityHasAI() { name = "___", hp = 137 }, new EnemyGoblin() });
        }
        public static EntityNamed random_item() {
            return new ItemCHealing();
        }
        public static EntityLiving /* null */ select_random_enemy(List<EntityLiving> enemies) {
            if (!enemies.Where(e => e.is_alive).Any())
                return null;
            var ret = enemies.First();
            do {
                ret = enemies[new Random().Next(0, enemies.Count)];
            } while (!ret.is_alive);
            return ret;
        }
    }
    public class Entity: EntityLiving
    {
        public EntityNamed weapon = NW.random_weapon();
        public EntityNamed armor; // maybe null
        public List<EntityLiving> enemies;
        public List<EntityNamed> items { get {
            ...
        } }
        public void item_use_helper(EntityNamed item, Event ee) {
            if (is_frozen) {
                Game.event_log.Add($"{name} заморожен и не может воспользоваться {item.name}");
            }
            if (!item.usable)
                return;
            if (item.usable_against && NW.select_random_enemy(enemies) == null)
                return;
            Game.event_log.Add($"{name} использует {item.name}");
            EntityLiving dst = this;
            if (item.usable_against) {
                dst = NW.select_random_enemy(enemies);
                Game.event_log[Game.event_log.Count - 1] += $" на {dst}";
            }
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
            item_use_helper((new Random().NextDouble() > 0.75 && armor != null ? armor : weapon), ee);
        }
    }
    public class EnemyGoblin: EntityHasAI {
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
        public enum PlayerAction {
            Use, Skip, Take
        }
        public List<PlayerAction> available_actions() {
            if (is_item_now) {
                return new List<PlayerAction> { PlayerAction.Take, PlayerAction.Skip };
            } else {
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
                player.item_use_helper(item, ee);
            } else if (action == PlayerAction.Take && is_item_now) {

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
            front.Clear();
            is_item_now = new Random().NextDouble() > 0.67;
            if (turn_nr % 10 == 0) {
                front.Add(NW.random_enemy(true));
            }
            else if (is_item_now)
            {
                front.Add(NW.random_item());
            } else {
                for (int i = 0; i < new Random().Next(1, 3); i++)
                    front.Add(NW.random_enemy(false));
            }
        }
    }
}
