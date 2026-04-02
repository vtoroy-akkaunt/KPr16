using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KPr16;

namespace KPr16
{
    public class EventDamage: Event {}
    public class Entity: EntityLiving
    {
        EntityNamed weapon;
        EntityNamed armor;
    }
    
    internal class Game
    {
        public static EntityLiving create_enemy(bool is_boss = false)
        {
            if (is_boss)
                return new EntityLiving() { name = "The ___", hp = 69 };
            else
                return new EntityLiving() { name = "___", hp = 37 };
        }
    }
}
