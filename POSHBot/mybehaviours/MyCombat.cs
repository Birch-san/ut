using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Net;

using POSH_sharp.sys;
using POSH_sharp.sys.annotations;
using Posh_sharp.POSHBot.util;
using POSH_sharp.sys.strict;

namespace Posh_sharp.POSHBot
{
    public class MyCombat : UTBehaviour
    {
        private bool targetChosen = false;
        private bool hasSkinned = false;

        internal CombatInfo info;
        public MyCombat(AgentBase agent)
            : base(agent, new string[] { "doskin", "Taunt", "Resist", "ChooseBestWeapon" },
                        new string[] { "SeeEnemy2", "skinned", "SeekAttacker", "PaintedTarget" })
        {
            info = new CombatInfo();
        }

        /// <summary>
        /// sets the attacker (i.e. the keepfocuson one) to be the first enemy player we have seen
        /// or the instigator of the most recent damage, if we know who that is
        /// </summary>
        /// <returns></returns>
        [ExecutableSense("SeekAttacker")]
        public bool SeekAttacker()
        {
            Console.Out.WriteLine(" in SeekAttacker");
            // FOR NOW



            return GetCombat().SetAttacker();
        }

        [ExecutableSense("SeeEnemy2")]
        public bool SeeEnemy2()
        {
            bool result = GetMovement().SeeEnemy();
            if (!result)
            {
                GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
            }
            return result;
        }

        [ExecutableAction("ChooseBestWeapon")]
        public bool ChooseBestWeapon()
        {
            if (GetBot().info.ContainsKey("Weapon"))
            {
                string mapName = GetBot().info["Weapon"].Substring(0, GetBot().info["Weapon"].LastIndexOf('.'));

                GetBot().SendMessage("CHANGEWEAPON", new Dictionary<string, string> { { "Id", mapName + ".AssaultRifle" } });
                GetBot().SendMessage("CHANGEWEAPON", new Dictionary<string, string> { { "Id", mapName + ".ShockRifle" } });
                GetBot().SendMessage("CHANGEWEAPON", new Dictionary<string, string> { { "Id", mapName + ".Minigun" } });
                return true;
            }
            return true;
        }

        [ExecutableAction("Resist")]
        public bool Resist()
        {
            GetCombat().FaceAttacker();
            GetCombat().ShootAttacker();

            return true;
        }

        [ExecutableAction("Taunt")]
        public bool Taunt()
        {
            Random random = new Random();
            int randomNumber = random.Next(0, 100);
            if (randomNumber > 85)
            {
                var client = new WebClient();
                // Download Text From web
                var text = client.DownloadString("http://www.pangloss.com/seidel/Shaker/index.html");
                var parts = text.Split(new string[] { "font" }, StringSplitOptions.None);
                var interest = parts[1].Substring(12);
                interest = interest.Remove(interest.Length - 2);

                GetBot().SendMessage("MESSAGE", new Dictionary<string, string> { { "Global", "True" }, { "Text", interest } });
            }
            return true;
        }

        [ExecutableAction("doskin")]
        public bool doskin()
        {
            GetBot().SendMessage("SETSKIN", new Dictionary<string, string> { { "Skin", "HumanFemaleA.MercFemaleB" } });
            hasSkinned = true;
            return true;
        }

        [ExecutableSense("skinned")]
        public bool skinned()
        {
            return hasSkinned;
        }

        private bool FindEnemyInView()
        {
            // whether we saw anyone
            var foundTarget = false;

            // work through who we can see, looking for an enemy
            string ourTeam = GetBot().info["Team"];
            Console.Out.WriteLine("Our Team: " + ourTeam);
            foreach (UTPlayer player in GetBot().viewPlayers.Values)
            {
                if (player.Team != ourTeam)
                {
                    // Turned KeepFocusOnID in to a tuple with the current_time as a timestamp FA
                    info.KeepFocusOnID = new Tuple<string, long>(player.Id, TimerBase.CurrentTimeStamp());
                    info.KeepFocusOnLocation = new Tuple<Vector3, long>(player.Location, TimerBase.CurrentTimeStamp());

                    foundTarget = true;
                    break;
                }
            }

            return foundTarget;
        }

        /// <summary>
        /// if its status is "held", update the CombatInfoClass to show who's holding it
        /// otherwise, set that to None as it means no-one is holding it
        /// </summary>
        /// <param name="values">Dictionary containing the Flag details</param>
        override internal void ReceiveFlagDetails(Dictionary<string, string> values)
        {
            // TODO: fix the mix of information in this method it should just contain relevant info
            // if (_debug_)
            //     Console.Out.WriteLine("in receiveFlagDetails");

            if (GetBot().info == null || GetBot().info.Count < 1)
                return;
            // set flag stuff
            if (values["Team"] == GetBot().info["Team"])
                if (values["State"].ToLower() == "held")
                    info.HoldingOurFlag = (values.ContainsKey("Holder")) ? values["Holder"] : string.Empty;
                else
                {
                    info.HoldingOurFlag = string.Empty;
                    info.HoldingOurFlagPlayerInfo = null;
                }
            else
            {
                if (values["State"].ToLower() == "held" && values.ContainsKey("Holder"))
                {
                    if (GetBot().viewPlayers.ContainsKey(values["Holder"]))
                        info.HoldingEnemyFlagPlayerInfo = GetBot().viewPlayers[values["Holder"]];
                    info.HoldingEnemyFlag = values["Holder"];
                }
                else
                {
                    info.HoldingEnemyFlag = string.Empty;
                    info.HoldingEnemyFlagPlayerInfo = null;
                }
            }
        }

        override internal void ReceiveProjectileDetails(Dictionary<string, string> values)
        {
            if (_debug_)
                Console.Out.WriteLine("received details of incoming projectile!");
            info.ProjectileDetails = new Projectile(values);
        }

        override internal void ReceiveDamageDetails(Dictionary<string, string> values)
        {
            if (_debug_)
                Console.Out.WriteLine("received details of damage!");
            info.DamageDetails = new Damage(values);
        }

        /// <summary>
        /// handle details about a player (not itself) dying
        /// remove any info about that player from CombatInfo
        /// </summary>
        /// <param name="values"></param>
        override internal void ReceiveKillDetails(Dictionary<string, string> values)
        {
            if (_debug_)
                Console.Out.WriteLine("received details of a kill!");

            info.ProjectileDetails = new Projectile(values);

            if (values["Id"] == info.HoldingOurFlag)
            {
                info.HoldingOurFlag = string.Empty;
                info.HoldingOurFlagPlayerInfo = null;
                GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
            }

            if (info.KeepFocusOnID != null && info.KeepFocusOnID.First != string.Empty)
                if (values["Id"] == info.KeepFocusOnID.First)
                {
                    info.GetFocusId();
                    info.GetFocusLocation();
                    GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
                }

        }

        override internal void ReceiveDeathDetails(Dictionary<string, string> value)
        {
            info.DamageDetails = null;
            info.KeepFocusOnID = null;
            info.KeepFocusOnLocation = null;
            GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
        }
    }
}
