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
    public class Jump : UTBehaviour
    {
        internal CombatInfo info;
        public Jump(AgentBase agent)
            :base(agent,new string[] {"jump"},
                        new string[] {})
        {
            info = new CombatInfo();
        }

        /*
        * 
        * ACTIONS 
        * 
        */

        /// <summary>
        /// Jumps
        /// </summary>
        /// <returns>True or false, dependent if the action was executed successful.</returns>
        [ExecutableAction("jump")]
        public bool Jump()
        {
            if (_debug_)
                Console.Out.WriteLine(" in TemplateAction1");

            // This is an example command which sends a request to the game engine to let the character stop shooting.
            // The commands are based on the included GameBots2004 API which is available on the project webpage.
            // GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
            return false;
        }

        /*
         * 
         * SENSES
         * 
         */
    }
}