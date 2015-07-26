using HappyFunTimes;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HFTSounds {
    public class Sound {
    };

    public class SoundFile : Sound {
      public SoundFile(string _filename) {
        filename = _filename;
      }
      public string filename;
    };

    public class SoundJSFX : Sound {
      public SoundJSFX(
          string generator,
          float superSamplingQuality,
          float masterVolume,
          float attackTime,
          float sustainTime,
          float sustainPunch,
          float decayTime,
          float minFrequency,
          float maxFrequency,
          float slide,
          float deltaSlide,
          float vibratoDepth,
          float vibratoFrequency,
          float vibratoDepthSlide,
          float vibratoFrequencySlide,
          float changeAmount,
          float changeSpeed,
          float squareDuty,
          float squareDutySweep,
          float repeatSpeed,
          float phaserSweep,
          float lpFilterCutoff,
          float lpFilterCutoffSweep,
          float lpFilterResonance,
          float hpFilterCutoff,
          float hpFilterCutoffSweep) {
        Init(
            generator,
            superSamplingQuality,
            masterVolume,
            attackTime,
            sustainTime,
            sustainPunch,
            decayTime,
            minFrequency,
            maxFrequency,
            slide,
            deltaSlide,
            vibratoDepth,
            vibratoFrequency,
            vibratoDepthSlide,
            vibratoFrequencySlide,
            changeAmount,
            changeSpeed,
            squareDuty,
            squareDutySweep,
            repeatSpeed,
            phaserSweep,
            lpFilterCutoff,
            lpFilterCutoffSweep,
            lpFilterResonance,
            hpFilterCutoff,
            hpFilterCutoffSweep);
      }

      private SoundJSFX(
          string generator,
          double superSamplingQuality,
          double masterVolume,
          double attackTime,
          double sustainTime,
          double sustainPunch,
          double decayTime,
          double minFrequency,
          double maxFrequency,
          double slide,
          double deltaSlide,
          double vibratoDepth,
          double vibratoFrequency,
          double vibratoDepthSlide,
          double vibratoFrequencySlide,
          double changeAmount,
          double changeSpeed,
          double squareDuty,
          double squareDutySweep,
          double repeatSpeed,
          double phaserSweep,
          double lpFilterCutoff,
          double lpFilterCutoffSweep,
          double lpFilterResonance,
          double hpFilterCutoff,
          double hpFilterCutoffSweep) {
        Init(
            generator,
            (float)superSamplingQuality,
            (float)masterVolume,
            (float)attackTime,
            (float)sustainTime,
            (float)sustainPunch,
            (float)decayTime,
            (float)minFrequency,
            (float)maxFrequency,
            (float)slide,
            (float)deltaSlide,
            (float)vibratoDepth,
            (float)vibratoFrequency,
            (float)vibratoDepthSlide,
            (float)vibratoFrequencySlide,
            (float)changeAmount,
            (float)changeSpeed,
            (float)squareDuty,
            (float)squareDutySweep,
            (float)repeatSpeed,
            (float)phaserSweep,
            (float)lpFilterCutoff,
            (float)lpFilterCutoffSweep,
            (float)lpFilterResonance,
            (float)hpFilterCutoff,
            (float)hpFilterCutoffSweep);
      }

      void Init(
          string _generator,
          float superSamplingQuality,
          float masterVolume,
          float attackTime,
          float sustainTime,
          float sustainPunch,
          float decayTime,
          float minFrequency,
          float maxFrequency,
          float slide,
          float deltaSlide,
          float vibratoDepth,
          float vibratoFrequency,
          float vibratoDepthSlide,
          float vibratoFrequencySlide,
          float changeAmount,
          float changeSpeed,
          float squareDuty,
          float squareDutySweep,
          float repeatSpeed,
          float phaserSweep,
          float lpFilterCutoff,
          float lpFilterCutoffSweep,
          float lpFilterResonance,
          float hpFilterCutoff,
          float hpFilterCutoffSweep) {
        generator = _generator;
        parameters = new float[25];

        parameters[ 0] = superSamplingQuality;
        parameters[ 1] = masterVolume;
        parameters[ 2] = attackTime;
        parameters[ 3] = sustainTime;
        parameters[ 4] = sustainPunch;
        parameters[ 5] = decayTime;
        parameters[ 6] = minFrequency;
        parameters[ 7] = maxFrequency;
        parameters[ 8] = slide;
        parameters[ 9] = deltaSlide;
        parameters[10] = vibratoDepth;
        parameters[11] = vibratoFrequency;
        parameters[12] = vibratoDepthSlide;
        parameters[13] = vibratoFrequencySlide;
        parameters[14] = changeAmount;
        parameters[15] = changeSpeed;
        parameters[16] = squareDuty;
        parameters[17] = squareDutySweep;
        parameters[18] = repeatSpeed;
        parameters[19] = phaserSweep;
        parameters[20] = lpFilterCutoff;
        parameters[21] = lpFilterCutoffSweep;
        parameters[22] = lpFilterResonance;
        parameters[23] = hpFilterCutoff;
        parameters[24] = hpFilterCutoffSweep;
      }

      public string generator = "";
      public float[] parameters;
    }

    public class Sounds : Dictionary<string, Sound> {

    };
}

