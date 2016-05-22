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
using System.Collections;
using System.Collections.Generic;

public class ControllerTestScript : MonoBehaviour {

    private int m_id;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private static Dictionary<int, bool> s_ids= new Dictionary<int, bool>();

    void Start ()
    {
        // Find an empty id;
        bool foo = false;
        for (int ii = 0; ii < 1000; ++ii) {
            if (!s_ids.TryGetValue(ii, out foo)) {
                m_id = ii;
                s_ids[ii] = true;
                break;
            }
        }

        m_gamepad = GetComponent<HFTGamepad>();
        m_hftInput = GetComponent<HFTInput>();

        // Delete ourselves if disconnected
        m_gamepad.OnDisconnect += Remove;

    }

    void Remove()
    {
        s_ids.Remove(m_id);
        Destroy(gameObject);
    }

    void OnGUI()
    {
        int areaWidth = 200;
        int unitWidth = areaWidth / 4;
        int unitHeight = 20;
        int xx = 10 + 110 * m_id;
        int yy = 10;
        GUI.Box(new Rect(xx, 10, areaWidth, unitHeight), m_gamepad.Name);
        yy += unitHeight;
        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "buttons");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.buttons.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.buttons[ii].pressed ? "*" : "");
        }
        yy += unitHeight * ((m_gamepad.buttons.Length + 3) / 4);

        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "axes");
        yy += unitHeight;
        for (int ii = 0; ii < m_gamepad.axes.Length; ++ii) {
            int x = ii % 4;
            int y = ii / 4;
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight), m_gamepad.axes[ii].ToString());
        }

        yy += unitHeight * ((m_gamepad.axes.Length + 3) / 4);

        GUI.Box(new Rect(xx, yy, areaWidth, unitHeight), "touch");
        yy += unitHeight;
        int numTouch = m_hftInput.touchCount;
        unitWidth = areaWidth / 3;
        unitHeight *= 2;
        for (int ii = 0; ii < numTouch; ++ii) {
            int x = ii % 3;
            int y = ii / 3;
            HFTInput.Touch touch = m_hftInput.GetTouch(ii);
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight, unitWidth, unitHeight / 2), touch.phase.ToString());
            GUI.Box(new Rect(xx + x * unitWidth, yy + y * unitHeight + unitHeight / 2, unitWidth, unitHeight / 2), touch.rawPosition.x.ToString() + "," + touch.rawPosition.y.ToString());
        }
    }
}
