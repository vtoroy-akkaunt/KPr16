using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace KPr16
{
    public class EnemyCGoblin : EntityHasAI
    {
        public EnemyCGoblin()
        {
            base.name = "Гоблин";
            base.hp = 30;
            base.weapon = new ItemCCritWeapon(NW.random.Next(10, 15), 0.5);
            base.armor = new ItemCGenericShield(3);
        }
        public override ImageSource image => new BitmapImage(new Uri("C:\\Users\\234912\\Pictures\\1.jpg"));
    }
    public class EnemyCSkeleton : EntityHasAI
    {
        public override ImageSource image => new BitmapImage(new Uri("C:\\Users\\234912\\Pictures\\2.jpg"));
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
        public override ImageSource image => new BitmapImage(new Uri("C:\\Users\\234912\\Pictures\\3.jpg"));
        public EnemyCMage()
        {
            base.name = "Маг";
            base.hp = 25;
            base.weapon = new ItemCFreezes(NW.random.Next(20, 30), 0.5);
            base.armor = new ItemCGenericShield(2);
        }
    }
    //
    public class EnemyCBossVVG : EnemyCGoblin
    {
        public EnemyCBossVVG()
        {
            base.name = "ВВГ";
            base.hp = 60;
            base.weapon = new ItemCCritWeapon(NW.random.Next(10, 15), 0.6);
            base.armor = new ItemCGenericShield(3);
        }
    }
    public class EnemyCBossKowalsky : EnemyCSkeleton
    {
        public EnemyCBossKowalsky()
        {
            base.name = "Kowalsky";
            base.hp = 100;
            base.weapon = new ItemCIgnoresArmor(NW.random.Next(15, 25), 1.0);
            base.armor = new ItemCGenericShield(5);
        }
    }
    public class EnemyCBossArchimageCpp : EnemyCMage
    {
        public EnemyCBossArchimageCpp()
        {
            base.name = "Архимаг С++";
            base.hp = 40;
            base.weapon = new ItemCFreezes(NW.random.Next(20, 30), 0.5);
            base.armor = new ItemCGenericShield(2);
        }
    }
    public class EnemyCBossArchimageCmm : EnemyCSkeleton
    {
        public EnemyCBossArchimageCmm()
        {
            base.name = "Архимаг С--";
            base.hp = 40;
            base.weapon = new ItemCIgnoresArmor(NW.random.Next(20, 30), 0.5);
            base.armor = new ItemCGenericShield(20);
        }
    }
    partial class Repo
    {
        public static Entity random_enemy(bool is_boss = false)
        {
            if (is_boss)
            {
                return NW.choice(new List<Entity> {
                    new EnemyCBossArchimageCmm(),
                    new EnemyCBossArchimageCpp(),
                    new EnemyCBossKowalsky(),
                    new EnemyCBossVVG(),
                });
            }
            else
            {
                return NW.choice(new List<Entity> {
                    new EnemyCGoblin(),
                    new EnemyCSkeleton(),
                    new EnemyCMage(),
                });
            }
        }
    }
}
