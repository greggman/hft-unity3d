using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using HappyFunTimes;
using CSSParse;

namespace HappyFunTimesExample {

// This is the same as ExampleSimplePlayer just slightly modified
// to be called from ExampleCharacterSelect
class ExampleCharacterSelectPlayer : MonoBehaviour {
    // Classes based on MessageCmdData are automatically registered for deserialization
    // by CmdName.
    private class MessageColor {
        public MessageColor(Color _color) {
            color = _color;
        }
        public Color color;
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

    int RandInt(int max) {
        return (int)(Random.value * 0.999 * max);
    }

    void Init() {
        if (m_renderer == null) {
            m_renderer = gameObject.GetComponent<Renderer>();
            m_position = gameObject.transform.localPosition;
        }
    }

    // Called by ExampleCharacterSelect using GameObject.SendMessage
    void InitializeFromCharacterSelect(ExampleCharacterSelect.StartInfo startInfo) {
        Init();
        // Save the netplayer object so we can use it send messages to the phone
        m_netPlayer = startInfo.netPlayer;

        // Register handler to call if the player disconnects from the game.
        m_netPlayer.OnDisconnect += Remove;

        // Handle Namechange. Either use the one passed in or make a new one
        m_playerNameManager = startInfo.playerNameManager;
        m_playerNameManager.OnNameChange += ChangeName;

        // Setup events for the different messages.
        m_netPlayer.RegisterCmdHandler<MessageMove>("move", OnMove);

        ExampleCharacterSelectGameSettings settings = ExampleCharacterSelectGameSettings.settings();
        m_position = new Vector3(Random.Range(0.0f, settings.areaWidth), 0, Random.Range(0.0f, settings.areaHeight));
        transform.localPosition = m_position;

        SetName(m_playerNameManager.Name);
        Color color = new Color(Random.value, Random.value, Random.value);
        // make one random component a minimum brightness
        color[RandInt(3)] = Random.value * 0.5f + 0.5f;
        SetColor(color);
    }

    void Start() {
        Init();
    }

    void Update() {
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
        m_netPlayer.SendCmd("color", new MessageColor(color));
    }

    public void OnTriggerEnter(Collider other) {
        // Because of physics layers we can only collide with the goal
        m_netPlayer.SendCmd("scored", new MessageScored(Random.Range(5, 15)));
    }

    private void Remove(object sender, System.EventArgs e) {
        Destroy(gameObject);
    }

    private void OnMove(MessageMove data) {
        ExampleCharacterSelectGameSettings settings = ExampleCharacterSelectGameSettings.settings();
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

