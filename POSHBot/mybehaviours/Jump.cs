﻿using System;
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
            : base(agent, new string[] { "dojump", "JumpForward" },
                        new string[] { "need_to_jump", "lose_your_way3" })
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
        [ExecutableAction("dojump")]
        public bool dojump()
        {
            if (_debug_)
                Console.Out.WriteLine(" in Jump");

            GetBot().SendMessage("JUMP", new Dictionary<string, string>());

            // This is an example command which sends a request to the game engine to let the character stop shooting.
            // The commands are based on the included GameBots2004 API which is available on the project webpage.
            // GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
            return true;
        }



        /// <summary>
        /// Jumps
        /// </summary>
        /// <returns>True or false, dependent if the action was executed successful.</returns>
        [ExecutableAction("JumpForward")]
        public bool JumpForward()
        {
            if (_debug_)
                Console.Out.WriteLine(" in JumpForward");

            GetBot().SendMessage("JUMP", new Dictionary<string, string>());

            // This is an example command which sends a request to the game engine to let the character stop shooting.
            // The commands are based on the included GameBots2004 API which is available on the project webpage.
            // GetBot().SendMessage("STOPSHOOT", new Dictionary<string, string>());
            return true;
        }

        /*
         * 
         * SENSES
         * 
         */

        [ExecutableSense("need_to_jump")]
        public bool need_to_jump()
        {
            if (_debug_)
                Console.Out.WriteLine("in NeedToJump");
            return false;
        }
    }
}