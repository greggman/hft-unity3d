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
        [Tooltip("name of controller html file. Must be in 'WebPlayerTemplates/HappyFunTimes'")]
        public string controllerFilename = "controllers/gamepad/controller.html";

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

        ///<summary>
        ///For a multiple machine game designates this game as the game where players start.
        ///Default: false
        ///Can be set from command line with --hft-master
        ///</summary>
        [Tooltip("For multi-computer games. Players start on master. Set from command line with --hft-master")]
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

        ///Normally if a game disconnets all players are also disconnected. This means
        ///they'll auto join the next game running.
        ///Default: true
        ///</summary>
        public bool disconnectPlayersIfGameDisconnects;

        /// <summary>
        /// Whether or not to run DNS server
        /// </summary>
        [Tooltip("for debugging. Set automatically with --hft-installation-mode")]
        public bool dns = false;

        /// <summary>
        /// Whether or not to run captive portal
        /// </summary>
        [Tooltip("for debugging. Set automatically with --hft-installation-mode")]
        public bool captivePortal = false;

        /// <summary>
        /// URL of rendezvous server. Defaults to
        /// http://happyfuntimes.net/api/inform2. If you're running your
        /// own server you'd change this.
        /// </summary>
        [Tooltip("default: http://happyfuntimes.net/api/inform2")]
        public string rendezvousUrl;

        /// <summary>
        /// Port to run server on
        /// </summary>
        public string serverPort = "";

        /// <summary>
        /// address to report for DNS A queries
        /// </summary>
        [Tooltip("addres to report for DNS A queries")]
        public string ipv4DnsAddress = "";

        /// <summary>
        /// address to report for DNS AAAA queries
        /// </summary>
        [Tooltip("addres to report for DNS AAAA queries")]
        public string ipv6DnsAddress = "";
    }

}  // namespace HappyFunTimes
