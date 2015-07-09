using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

namespace HappyFunTimesExample {

class TouchPlayer : MonoBehaviour {

    void Start() {
        m_gamepad  = GetComponent<HFTGamepad>();
        m_renderer = GetComponent<Renderer>();
        m_position = transform.localPosition;

        m_gamepad.OnNameChange += ChangeName;

        SetName(m_gamepad.Name);
        SetColor(m_gamepad.Color);
    }

    void Update() {
        TouchGameSettings settings = TouchGameSettings.settings();
        float l = 1.0f; //Time.deltaTime * 5.0f;
        float nx = m_gamepad.axes[HFTGamepad.AXIS_TOUCH_X] * 0.5f;        // -0.5 <-> 0.5
        float ny = m_gamepad.axes[HFTGamepad.AXIS_TOUCH_Y] * 0.5f + 0.5f; //    0 <-> 1
        m_position.x = Mathf.Lerp(m_position.x, settings.areaWidth * nx, l);
        m_position.z = Mathf.Lerp(m_position.z, settings.areaHeight - (ny * settings.areaHeight) - 1, l);  // because in 2D down is positive.

        gameObject.transform.localPosition = m_position;
    }

    void OnGUI()
    {
        Vector2 size = m_guiStyle.CalcSize(m_guiName);
        Vector3 coords = Camera.main.WorldToScreenPoint(transform.position);
        m_nameRect.x = coords.x - size.x * 0.5f - 5.0f;
        m_nameRect.y = Screen.height - coords.y - 30.0f;
        GUI.Box(m_nameRect, m_name, m_guiStyle);
    }

    void SetName(string name) {
        m_name = name;
        gameObject.name = "Player-" + m_name;
        m_guiName = new GUIContent(m_name);
        m_guiStyle.normal.textColor = Color.black;
        m_guiStyle.contentOffset = new Vector2(4.0f, 2.0f);
        Vector2 size = m_guiStyle.CalcSize(m_guiName);
        m_nameRect.width  = size.x + 12;
        m_nameRect.height = size.y + 5;
    }

    void SetColor(Color color)
    {
        m_color = color;
        m_renderer.material.color = m_color;
        Color[] pix = new Color[1];
        pix[0] = color;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        m_guiStyle.normal.background = tex;
    }

    public void OnTriggerEnter(Collider other) {
        // Because of physics layers we can only collide with the goal
    }

    private void Remove(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    private void ChangeName(object sender, EventArgs e) {
        SetName(m_gamepad.Name);
    }

    private Renderer m_renderer;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private Vector3 m_position;
    private Color m_color;
    private string m_name;
    private GUIStyle m_guiStyle = new GUIStyle();
    private GUIContent m_guiName = new GUIContent("");
    private Rect m_nameRect = new Rect(0,0,0,0);
}

}  // namespace HappyFunTimesExample

