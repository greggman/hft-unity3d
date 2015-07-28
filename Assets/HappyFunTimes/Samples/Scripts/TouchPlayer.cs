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

        m_gamepad.OnNameChange += ChangeName;

        SetName(m_gamepad.Name);
        SetColor(m_gamepad.Color);
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

