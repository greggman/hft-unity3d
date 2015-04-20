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
/// </summary>
/// <example>
/// Example: Assume your command line is
/// <code>
/// myprogram --name=gregg --count=123 --debug
/// </code>
/// You can then read those options with
/// <code>
/// <![CDATA[
/// using HappyFunTimes;
///
/// ...
///    ArgParser p = new ArgParser();
///
///    // Set Defaults
///    string name = "Someone";
///    int count = 1;
///    bool debug = false;
///
///    p.TryGet<string>("name", name);
///    p.TryGet<int>("count", count);
///    if (p.Contains("debug")) {
///      debug = true;
///    }
/// ]]>
/// </code>
/// </example>
/// <returns></returns>
public class ArgParser {

    /// <summary>
    /// Constructor for ArgParser that parses command line arguments
    /// </summary>
    public ArgParser()
    {
        Init(System.Environment.GetCommandLineArgs());
    }

    /// <summary>
    /// Constructor for ArgParser which you can pass your own array of strings.
    /// </summary>
    /// <param name="args">Array of command line like argument strings</param>
    public ArgParser(string[] args)
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
    public bool TryGet<T>(string id, ref T value) {
        string v;
        bool found = m_switches.TryGetValue(id, out v);
        if (found) {
            value = (T)System.Convert.ChangeType(v, typeof(T));
        }
        return found;
    }

    private void Init(string[] arguments)
    {
        m_switches = new Dictionary<string, string>();
        foreach (string arg in arguments) {
            if (arg.StartsWith("--")) {
                int equalsNdx = arg.IndexOf('=');
                if (equalsNdx >= 0) {
                    m_switches[arg.Substring(2, equalsNdx - 2)] = arg.Substring(equalsNdx + 1);
                } else {
                    m_switches[arg.Substring(2)] = "true";
                }
            }
        }
    }

    private Dictionary<string, string> m_switches;
};


}  // namespace HappyFunTimes

