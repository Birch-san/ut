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
    public class MyNavigator : UTBehaviour
    {
        private NavPoint __selectedNavpoint__;
        private string __closestNavpointID__;
        private string __lastVisitedNavpoint__;
        private int __deviation__;

        private Dictionary<string, bool> closestNavPointReachable;
        private Dictionary<string, int> navPointHistory;
        private Dictionary<string, NavPoint> navPoints;

        private int directionWeight;

        internal CombatInfo info;
        public MyNavigator(AgentBase agent)
            : base(agent, new string[] { },
                        new string[] { "my_select_navpoint" })
        {
            closestNavPointReachable = new Dictionary<string, bool>();
            navPointHistory = new Dictionary<string, int>();
            navPoints = new Dictionary<string, NavPoint>();
            __closestNavpointID__ = "";
            directionWeight = 1;
            __deviation__ = 50;
        }

        /*
        * 
        * ACTIONS 
        * 
        */

        /*
         * 
         * SENSES
         * 
         */

        [ExecutableSense("my_select_navpoint")]
        public bool my_select_navpoint(string navID)
        {
            string leastVisitedNav = "";

            directionWeight = 1;
            Console.Out.WriteLine("in select_navpoint");

            if (navID != "" && navPoints.ContainsKey(navID))
            {
                __selectedNavpoint__ = navPoints[navID];
                return true;
            }

            if (__closestNavpointID__ == null)
                return false;

            int count = 0;

            foreach (NavPoint.Neighbor nei in navPoints[__closestNavpointID__].NGP)
                if (leastVisitedNav == "")
                {
                    leastVisitedNav = nei.Id;
                    if (navPointHistory.ContainsKey(leastVisitedNav))
                        count = navPointHistory[leastVisitedNav];
                    else
                        break;
                }
                else if (navPointHistory.ContainsKey(nei.Id))
                {
                    if (navPointHistory[nei.Id] < count)
                    {
                        leastVisitedNav = nei.Id;
                        count = navPointHistory[nei.Id];
                    }
                }
                else
                {
                    leastVisitedNav = nei.Id;
                    break;
                }
            if (leastVisitedNav == "")
            {
                __selectedNavpoint__ = null;
                return false;
            }

            __selectedNavpoint__ = navPoints[leastVisitedNav];

            if (_debug_)
                Console.Out.WriteLine("selected Navpoint: " + leastVisitedNav);

            return true;
        }
    }
}