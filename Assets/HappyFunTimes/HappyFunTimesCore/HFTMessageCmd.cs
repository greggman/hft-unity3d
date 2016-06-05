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

namespace HappyFunTimes
{

    public class HFTMessageCmd
    {
        public HFTMessageCmd()
        {
            cmd = "";
            data = null;
        }

        public HFTMessageCmd(string _cmd, object _data)
        {
            cmd = _cmd;
            data = _data;
        }

        public string cmd;           // command to emit
        public object data;  // data for command
    };

/// <summary>
/// Deprecated?
///
/// The only plus to this class is you define the command name
/// once and then when you register it no need to specify the
/// cmd name and if you send it no need to specify the command
/// so in that sense it's a D.R.Y. thing
///
/// Base class for all message data. Be sure to set the CmdName attribute
/// </summary>
/// <example>
/// <code>
///
/// [CmdName("playerHit")]
/// public class PlayerHit : MessageCmdData
///     public int numHitPoints;
/// }
/// </code>
/// </example>
    public class MessageCmdData
    {

        public static string GetCmdName(System.Type type)
        {
            //Querying Class Attributes
            foreach (Attribute attr in type.GetCustomAttributes(true))
            {
                CmdNameAttribute cmdNameAttr = attr as CmdNameAttribute;
                if (cmdNameAttr != null)
                {
                    return cmdNameAttr.CmdName;
                }
            }
            return null;
        }

    }

// This is just to cache the command names since iterating over fields seems like it
// would be slow. Probably a pre-mature optimization.
    public class HFTMessageCmdDataNameDB
    {

        public static string GetCmdName(System.Type type)
        {
            string name;
            if (!m_typeToCommandName.TryGetValue(type, out name))
            {
                name = MessageCmdData.GetCmdName(type);
                m_typeToCommandName[type] = name;
            }
            return name;
        }

        private static Dictionary<System.Type, string> m_typeToCommandName = new Dictionary<System.Type, string>();
    }

/// <summary>
/// Attibute used to associate a command name with a class.
/// In C# land we could just use the name of the class through reflection
/// but these names originate in JavaScript where there is no class name.
/// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CmdNameAttribute : System.Attribute
    {
        public readonly string CmdName;

        public CmdNameAttribute(string cmdName)
        {
            this.CmdName = cmdName;
        }
    }

    public class HFTMessageSwitchGame
    {
        public string gameId;
        public object data;

        public HFTMessageSwitchGame() { } // for deserialization
        public HFTMessageSwitchGame(string id, object d)
        {
            gameId = id;
            data = d;
        }
    }

    public class HFTMessageGameDisconnect
    {
        public string id;

        public HFTMessageGameDisconnect(string _id)
        {
            id = _id;
        }
    }

    public class HFTMessageGameStart
    {
        public string id = "";
        public string gameId = "";
    };

    public class RelayServerCmd
    {
        public RelayServerCmd(string _cmd, string _id, object _data)
        {
            cmd = _cmd;
            id = _id;
            data = _data;
        }

        public string cmd;
        public string id;
        public object data;
    }

    public class HFTMessageLog
    {
        public HFTMessageLog(string _type, string _msg)
        {
            type = _type;
            msg = _msg;
        }
        public string type = "";
        public string msg = "";
    }

    public class HFTMessageAssignAsClientForGame
    {
        public string gameId;
    }

    public class HFTMessageAddFile
    {
        public HFTMessageAddFile() {}
        public HFTMessageAddFile(string filename, string data)
        {
            this.filename = filename;
            this.data = data;
        }
        public string filename;
        public string data;
    }

    public class HFTTimePing
    {
        public HFTTimePing() {}
        public HFTTimePing(double currentTimeInSeconds)
        {
            this.time = currentTimeInSeconds;
        }
        public double time;
    }

}  // namespace HappyFunTimes
