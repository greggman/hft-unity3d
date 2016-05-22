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
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

namespace HappyFunTimesExample {

class TouchPlayer : MonoBehaviour
{
    void Start()
    {
        m_gamepad  = GetComponent<HFTGamepad>();
        m_renderer = GetComponent<Renderer>();
        m_position = transform.localPosition;

        m_text = transform.FindChild("NameUI/Name").gameObject.GetComponent<UnityEngine.UI.Text>();
        m_rawImage = transform.FindChild("NameUI/NameBackground").gameObject.GetComponent<UnityEngine.UI.RawImage>();
        m_rawImage.material = (Material)Instantiate(m_rawImage.material);

        // Notify us if the name changes.
        m_gamepad.OnNameChange += ChangeName;

        // Delete ourselves if disconnected
        m_gamepad.OnDisconnect += Remove;

        SetName(m_gamepad.Name);
        SetColor(m_gamepad.Color);
    }

    void Remove()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        TouchGameSettings settings = TouchGameSettings.settings();
        float l = 1.0f; //Time.deltaTime * 5.0f;
        float nx = m_gamepad.axes[HFTGamepad.AXIS_TOUCH_X] * 0.5f;        // -0.5 <-> 0.5
        float ny = m_gamepad.axes[HFTGamepad.AXIS_TOUCH_Y] * 0.5f + 0.5f; //    0 <-> 1
        m_position.x = Mathf.Lerp(m_position.x, settings.areaWidth * nx, l);
        m_position.z = Mathf.Lerp(m_position.z, settings.areaHeight - (ny * settings.areaHeight) - 1, l);  // because in 2D down is positive.

        gameObject.transform.localPosition = m_position;
    }

    void SetName(string name)
    {
        m_name = name;
        gameObject.name = "Player-" + m_name;
        m_text.text = name;
    }

    void SetColor(Color color)
    {
        m_color = color;
        m_renderer.material.color = m_color;
        m_rawImage.material.color = m_color;
    }

    public void OnTriggerEnter(Collider other)
    {
        // Because of physics layers we can only collide with the goal
    }

    private void Remove(object sender, EventArgs e)
    {
        Destroy(gameObject);
    }

    private void ChangeName(object sender, EventArgs e)
    {
        SetName(m_gamepad.Name);
    }

    private Renderer m_renderer;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private UnityEngine.UI.Text m_text;
    private UnityEngine.UI.RawImage m_rawImage;
    private Vector3 m_position;
    private Color m_color;
    private string m_name;
}

}  // namespace HappyFunTimesExample

