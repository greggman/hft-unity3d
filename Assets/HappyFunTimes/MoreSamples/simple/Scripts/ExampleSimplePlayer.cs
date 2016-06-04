using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

namespace HappyFunTimesExample {

class ExampleSimplePlayer : MonoBehaviour {
    // Classes based on MessageCmdData are automatically registered for deserialization
    // by CmdName.
    private class MessageColor {
        public string color = "";    // in CSS format rgb(r,g,b)
    };

    private class MessageMove {
        public float x = 0;
        public float y = 0;
    };

    // NOTE: This message is only sent, never received
    // therefore it does not need a no parameter constructor.
    // If you do receive one you'll get an error unless you
    // add a no parameter constructor.
    private class MessageScored {
        public MessageScored(int _points) {
            points = _points;
        }

        public int points;
    }

    void InitializeNetPlayer(SpawnInfo spawnInfo) {
        // Save the netplayer object so we can use it send messages to the phone
        m_netPlayer = spawnInfo.netPlayer;

        // Register handler to call if the player disconnects from the game.
        m_netPlayer.OnDisconnect += Remove;

        // Track name changes
        m_playerNameManager = new HFTPlayerNameManager(m_netPlayer);
        m_playerNameManager.OnNameChange += ChangeName;

        // Setup events for the different messages.
        m_netPlayer.RegisterCmdHandler<MessageMove>("move", OnMove);
        m_netPlayer.RegisterCmdHandler<MessageColor>("color", OnColor);

        ExampleSimpleGameSettings settings = ExampleSimpleGameSettings.settings();
        m_position = new Vector3(UnityEngine.Random.Range(0, settings.areaWidth), 0, UnityEngine.Random.Range(0, settings.areaHeight));
        transform.localPosition = m_position;

        SetName(m_playerNameManager.Name);
    }

    void Start() {
        m_renderer = gameObject.GetComponent<Renderer>();
        m_position = gameObject.transform.localPosition;
        SetColor(new Color(0.0f, 1.0f, 0.0f));
    }

    void Update() {
    }

    void OnDestroy() {
        if (m_playerNameManager != null) {
          m_playerNameManager.Close();
          m_playerNameManager = null;
        }
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
        m_netPlayer.SendCmd("scored", new MessageScored(UnityEngine.Random.Range(5, 15)));
    }

    private void Remove(object sender, EventArgs e) {
        Destroy(gameObject);
    }

    private void OnColor(MessageColor data) {
        SetColor(CSSParse.Style.ParseCSSColor(data.color));
    }

    private void OnMove(MessageMove data) {
        ExampleSimpleGameSettings settings = ExampleSimpleGameSettings.settings();
        m_position.x = data.x * settings.areaWidth;
        m_position.z = settings.areaHeight - (data.y * settings.areaHeight) - 1;  // because in 2D down is positive.

        gameObject.transform.localPosition = m_position;
    }

    private void ChangeName(string name) {
        SetName(name);
    }

    private NetPlayer m_netPlayer;
    private HFTPlayerNameManager m_playerNameManager;
    private Renderer m_renderer;
    private Vector3 m_position;
    private Color m_color;
    private string m_name;
    private GUIStyle m_guiStyle = new GUIStyle();
    private GUIContent m_guiName = new GUIContent("");
    private Rect m_nameRect = new Rect(0,0,0,0);
}

}  // namespace HappyFunTimesExample

