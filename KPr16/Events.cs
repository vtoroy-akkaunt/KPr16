using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPr16
{
    public class EventIgnore : Event { }
    public class EventItemUse : Event { }
    public class EventHealing : Event
    {
        public int hp;
    }
    public class EventFreeze : Event { }
    public class EventDamage : Event
    {
        public int hp;
        public bool ignores_armor = false;
    }
    public class EventOnceProtection : Event
    {
        public int hp;
    }
}
