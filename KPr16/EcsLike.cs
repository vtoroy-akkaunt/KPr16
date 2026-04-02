using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace KPr16
{
    public class EntityNamed
    {
        public string name { get; set; }
    }
    public class EntityLiving: EntityNamed
    {
        public int hp { get; set; }
    }
    public class Event
    {
        public EntityNamed src { get; set; }
        public EntityNamed dst { get; set; }
    }
    public interface Behaviour
    {
        void process(Event e);
    }
}
