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
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using HappyFunTimes;

namespace HappyFunTimesEditor {

public class HFTCmdRunner
{
    public HFTCmdRunner(bool async = false)
    {
        m_async = async;
    }

    public void Run(string cmdPath, string[] arguments = null, string workingDirectory = null)
    {
        if (m_proc != null)
        {
            throw new System.InvalidOperationException("command already running");
        }

        m_stdoutSB = new System.Text.StringBuilder();
        m_stderrSB = new System.Text.StringBuilder();
        m_proc = new Process();
        ProcessStartInfo psi = m_proc.StartInfo;

        psi.FileName = cmdPath;
        if (arguments != null && arguments.Length > 0)
        {
            psi.Arguments = EscapeArguments(arguments);

        }

        if (debug)
        {
            UnityEngine.Debug.Log(cmdPath);
            UnityEngine.Debug.Log(psi.Arguments);
        }

        if (System.String.IsNullOrEmpty(workingDirectory))
        {
            workingDirectory = System.IO.Directory.GetCurrentDirectory();
        }

        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        m_proc.EnableRaisingEvents = true;
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = workingDirectory;
        psi.UseShellExecute = false;

        if (m_async)
        {
            m_proc.Exited += proc_HasExited;
        }

        m_proc.ErrorDataReceived += proc_ErrorDataReceived;
        m_proc.OutputDataReceived += proc_OutDataReceived;

        m_proc.Start();

        m_proc.BeginErrorReadLine();
        m_proc.BeginOutputReadLine();

        if (!m_async) {
            m_proc.WaitForExit();
            End();
        }
    }

    public void RunNoWait(string cmdPath, string[] arguments = null, string workingDirectory = null)
    {
        if (m_proc != null)
        {
            throw new System.InvalidOperationException("command already running");
        }

        m_proc = new Process();
        ProcessStartInfo psi = m_proc.StartInfo;

        psi.FileName = cmdPath;
        if (arguments != null && arguments.Length > 0)
        {
            psi.Arguments = EscapeArguments(arguments);

        }

        if (debug)
        {
            UnityEngine.Debug.Log(cmdPath);
            UnityEngine.Debug.Log(psi.Arguments);
        }

        if (System.String.IsNullOrEmpty(workingDirectory))
        {
            workingDirectory = System.IO.Directory.GetCurrentDirectory();
        }

        psi.RedirectStandardOutput = false;
        psi.RedirectStandardError = false;
        m_proc.EnableRaisingEvents = false;
        psi.CreateNoWindow = true;
        psi.WorkingDirectory = workingDirectory;
        psi.UseShellExecute = true;

        m_proc.Start();
    }

    // "Open" on OSX, "Start" on Windows
    public void Open(string[] arguments, string workingDirectory = null)
    {
        string exePath = "";
        List<string> preArgs = new List<string>();

        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                exePath = arguments[0];
                for (int ii = 1; ii < arguments.Length; ++ii)
                {
                    preArgs.Add(arguments[ii]);
                }
                arguments = new string[0];
                break;
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                exePath = "/usr/bin/open";
                break;
            default:
                UnityEngine.Debug.LogError("Open not supported on this platform");
                return;
        }
        string[] newArgs = new string[arguments.Length + preArgs.Count];
        for (int ii = 0; ii < preArgs.Count; ++ii)
        {
            newArgs[ii] = preArgs[ii];
        }
        arguments.CopyTo(newArgs, preArgs.Count);
        RunNoWait(exePath, newArgs, workingDirectory);
    }

    public void HFTRun(string[] arguments = null, string workingDirectory = null)
    {
        HFTExe hftExe = HFTExe.Instance;

        if (!hftExe.GetHFTPath())
        {
            UnityEngine.Debug.LogError("Could not run HappyFunTimes");
            return;
        }
        string[] newArgs = new string[(arguments == null ? 0 : arguments.Length) + 1];
        newArgs[0] = hftExe.hftPath;
        arguments.CopyTo(newArgs, 1);
        Run(hftExe.nodePath, newArgs, workingDirectory);
    }

    private string EscapeArguments(string[] arguments)
    {
        string[] escapedArgs = new string[arguments.Length];
        for (int ii = 0; ii < arguments.Length; ++ii)
        {
            string arg = arguments[ii];
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                case RuntimePlatform.WindowsPlayer:
                    escapedArgs[ii] = "\"" + arg.Replace("\"", "^\"") + "\"";
                    break;
                default:
                    escapedArgs[ii] = "\"" + arg.Replace("\"", "\\\"") + "\"";
                    break;
            }
        }
        return System.String.Join(" ", escapedArgs);
    }

    void End()
    {
        ExitCode = m_proc.ExitCode;
        stdout = m_stdoutSB.ToString();
        stderr = m_stderrSB.ToString();
        m_stdoutSB = null;
        m_stderrSB = null;

        if (debug)
        {
            if (!System.String.IsNullOrEmpty(stdout))
            {
                UnityEngine.Debug.Log(stdout);
            }
            if (!System.String.IsNullOrEmpty(stderr))
            {
                UnityEngine.Debug.Log(stderr);
            }
        }

        m_proc = null;

        System.EventHandler<System.EventArgs> handler = OnFinish;
        if (handler != null)
        {
            handler(this, new System.EventArgs());
        }
    }

    void proc_HasExited(object sender, System.EventArgs e)
    {
        End();
    }

    void proc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        m_stderrSB.Append(e.Data);
    }

    void proc_OutDataReceived(object sender, DataReceivedEventArgs e)
    {
        m_stdoutSB.Append(e.Data);
    }

    private event System.EventHandler<System.EventArgs> OnFinish;

    private System.Text.StringBuilder m_stdoutSB;
    private System.Text.StringBuilder m_stderrSB;
    private bool m_async = false;
    private Process m_proc = null;

    public string stdout;
    public string stderr;
    public int ExitCode = 1; // 0 is success
    public bool debug = false;
}

}  // namespace HappyFunTimesEditor

