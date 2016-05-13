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



using HappyFunTimes;
using HFTSounds;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class HFTGlobalSoundHelper : MonoBehaviour {

  public static Sounds GetSounds() {
    return s_sounds;
  }

  void Awake()
  {
      if (s_sounds == null)
      {
        InitSounds();
      }
  }

  void InitSounds()
  {
    s_sounds = new Sounds();
    string soundFolder = "/";    // This gets all sounds. Should I change it?
    HFTWebFileDB db = HFTWebFileDB.GetInstance();
    AddSoundFiles(soundFolder, db.GetFiles(soundFolder, "*.mp3"));
    AddSoundFiles(soundFolder, db.GetFiles(soundFolder, "*.wav"));
    AddJSFXSounds(db.GetFiles(soundFolder, "*.jsfx.txt"));
  }

  void AddJSFXSounds(string[] filenames)
  {
    foreach(string filename in filenames)
    {
      byte[] bytes = null;
      if (!HFTWebFileDB.GetInstance().GetFile(filename, out bytes))
      {
        Debug.LogError("no such file: " + filename);
      }
      string content = System.Text.Encoding.UTF8.GetString(bytes);
      string[] lines = content.Split(s_lineDelims, System.StringSplitOptions.None);
      int lineNo = 0;
      foreach (string lineStr in lines)
      {
        ++lineNo;
        string line = lineStr.Split('#')[0].Split('/')[0].Split(';')[0].Trim();
        if (line.Length == 0)
        {
          continue;
        }

        // TODO remove comments
        Match m = s_jsfxRE.Match(line);
        if (!m.Success)
        {
          Debug.LogError(filename + " line: " + lineNo + " could not parse line");
          continue;
        }
        string name = m.Groups[1].Value;
        string generator = m.Groups[2].Value;
        string numbersString = m.Groups[3].Value;
        string[] numberStrings = numbersString.Split(',');
        if (numberStrings.Length != 27)
        {
          Debug.LogError(filename + " line:" + lineNo + " expected 27 values found " + numberStrings.Length);
          continue;
        }

        float[] parameters = new float[27];
        int i = 0;
        bool error = false;
        foreach (string numstr in numberStrings)
        {
          try
          {
            parameters[i] = float.Parse(numstr, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
          }
          catch (System.Exception)
          {
            Debug.LogError(filename + " line:" + lineNo + " could not parse number " + numstr);
            error = true;
          }
          ++i;
        }

        if (error)
        {
          continue;
        }
        s_sounds[name] = new SoundJSFX(generator, parameters);
      }
    }
  }

  void AddSoundFiles(string folder, string[] filenames)
  {
    foreach(string filename in filenames)
    {
      string filepath = Path.Combine(folder, filename.Replace("\\", "/"));
      s_sounds[Path.GetFileNameWithoutExtension(filename)] = new SoundFile(filepath);
    }
  }

  private static Sounds s_sounds = null;
  private static Regex s_jsfxRE = new Regex(@"(\w+)\s*?\[""(\w+)""\s*?,(.*?)\]");
  private static string[] s_lineDelims = new string[] { "\r\n", "\n" };
};

