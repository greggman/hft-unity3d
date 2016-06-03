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

    [MenuItem("Window/HappyFunTimes/Examples/Simple Example/UnityPackage", false, 4)]
    static void ExampleSimple() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unitysimple");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Simple Example/Docs", false, 4)]
    static void ExampleSimpleDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unitysimple");
    }

    [MenuItem("Window/HappyFunTimes/Examples/2D Platform Example/UnityPackage", false, 5)]
    static void Example2DPlatformer() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity2dplatformer");
    }

    [MenuItem("Window/HappyFunTimes/Examples/2D Platform Example/Docs", false, 5)]
    static void Example2DPlatformerDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unity2dplatformer");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Character Controller Example/UnityPackage", false, 6)]
    static void ExampleCharacterController() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unitycharacterexample");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Character Controller Example/Docs", false, 6)]
    static void ExampleCharacterControllerDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unitycharacterexample");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Device Orientation and Acceleration Example/UnityPackage", false, 7)]
    static void ExampleMultiDeviceOrientationAndAcceleration() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity-deviceorientation-and-accel");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Device Orientation and Acceleration Example/Docs", false, 7)]
    static void ExampleMultiDeviceOrientationAndAccelerationDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unity-deviceorientation-and-accel");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Players in Scene Example/UnityPackage", false, 8)]
    static void ExamplePlayersInScene() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unityplayersinscene");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Players in Scene Example/Docs", false, 8)]
    static void ExamplePlayersInSceneDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unityplayersinscene");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Character Selection/UnityPackage", false, 9)]
    static void ExampleCharacterSelect() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity-character-select");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Character Selection/Docs", false, 9)]
    static void ExampleCharacterSelectDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unity-character-select");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Multi Machine Example/UnityPackage", false, 9)]
    static void ExampleMultiMachine() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity-multi-character-select");
    }

    [MenuItem("Window/HappyFunTimes/Examples/Mutli Machine Example/Docs", false, 10)]
    static void ExampleMultiMachineDocs() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/samples.html?type=docs&owner=greggman&repo=hft-unity-multi-game-example");
    }

    [MenuItem("Window/HappyFunTimes/HappyFunTimes Installer", false, 11)]
    static void HappyFunTimesInstaller() {
        Application.OpenURL("http://docs.happyfuntimes.net/docs/unity/install.html");
    }

    [MenuItem("Window/HappyFunTimes/SuperHappyFunTimes", false, 12)]
    static void SuperHappyFunTimes() {
        Application.OpenURL("http://superhappyfuntimes.net");
    }

    [MenuItem("Window/HappyFunTimes/Package Editor", false, 13)]
    static void PackageEditor() {
        HFTWindow.ShowWindow();
    }

    //[MenuItem("Window/HappyFunTimes/Export", false, 13)]
    //static void Export() {
    //}
    //
    //[MenuItem("Window/HappyFunTimes/Publish Game", false, 14)]
    //static void Publish() {
    //}
}

}  // namespace HappyFunTimesEditor


