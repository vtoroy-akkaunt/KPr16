using KPr16;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace KPr16 {
    public class EventIgnore: Event {}
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
                if (protect_once > 0)
                {
                    if (ee.ignores_armor)
                    {
                        Game.event_log.Add($"{ee.src.name} игнорирует защиту игрока!");
                        return;
                    }
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
    public class EventItemUse : Event { }
    public class EventHealing : Event {
        public int hp;
    }
    public class EventFreeze : Event { }
    public class ItemCGenericHealing : EntityNamed {
        private int hp;
        public ItemCGenericHealing(int hp) {
            base.name = "Аптэчка";
            this.hp = hp;
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
        public bool ignores_armor = false;
    }
    public class ItemCGenericWeapon : EntityNamed {
        protected int hp;
        public ItemCGenericWeapon(int hp) {
            base.name = "Меч";
            this.hp = hp;
        }
        public override string description => $"Наносит {hp} урона";
        public override bool usable => true;
        public override bool usable_against => true;
        public override bool is_weapon => true;
        public override void proccess_event(ref Event e) {
            if (e is EventItemUse) {
                e = new EventDamage();
                (e as EventDamage).hp = hp;
            }
        }
    }
    public class ItemCCritWeapon: ItemCGenericWeapon
    {
        private double crit_chance;
        public ItemCCritWeapon(int hp, double crit_chance): base(hp)
        {
            this.crit_chance = crit_chance;
            base.name = "Магический критический меч";
        }
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                var ee = new EventDamage { hp = base.hp };
                if (NW.random.NextDouble() < crit_chance)
                {
                    Game.event_log.Add($"{name} критует!");
                    ee.hp *= 2;
                }
                e = ee;
            }
        }
    }
    public class ItemCIgnoresArmor : ItemCGenericWeapon
    {
        private double chance;
        public ItemCIgnoresArmor(int hp, double chance) : base(hp)
        {
            this.chance = chance;
            base.name = "Игнорирующий меч";
        }
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                var ee = new EventDamage { hp = base.hp };
                if (NW.random.NextDouble() < chance)
                {
                    ee.ignores_armor = true;
                }
                e = ee;
            }
        }
    }
    public class EventOnceProtection: Event
    {
        public int hp;
    }
    public class ItemCGenericShield : EntityNamed
    {
        private int hp;
        public ItemCGenericShield(int hp)
        {
            base.name = "Щит";
            this.hp = hp;
        }
        public override string description => $"Защищает от {hp} урона";
        public override bool usable => true;
        public override bool is_armor => true;
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                e = new EventOnceProtection { hp = hp };
            }
        }
    }
    class NW {
        public static Random random = new();
        public static EntityNamed random_weapon() {
            return new ItemCGenericWeapon(NW.random.Next(10, 20));
        }
        public static T choice<T>(List<T> list) {
            return list[NW.random.Next(0, list.Count)];
        }
        public static Entity random_enemy(bool is_boss = false) {
            if (is_boss)
            {
                return choice(new List<Entity> { new EntityHasAI() { name = "The враг", hp = NW.random.Next(150, 200) } });
            }
            else
            {
                return choice(new List<Entity> {
                    new EnemyCGoblin(),
                    new EnemyCSkeleton()
                });
            }
        }
        public static EntityNamed random_item() {
            return choice(new List<EntityNamed> {
                new ItemCGenericHealing(NW.random.Next(25, 50)),
                new ItemCGenericShield (NW.random.Next(10, 50)),
                random_weapon(),
            });
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
        public EntityNamed weapon = NW.random_weapon();
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
    public class EnemyCGoblin: EntityHasAI {
        public EnemyCGoblin()
        {
            base.name = "Гоблин";
            base.hp = 30;
            base.weapon = new ItemCCritWeapon(NW.random.Next(30, 50), 0.2);
        }
    }
    public class EnemyCSkeleton : EntityHasAI
    {
        public EnemyCSkeleton()
        {
            base.name = "Скелет";
            base.hp = 40;
            base.weapon = new ItemCIgnoresArmor(NW.random.Next(30, 50), 1.0);
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
                front.Add(NW.random_item());
            } else {
                if (turn_nr % 10 == 0)
                {
                    player.enemies.Add(NW.random_enemy(true));
                }
                else
                {
                    for (int i = 0; i < NW.random.Next(1, 3); i++)
                    {
                        player.enemies.Add(NW.random_enemy(false));
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
