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
using DeJson;
using System.IO;
using System.Collections.Generic;
using HappyFunTimes;

namespace HappyFunTimesEditor {

public class HFTPlatforms : ScriptableObject {
    public bool Windows = true;
    public bool Mac = true;
    public bool Linux = true;

    void OnEnable()
    {
        hideFlags = HideFlags.DontSave;
    }

    public bool Have(string platform)
    {
        platform = platform.ToLowerInvariant();
        if (platform == "win")
        {
            return Windows;
        }
        if (platform == "mac")
        {
            return Mac;
        }
        if (platform == "linux")
        {
            return Linux;
        }
        return false;
    }

    public bool HaveAtLeastOne()
    {
        return Windows || Mac || Linux;
    }

    public void None()
    {
        Windows = false;
        Mac = false;
        Linux = false;
    }
}

public class HFTExport
{
    public class BuildInfo {
        public BuildInfo(string plat, string zip, string bin, string dir, BuildTarget target)
        {
            platform = plat;
            zipSuffix = zip;
            binSuffix = bin;
            dirSuffix = dir;
            unityTarget = target;
        }

        public string platform;
        public string zipSuffix;
        public string binSuffix;
        public string dirSuffix;
        public BuildTarget unityTarget;
    }

    public static List<BuildInfo> GetBuildInfos()
    {
        List<BuildInfo> buildInfos = new List<BuildInfo>();
        buildInfos.Add(new BuildInfo("win", "-win.zip", "-win.exe", "-win_Data", BuildTarget.StandaloneWindows));
        buildInfos.Add(new BuildInfo("mac", "-osx.zip", "-osx.app", "-osx.app", BuildTarget.StandaloneOSXIntel));
        buildInfos.Add(new BuildInfo("linux", "-linux.zip", "-linux.x86", "-linux_Data", BuildTarget.StandaloneLinux));

        return buildInfos;
    }

    public class ExportInfo {
        public string filename;
        public string platform;
        public string type;
    }

    public class Options {
        public Options(HFTPlatforms _platforms)
        {
            platforms = _platforms;
        }
        public HFTPlatforms platforms;
    }

    public bool Export(Options options) {
        if (!options.platforms.HaveAtLeastOne())
        {
            throw new System.ArgumentException("no platforms selected");
        }
        errors = "";
        exportInfos = null;
        exportInfoDict.Clear();

        HFTPackage package = new HFTPackage();
        package.Init();

        string gameId = package.GetString("gameId");
        string workPath = Path.GetDirectoryName(Application.dataPath);
        string binPath = Path.Combine(workPath, "bin");
        string basePath = Path.Combine(binPath, gameId);

        if (!Directory.Exists(binPath)) {
            Directory.CreateDirectory(binPath);
        }

        string[] levels = {};
        List<string> platforms = new List<string>();

        int count = 0;
        List<BuildInfo> buildInfos = GetBuildInfos();
        foreach (BuildInfo buildInfo in buildInfos) {
            if (options.platforms.Have(buildInfo.platform)) {
                string outPath = basePath + buildInfo.binSuffix;
                string errMsg = BuildPipeline.BuildPlayer(levels, outPath, buildInfo.unityTarget, BuildOptions.None);
                if (!System.String.IsNullOrEmpty(errMsg)) {
                    errors += errMsg + "\n";
                }
                platforms.Add(buildInfo.platform);
            }
            ++count;
        }

        string platformArgs = System.String.Join(",", platforms.ToArray());

        if (!System.String.IsNullOrEmpty(errors)) {
            return false;
        }

        HFTCmdRunner runner = new HFTCmdRunner();
        runner.HFTRun(new string[] { "make-release", binPath, "--platforms=" + platformArgs, "--json", "--src=" + workPath + ""}, workPath);
        if (runner.ExitCode != 0) {
            errors = runner.stderr;
            return false;
        }

        Deserializer deserializer = new Deserializer();
        exportInfos = deserializer.Deserialize<ExportInfo[]>(runner.stdout);
        foreach (ExportInfo exportInfo in exportInfos) {
            exportInfoDict[exportInfo.platform] = exportInfo;
        }
        return true;
    }

    public string errors = "";
    public ExportInfo[] exportInfos = null;
    public Dictionary<string, ExportInfo> exportInfoDict = new Dictionary<string, ExportInfo>();
}

}  // namespace HappyFunTimesEditor
