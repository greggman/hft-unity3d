using HappyFunTimes;
using HFTSounds;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class HFTGlobalSoundHelper : MonoBehaviour {

  public static Sounds GetSounds() {
    return s_sounds;
  }

  void Awake()
  {
      if (s_sounds == null) {
        InitSounds();
      }
  }

  void InitSounds()
  {
    s_sounds = new Sounds();
    string baseFolder = Path.Combine(Path.Combine(Application.dataPath, "WebPlayerTemplates"), "HappyFunTimes");
    string soundFolder = Path.Combine(baseFolder, "sounds");
    if (Directory.Exists(soundFolder)) {
      AddSoundFiles(baseFolder, Directory.GetFiles(soundFolder, "*.mp3"));
      AddSoundFiles(baseFolder, Directory.GetFiles(soundFolder, "*.wav"));
    }
  }

  void AddSoundFiles(string baseFolder, string[] filenames)
  {
    foreach(string filename in filenames)
    {
      string filepath = filename.Substring(baseFolder.Length + 1).Replace("\\", "/");
      s_sounds[Path.GetFileNameWithoutExtension(filename)] = new SoundFile(filepath);
    }
  }

  private static Sounds s_sounds = null;
};

