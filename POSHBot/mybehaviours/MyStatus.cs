using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using POSH_sharp.sys;
using POSH_sharp.sys.annotations;
using Posh_sharp.POSHBot.util;
using POSH_sharp.sys.strict;

namespace Posh_sharp.POSHBot
{
    public class MyStatus : UTBehaviour
    {
        public MyStatus(AgentBase agent)
            : base(agent, new string[] { },
                        new string[] { "AreArmed2", "AmmoAmount2", "ArmedAndAmmo2", "teammate_has_enemy_flag", "enemy_has_our_flag" })
        {
        }

        [ExecutableSense("AreArmed2")]
        public bool AreArmed2()
        {
            if (GetBot().info.Count == 0)
                return false;

            if (GetBot().info["Weapon"] == "None")
            {
                Console.WriteLine("unarmed");
                return false;
            }
            Console.WriteLine("armed with " + GetBot().info["Weapon"]);

            return true;
        }

        [ExecutableSense("AmmoAmount2")]
        public int AmmoAmount2()
        {
            if (GetBot().info.Count == 0)
                return 0;
            
            var inf = GetBot().info;
            if (GetBot().info.ContainsKey("PrimaryAmmo"))
            {
                var ammo = GetBot().info["PrimaryAmmo"];
                Console.WriteLine(ammo);
                var parsed = int.Parse(ammo);
                return parsed;
            }
            // doesn't seem to have such a key
            return -1;
            //return int.Parse(GetBot().info["CurrentAmmo"]);
        }

        [ExecutableSense("ArmedAndAmmo2")]
        public bool ArmedAndAmmo2()
        {
            return (AreArmed2() && AmmoAmount2() > 0) ? true : false;
        }

        [ExecutableSense("teammate_has_enemy_flag")]
        public bool teammate_has_enemy_flag()
        {
            if (_debug_)
                Console.Out.WriteLine("in teammate_has_enemy_flag");

            if (GetCombat().info.HoldingEnemyFlag != null && GetCombat().info.HoldingEnemyFlag != GetBot().info["BotId"])
            {
                return true;
            }

            return false;
        }

        [ExecutableSense("enemy_has_our_flag")]
        public bool enemy_has_our_flag()
        {
            if (_debug_)
                Console.Out.WriteLine("in enemy_has_our_flag");

            if (GetCombat().info.HoldingEnemyFlag != null && GetCombat().info.HoldingEnemyFlag == GetBot().info["BotId"])
            {
                return true;
            }

            return false;
        }
    }
}
