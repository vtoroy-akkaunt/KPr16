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
