using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPr16
{
    public class ItemCGenericHealing : EntityNamed
    {
        private int hp;
        public ItemCGenericHealing(int hp)
        {
            base.name = "Аптэчка";
            this.hp = hp;
        }
        public override string description => "Восстанавливает " + Convert.ToString(hp) + " HP";
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse ee)
            {
                var e2 = new EventHealing();
                e2.src = e.src;
                e2.dst = e.dst;
                (e as EventHealing).hp = hp;
            }
        }
        public override bool usable => true;
    }
    public class ItemCGenericWeapon : EntityNamed
    {
        protected int hp;
        public ItemCGenericWeapon(int hp)
        {
            base.name = "Меч";
            this.hp = hp;
        }
        public override string description => $"Наносит {hp} урона";
        public override bool usable => true;
        public override bool usable_against => true;
        public override bool is_weapon => true;
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                var e2 = new EventDamage();
                e2.src = e.src;
                e2.dst = e.dst;
                (e2 as EventDamage).hp = hp;
                e = e2;
            }
        }
    }
    public class ItemCCritWeapon : ItemCGenericWeapon
    {
        private double crit_chance;
        public ItemCCritWeapon(int hp, double crit_chance) : base(hp)
        {
            this.crit_chance = crit_chance;
            base.name = "Магический критический меч";
        }
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                var e2 = new EventDamage { hp = base.hp };
                e2.src = e.src;
                e2.dst = e.dst;
                if (NW.random.NextDouble() < crit_chance)
                {
                    Game.event_log.Add($"{name} критует!");
                    e2.hp *= 2;
                }
                e = e2;
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
                var e2 = new EventDamage { hp = base.hp };
                e2.src = e.src;
                e2.dst = e.dst;
                if (NW.random.NextDouble() < chance)
                {
                    e2.ignores_armor = true;
                }
                e = e2;
            }
        }
    }
    public class ItemCFreezes : ItemCGenericWeapon
    {
        private double chance;
        public ItemCFreezes(int hp, double chance) : base(hp)
        {
            this.chance = chance;
            base.name = "Замораживающий меч";
        }
        public override void proccess_event(ref Event e)
        {
            if (e is EventItemUse)
            {
                var e2 = new EventDamage { hp = base.hp };
                e2.src = e.src;
                e2.dst = e.dst;
                if (NW.random.NextDouble() < chance)
                {
                    Event e3 = new EventFreeze { src = e.src, dst = e.dst };
                    e2.dst.proccess_event(ref e3);
                }
                e = e2;
            }
        }
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
    partial class Repo
    {
        public static EntityNamed random_weapon()
        {
            return new ItemCGenericWeapon(NW.random.Next(10, 20));
        }
        public static EntityNamed random_item()
        {
            return NW.choice(new List<EntityNamed> {
                new ItemCGenericHealing(NW.random.Next(25, 50)),
                new ItemCGenericShield (NW.random.Next(10, 50)),
                random_weapon(),
            });
        }
    }
}
