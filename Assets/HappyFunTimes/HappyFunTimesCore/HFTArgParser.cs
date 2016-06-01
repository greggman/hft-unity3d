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
using System.Collections.Generic;

namespace HappyFunTimes {

/// <summary>
/// A very simple commandline argument parser.
///
/// It just looks arguments that start with "--". For any
/// argument that starts with "--" it will then look for the
/// first "=" sign. If found it will set remember the value
/// for that option. If no equals is found it uses the value
/// "true"
///
/// Any argument can also be set by enviroment variable
/// where the - is replaced by _ and the name is uppercased
/// eg. --foo-bar checks for FOO_BAR environment variable
/// Command line arguments take precedence.
///
/// </summary>
/// <example>
/// Example: Assume your command line is
/// <code>
/// myprogram --name=gregg --count=123 --debug
/// </code>
///
/// You can then read those options with
/// <code>
/// <![CDATA[
/// using HappyFunTimes;
///
/// ...
///    HFTArgParser p = new HFTArgParser();
///
///    // Set Defaults
///    string name = "Someone";
///    int count = 1;
///    bool debug = false;
///
///    p.TryGet<string>("name", name);
///    p.TryGet<int>("count", count);
///    p.TryGetBool("debug", debug);
/// ]]>
/// </code>
/// </example>
/// <returns></returns>
public class HFTArgParser {

    /// <summary>
    /// Constructor for HFTArgParser that parses command line arguments
    /// </summary>
    public HFTArgParser()
    {
        #if (!UNITY_IOS)
            Init(System.Environment.GetCommandLineArgs());
        #else
            Init(new string[0]);
        #endif
    }

    /// <summary>
    /// Constructor for HFTArgParser which you can pass your own array of strings.
    /// </summary>
    /// <param name="args">Array of command line like argument strings</param>
    public HFTArgParser(string[] args)
    {
        Init(args);
    }

    /// <summary>
    /// Check if a switch exists.
    /// </summary>
    /// <param name="id">name of switch</param>
    /// <returns>true if switch was contained in arguments</returns>
    public bool Contains(string id)
    {
        return m_switches.ContainsKey(id);
    }

    /// <summary>
    /// Gets the value of a switch if it exists
    /// </summary>
    /// <param name="id">id of switch</param>
    /// <param name="value">variable to receive the value</param>
    /// <returns>true if the value exists. False if it does not. If false value has not been affected.</returns>
    public bool TryGet<T>(string id, ref T value)
    {
        string v = null;
        m_switches.TryGetValue(id, out v);

        if (v == null)
        {
            string envName = id.ToUpper(System.Globalization.CultureInfo.InvariantCulture).Replace("-", "_");
            v = System.Environment.GetEnvironmentVariable(envName);
        }

        if (v != null)
        {
            value = (T)System.Convert.ChangeType(v, typeof(T));
        }
        return v != null;
    }

    /// <summary>
    /// Specialed for bool because we care about whether or not it
    /// exists and/or is preceeded by `--no`
    /// </summary>
    /// <param name="id"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool TryGetBool(string id, ref bool value)
    {
        string v = null;
        if (TryGet<string>(id, ref v))
        {
            value = !v.ToLowerInvariant().Equals("false");
            return true;
        }

        if (TryGet<string>("no-" + id, ref v))
        {
            value = false;
            return true;
        }
        return false;
    }

    private void Init(string[] arguments)
    {
        m_switches = new Dictionary<string, string>();
        foreach (string arg in arguments)
        {
            if (arg.StartsWith("--"))
            {
                int equalsNdx = arg.IndexOf('=');
                if (equalsNdx >= 0)
                {
                    m_switches[arg.Substring(2, equalsNdx - 2)] = arg.Substring(equalsNdx + 1);
                }
                else
                {
                    m_switches[arg.Substring(2)] = "true";
                }
            }
        }
    }

    static public HFTArgParser GetInstance()
    {
        if (s_parser == null)
        {
            s_parser = new HFTArgParser();
        }
        return s_parser;
    }

    static private HFTArgParser s_parser;
    private Dictionary<string, string> m_switches;
};


}  // namespace HappyFunTimes

