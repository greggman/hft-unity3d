/*
 * Copyright 2014, Gregg Tavares.
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are
 * met:
 *
 *     * Redistributions of source code must retain the above copyright
 * notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above
 * copyright notice, this list of conditions and the following disclaimer
 * in the documentation and/or other materials provided with the
 * distribution.
 *     * Neither the name of Gregg Tavares. nor the names of its
 * contributors may be used to endorse or promote products derived from
 * this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
 * A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
 * OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
 * DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
 * THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using DeJson;
using UnityEngine;

namespace HappyFunTimes
{

    [Serializable]
    public class HFTGameOptions
    {
        ///<summary>
        /// there's generally no need to set this.
        ///</summary>
        public string gameId = "";

        /// <summary>
        /// Name of game (shown if more than one game running on WiFi)
        /// </summary>
        [Tooltip("Used when > 1 games on same wifi. Blank = app name")]
        public string name = "";

        ///<summary>
        /// id used for multigames games. Can be set from command line
        /// with --hft-id=someid
        ///</summary>
        [Tooltip("For multi-computer games. Set from command line --hft-id=<id>")]
        public string id = "";

        /// <summary>
        /// As the user for a name when they start
        /// </summary>
        [Tooltip("ask users for name when they join")]
        public bool askUserForName = true;

        /// <summary>
        /// Show game menu(allow user to set name)
        /// </summary>
        [Tooltip("show gear menu on controller")]
        public bool showMenu = true;

        ///<summary>
        ///For a multiple machine game designates this game as the game where players start.
        ///Default: false
        ///Can be set from command line with --hft-master
        ///</summary>
        [Tooltip("for multi-computer games. Set from command line --hft-master")]
        public bool master;

        ///<summary>
        ///The URL of HappyFunTimes
        ///
        ///Normally you don't need to set this as HappyFunTimes is running on the same machine
        ///as the game. But, for multi-machine games you'd need to tell each machine the address
        ///of the machine running HappyFunTimes. Example: "ws://192.168.2.9:18679".
        ///
        ///Can be set from the command line with --hft-url=someurl
        ///</summary>
        [Tooltip("for mulit-computer games. Set from command line --hft-url=<someurl>")]
        public string url;

        ///<summary>
        ///Normally if a game disconnets all players are also disconnected. This means
        ///they'll auto join the next game running.
        ///Default: true
        ///</summary>
        public bool disconnectPlayersIfGameDisconnects;

        ///<summary>
        ///Prints all the messages in and out to the console.
        ///</summary>
        [Tooltip("debugging")]
        public bool showMessages;

        ///<summary>
        /// don't set this. it will be set automatically
        ///</summary>
        [HideInInspector]
        public string cwd;

        /// <summary>
        /// whether or not to show in list of games
        /// </summary>
        public bool showInList = true;

        /// <summary>
        /// Whether or not to start the server
        ///
        /// By default it's true unless HFT_URL is set then it's false;
        /// </summary>
        public bool startServer = true;

        /// <summary>
        /// URL of rendezvous server. Defaults to
        /// http://happyfuntimes.net/api/inform2. If you're running your
        /// own server you'd change this.
        /// </summary>
        [Tooltip("default: happyfuntimes.net")]
        public string rendezvousUrl;

        /// <summary>
        /// Port to run server on
        /// </summary>
        public string serverPort = "";

        public string getGameId()
        {
            return String.IsNullOrEmpty(gameId) ? "HFTUnity" : gameId;
        }
    }

}  // namespace HappyFunTimes
