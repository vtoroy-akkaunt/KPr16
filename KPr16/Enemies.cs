using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPr16
{
    public class EnemyCGoblin : EntityHasAI
    {
        public EnemyCGoblin()
        {
            base.name = "Гоблин";
            base.hp = 30;
            base.weapon = new ItemCCritWeapon(NW.random.Next(10, 15), 0.2);
            base.armor = new ItemCGenericShield(3);
        }
    }
    public class EnemyCSkeleton : EntityHasAI
    {
        public EnemyCSkeleton()
        {
            base.name = "Скелет";
            base.hp = 40;
            base.weapon = new ItemCIgnoresArmor(NW.random.Next(15, 25), 1.0);
            base.armor = new ItemCGenericShield(5);
        }
    }
    public class EnemyCMage : EntityHasAI
    {
        public EnemyCMage()
        {
            base.name = "Маг";
            base.hp = 25;
            base.weapon = new ItemCFreezes(NW.random.Next(20, 30), 0.15);
            base.armor = new ItemCGenericShield(2);
        }
    }
    partial class Repo
    {
        public static Entity random_enemy(bool is_boss = false)
        {
            if (is_boss)
            {
                return NW.choice(new List<Entity> { new EntityHasAI() { name = "The враг", hp = NW.random.Next(150, 200) } });
            }
            else
            {
                return NW.choice(new List<Entity> {
                    new EnemyCGoblin(),
                    new EnemyCSkeleton()
                });
            }
        }
    }
}
