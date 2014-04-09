using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using POSH_sharp.sys;
using POSH_sharp.sys.annotations;
using Posh_sharp.POSHBot.util;
using POSH_sharp.sys.strict;

namespace Posh_sharp.POSHBot
{
    public class MyMovement: UTBehaviour
    {
        internal PositionsInfo info;
        string pathHomeId;
        string reachPathHomeId;
        string pathToEnemyBaseId;
        string reachPathToEnemyBaseID;

        public MyMovement(AgentBase agent)
            : base(agent,
            new string[] { "retrace_navpoint2", "WobbleForward" },
            new string[] {  })
        {
            this.info = new PositionsInfo();
            pathHomeId = "PathHome";
            reachPathHomeId = "ReachPathHome";
            pathToEnemyBaseId = "PathThere";
            reachPathToEnemyBaseID = "ReachPathThere";

        }


        /*
         * 
         * internal methods
         * 
         */

        /// <summary>
        /// updates the flag positions in PositionsInfo
        /// also updates details of bases, if relevant info sent
        /// the position of a flag is how we determine where the bases are
        /// </summary>
        /// <param name="values">Dictionary containing the Flag details</param>
        override internal void ReceiveFlagDetails(Dictionary<string, string> values)
        {
            // TODO: fix the mix of information in this method it should just contain relevant info

            
            if (GetBot().info == null || GetBot().info.Count < 1)
                return;
            // set flag stuff
            if (values["Team"] == GetBot().info["Team"])
            {
                if (info.ourFlagInfo != null && info.ourFlagInfo.ContainsKey("Location"))
                    return;
                info.ourFlagInfo = values;
            }
            else
            {
                if (info.enemyFlagInfo != null && info.enemyFlagInfo.ContainsKey("Location"))
                    return;
                info.enemyFlagInfo = values;
            }

            if (values["State"] == "home")
                if (values["Team"] == GetBot().info["Team"])
                    info.ownBasePos = NavPoint.ConvertToNavPoint(values);
                else
                    info.enemyBasePos = NavPoint.ConvertToNavPoint(values);
        }

        public string GetBaseNavId()
        {
            return GetNavigator().GetSelectedNavpoint().Id;
            //return GetMovement().info.ownBasePos.Id;
        }

        /*
        * 
        * ACTIONS 
        * 
        */
        [ExecutableAction("retrace_navpoint2")]
        public bool retrace_navpoint2()
        {
            if (GetMovement().KnowOwnBasePos())
            {
                // if at own base, stay there rather than overshooting
                if (GetNavigator().at_own_base())
                {
                    return false;
                    //GetNavigator().select_navpoint(GetBaseNavId());
                }
            }

            // we need to clear navpoints sometime if we realise we are in our base.
            return GetNavigator().retrace_navpoint();
        }

        [ExecutableAction("WobbleForward")]
        public bool WobbleForward()
        {
            if (GetNavigator().selected_target())
            {
                GetBot().SendMessage("JUMP", new Dictionary<string, string>());
                Thread.Sleep(100);
                GetBot().SendMessage("DODGE", new Dictionary<string, string> { { "Direction", GetNavigator().GetSelectedNavpoint().Location.ToString() } });
            }

            // we need to clear navpoints sometime if we realise we are in our base.
            return true;
        }
       

        /*
         * 
         * SENSES
         * 
         */

        
    }
}