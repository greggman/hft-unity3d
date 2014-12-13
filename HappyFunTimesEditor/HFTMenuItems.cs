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
    [MenuItem("Window/HappyFunTimes/Docs", false, 1)]
    static void Docs() {
        Application.OpenURL("http://greggman.github.io/happyfuntimes/docs/unitydocs.md");
    }

    [MenuItem("Window/HappyFunTimes/Package Editor", false, 2)]
    static void PackageEditor() {
        HFTWindow.ShowWindow();
    }

    [MenuItem("Window/HappyFunTimes/Examples/Simple Example", false, 3)]
    static void ExampleSimple() {
        Application.OpenURL("http://greggman.github.io/hft-unitysimple/");
    }

    [MenuItem("Window/HappyFunTimes/Examples/2D Platform Example", false, 4)]
    static void Example2DPlatformer() {
        Application.OpenURL("http://greggman.github.io/hft-unity2dplatformer/");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Character Controller Example", false, 5)]
    static void ExampleCharacterController() {
        Application.OpenURL("http://greggman.github.io/hft-unitycharacterexample/");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Mutli Machine Example", false, 6)]
    static void ExampleMultiMachine() {
        Application.OpenURL("http://greggman.github.io/hft-unity-multi-game-example/");
    }

    [MenuItem("Window/HappyFunTimes/HappyFunTimes Installer", false, 7)]
    static void HappyFunTimesInstaller() {
        Application.OpenURL("http://superhappyfuntimes.net/install");
    }

    [MenuItem("Window/HappyFunTimes/SuperHappyFunTimes", false, 8)]
    static void SuperHappyFunTimes() {
        Application.OpenURL("http://superhappyfuntimes.net");
    }

    //[MenuItem("Window/HappyFunTimes/Export", false, 9)]
    //static void Export() {
    //}
    //
    //[MenuItem("Window/HappyFunTimes/Publish Game", false, 10)]
    //static void Publish() {
    //}


}

}  // namespace HappyFunTimes


