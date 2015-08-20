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

namespace HappyFunTimes {

  public struct DirInfo {
    public DirInfo(int _direction, float _dx, float _dy, int _bits, string _symbol) {
      direction = _direction;
      dx = _dx;
      dy = _dy;
      bits = _bits;
      symbol = _symbol;
    }
    public int direction;
    public float dx;
    public float dy;
    public int bits;
    public string symbol;
  }

  public class Direction {
    public const int RIGHT = 0x1;
    public const int LEFT  = 0x2;
    public const int UP    = 0x4;
    public const int DOWN  = 0x8;

    public static void Init() {
      s_dirInfo = new DirInfo[9];
      s_dirInfo[0] = new DirInfo(-1,  0,  0, 0           , "\u2751");
      s_dirInfo[1] = new DirInfo( 0,  1,  0, RIGHT       , "\u2192"); // right
      s_dirInfo[2] = new DirInfo( 1,  1,  1, UP | RIGHT  , "\u2197"); // up-right
      s_dirInfo[3] = new DirInfo( 2,  0,  1, UP          , "\u2191"); // up
      s_dirInfo[4] = new DirInfo( 3, -1,  1, UP | LEFT   , "\u2196"); // up-left
      s_dirInfo[5] = new DirInfo( 4, -1,  0, LEFT        , "\u2190"); // left
      s_dirInfo[6] = new DirInfo( 5, -1, -1, DOWN | LEFT , "\u2199"); // down-left
      s_dirInfo[7] = new DirInfo( 6,  0, -1, DOWN        , "\u2193"); // down
      s_dirInfo[8] = new DirInfo( 7,  1, -1, DOWN | RIGHT, "\u2198"); // down-right
    }

    public static DirInfo GetDirectionInfo(int direction) {
        if (s_dirInfo == null) {
          Init();
        }
        return s_dirInfo[direction + 1];
    }

    private static DirInfo[] s_dirInfo = null;
  }

}

