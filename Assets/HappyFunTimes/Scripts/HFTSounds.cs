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
          float startFrequency,
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
          float phaserOffset,
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
            startFrequency,
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
            phaserOffset,
            phaserSweep,
            lpFilterCutoff,
            lpFilterCutoffSweep,
            lpFilterResonance,
            hpFilterCutoff,
            hpFilterCutoffSweep);
      }

      public SoundJSFX(
          string generator,
          double superSamplingQuality,
          double masterVolume,
          double attackTime,
          double sustainTime,
          double sustainPunch,
          double decayTime,
          double minFrequency,
          double startFrequency,
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
          double phaserOffset,
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
            (float)startFrequency,
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
            (float)phaserOffset,
            (float)phaserSweep,
            (float)lpFilterCutoff,
            (float)lpFilterCutoffSweep,
            (float)lpFilterResonance,
            (float)hpFilterCutoff,
            (float)hpFilterCutoffSweep);
      }

      public SoundJSFX(string _generator, float[] _params) {
        Init(_generator,
             _params[ 0],
             _params[ 1],
             _params[ 2],
             _params[ 3],
             _params[ 4],
             _params[ 5],
             _params[ 6],
             _params[ 7],
             _params[ 8],
             _params[ 9],
             _params[10],
             _params[11],
             _params[12],
             _params[13],
             _params[14],
             _params[15],
             _params[16],
             _params[17],
             _params[18],
             _params[19],
             _params[20],
             _params[21],
             _params[22],
             _params[23],
             _params[24],
             _params[25],
             _params[26]);
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
          float startFrequency,
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
          float phaserOffset,
          float phaserSweep,
          float lpFilterCutoff,
          float lpFilterCutoffSweep,
          float lpFilterResonance,
          float hpFilterCutoff,
          float hpFilterCutoffSweep) {
        generator = _generator;
        parameters = new float[27];

        parameters[ 0] = superSamplingQuality;
        parameters[ 1] = masterVolume;
        parameters[ 2] = attackTime;
        parameters[ 3] = sustainTime;
        parameters[ 4] = sustainPunch;
        parameters[ 5] = decayTime;
        parameters[ 6] = minFrequency;
        parameters[ 7] = startFrequency;
        parameters[ 8] = maxFrequency;
        parameters[ 9] = slide;
        parameters[10] = deltaSlide;
        parameters[11] = vibratoDepth;
        parameters[12] = vibratoFrequency;
        parameters[13] = vibratoDepthSlide;
        parameters[14] = vibratoFrequencySlide;
        parameters[15] = changeAmount;
        parameters[16] = changeSpeed;
        parameters[17] = squareDuty;
        parameters[18] = squareDutySweep;
        parameters[19] = repeatSpeed;
        parameters[20] = phaserOffset;
        parameters[21] = phaserSweep;
        parameters[22] = lpFilterCutoff;
        parameters[23] = lpFilterCutoffSweep;
        parameters[24] = lpFilterResonance;
        parameters[25] = hpFilterCutoff;
        parameters[26] = hpFilterCutoffSweep;
      }

      public string generator = "";
      public float[] parameters;
    }

    public class Sounds : Dictionary<string, Sound> {

    };
}

