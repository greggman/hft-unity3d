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

namespace HappyFunTimesEditor {

public class HFTMenuItems {
    [MenuItem ("Window/HappyFunTimes/Settings", false, 1)]
    static void HappyFunTimesWindow () {
        ScriptableObject s = HappyFunTimes.HFTHappyFunTimesSettings.GetInstance();
        Selection.objects = new UnityEngine.Object[] { s };
    }

    [MenuItem("Window/HappyFunTimes/Docs", false, 2)]
    static void Docs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity");
    }

    [MenuItem("Window/HappyFunTimes/Troubleshooting", false, 2)]
    static void HappyFunTimesTroubleShooting() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/troubleshooting.html");
    }

    [MenuItem("Window/HappyFunTimes/Support", false, 3)]
    static void HappyFunTimesSupport() {
        Application.OpenURL("http://github.com/greggman/HappyFunTimes/issues");
    }

    [MenuItem("Window/HappyFunTimes/SuperHappyFunTimes", false, 12)]
    static void SuperHappyFunTimes() {
        Application.OpenURL("http://superhappyfuntimes.net");
    }
}

}  // namespace HappyFunTimesEditor


