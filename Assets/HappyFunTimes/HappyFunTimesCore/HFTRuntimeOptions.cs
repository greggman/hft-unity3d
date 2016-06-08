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
    public class HFTRuntimeOptions {
        public HFTRuntimeOptions()
        {
        }

        public HFTRuntimeOptions(HFTGameOptions userOptions)
        {
            name = String.IsNullOrEmpty(userOptions.name) ? Application.productName : userOptions.name;

            id = userOptions.id;
            // gameId = String.IsNullOrEmpty(Application.productName) ? "HFTUnityUnnamed" : HFTUtil.SafeName(Application.productName);
            // This currently has to be HFTUnity because the controller doesn't know what gameId to use
            // We could send it with "?gameId=" but does it really matter?
            gameId = "HFTUnity";
            serverPort = String.IsNullOrEmpty(userOptions.serverPort) ? "18679" : userOptions.serverPort;
            rendezvousUrl = userOptions.rendezvousUrl;
            master = userOptions.master;
            url = userOptions.url;
            dns = userOptions.dns;
            captivePortal = userOptions.captivePortal;
            showMessages = HFTHappyFunTimesSettings.showMessages;
            debug = HFTHappyFunTimesSettings.debug;
            ipv4DnsAddress = userOptions.ipv4DnsAddress;
            ipv6DnsAddress = userOptions.ipv6DnsAddress;
            controllerFilename = userOptions.controllerFilename;
            if (String.IsNullOrEmpty(controllerFilename))
            {
                controllerFilename = "controller.html";
            }
            controllerFilename = controllerFilename.Replace("\\", "/");

            HFTArgs args = new HFTArgs();

            args.installationMode.GetIfSet(ref installationMode);
            args.master.GetIfSet(ref master);
            args.showMessages.GetIfSet(ref showMessages);
            args.debug.GetIfSet(ref debug);

            args.url.GetIfSet(ref url);
            args.id.GetIfSet(ref id);
            args.rendezvousUrl.GetIfSet(ref rendezvousUrl);
            args.serverPort.GetIfSet(ref serverPort);
            args.ipv4DnsAddress.GetIfSet(ref ipv4DnsAddress);
            args.ipv6DnsAddress.GetIfSet(ref ipv6DnsAddress);

            if (HFTHappyFunTimesSettings.installationMode)
            {
                installationMode = true;
            }

            if (String.IsNullOrEmpty(url))
            {
                url = "ws://localhost:" + serverPort;
                startServer = true;
            } else {
                startServer = false;
            }

            #if UNITY_STANDALONE_OSX
                // We only provide an external server for OSX at the moment
                // That's because AFAIK on Windows we can get ports 80 and 53 without admin
                // (might be prompted for network access)
                // In Linux people can run with sudo (or submit a PR for a better way)
                // On OSX we don't want to run as sudo because it makes root own unity's logs
                if (dns || installationMode)
                {
                    startExternalServer = true;
                }
                if (startExternalServer)
                {
                    // is there any reason this wouldn't be true?
                    // You're running as slave. Then you're not starting a server
                    // You're running as master. You may or may not be starting a server
                    sendFilesToServer = true;
                }
            #endif

        }

        public string url = "";
        public string id = "";
        public string name = "";
        public string gameId = "HFTUnity";
        public string controllerFilename = "";
        public bool disconnectPlayersIfGameDisconnects = true;
        public bool installationMode = false;
        public bool master = false;
        public bool showInList = true;
        public bool showMessages;
        public string debug = "";
        public bool startServer;
        public bool dns;
        public bool captivePortal;
        public bool syncedClock = true;
        public string serverPort = "";
        public string rendezvousUrl;
        public string ipv4DnsAddress = "";
        public string ipv6DnsAddress = "";

        #if UNITY_STANDALONE_OSX
            public bool sendFilesToServer;
            public bool startExternalServer;
        #endif

        public class HFTArgs : HFTArgsBase
        {
            public HFTArgs() : base("hft") {
            }
            public HFTArg<string> url;
            public HFTArg<string> id;
            public HFTArg<string> rendezvousUrl;
            public HFTArg<string> serverPort;
            public HFTArgBool installationMode;
            public HFTArgBool master;
            public HFTArgBool showMessages;
            public HFTArg<string> debug;
            public HFTArg<string> ipv4DnsAddress;
            public HFTArg<string> ipv6DnsAddress;
        }

    }

}  // namespace HappyFunTimes
