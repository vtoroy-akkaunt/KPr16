using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using KPr16;

namespace KPr16
{
    public class EventItemUse : Event { }
    public class EventHealing : Event {
        public int hp;
    }
    public class ItemCHealing : EntityNamed
    {
        private int hp = 50;
        public ItemCHealing() {
            base.name = "Аптэчка";
        }
        public override string description => "Восстанавливает " + Convert.ToString(hp) + " HP";
        public override void proccess_event(ref Event e)
        {
            if (e is EventHealing ee)
            {
                ee.hp = hp;
            }
        }
        public override bool usable => true;
    }
    public class EventDamage : Event
    {
        public int hp;
    }
    public class ItemCWeapon : EntityNamed
    {
        private int hp = 66;
        public ItemCWeapon()
        {
            base.name = "Меч им. Альшаковой";
        }
        public override string description => $"Наносит {hp} урона";
        public override bool usable => true;
        public override void proccess_event(ref Event e)
        {
            if (e is EventDamage ee)
            {
                ee.hp = hp;
            }
        }
    }
    private EntityNamed random_weapon() {
        return new ItemCWeapon();
    }
    public class Entity: EntityLiving
    {
        public EntityNamed weapon;
        public EntityNamed armor; // maybe null
    }
    public class EnemyGoblin: Entity
    {
        public EnemyGoblin()
        {
            base.name = "Гоблин";
            base.hp = 88;
            base.weapon = 
        }
    }
    
    internal class Game
    {
        public List<EntityNamed> front = new List<EntityNamed>();
        EntityLiving player;
        bool is_item_now = true;
        public Game()
        {
            player = new EntityLiving();
            player.name = "Игрок";
            player.hp = 1337;
        }
        private static EntityLiving create_enemy(bool is_boss = false)
        {
            if (is_boss)
                return new EntityLiving() { name = "The ___", hp = 69 };
            else
                return new EntityLiving() { name = "___", hp = 37 };
        }
        private static EntityNamed create_item()
        {
            return new EntityNamed() { name = "Аптэчка" };
        }
        public enum PlayerAction
        {
            Skip, Use
        }
        public void proccess_player_action(PlayerAction action)
        {

        }
        private void next_stage()
        {
            if (is_item_now)
            {

            }
        }
    }
}
