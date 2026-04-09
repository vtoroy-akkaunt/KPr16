using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace KPr16
{
    public class EntityNamed
    {
        public string name { get; set; }
        public ImageSource image { get; set; } // maybe null
        public virtual string description { get { return name; } }
        public virtual void proccess_event(ref Event e) {}
        public virtual bool usable { get { return false; } }
        public virtual bool usable_against { get { return false; } }
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
