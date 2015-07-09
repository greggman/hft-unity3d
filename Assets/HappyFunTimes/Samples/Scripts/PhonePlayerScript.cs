using UnityEngine;
using System.Collections;

public class PhonePlayerScript : MonoBehaviour {

    public float rotationSpeed = 5.0f;
    public float moveSpeed = 5.0f;
    public float moveFriction = 0.95f;
    public float shakeThreshold = 20.0f;
    private Renderer m_renderer;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private Color m_color;
    private string m_name;
    private GUIStyle m_guiStyle = new GUIStyle();
    private GUIContent m_guiName = new GUIContent("");
    private Rect m_nameRect = new Rect(0,0,0,0);
    private float speed = 0.0f;

    private static int s_playerCount = 0;


    // Use this for initialization
    void Start () {
        m_renderer = GetComponent<Renderer>();
        m_gamepad = GetComponent<HFTGamepad>();
        m_hftInput = GetComponent<HFTInput>();

        int playerNdx = s_playerCount++;
        transform.position = new Vector3(
            CenterOut(playerNdx % 9)     * 2.5f,
            CenterOut(playerNdx / 9 % 5) * 2.5f,
            transform.position.z);

        SetName(m_gamepad.Name);
        SetColor(m_gamepad.Color);

        // Notify us if the name changes.
        m_gamepad.OnNameChange += ChangeName;
    }

    // Update is called once per frame
    void Update () {
        Quaternion q = Quaternion.Slerp(transform.rotation, m_hftInput.gyro.attitude, rotationSpeed * Time.deltaTime);
        transform.rotation = q;

        if (Mathf.Abs(m_hftInput.gyro.userAcceleration.z) > shakeThreshold) {
            speed = moveSpeed;
        }
        speed = speed * moveFriction;

        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    private float CenterOut(int v) {
        if (v == 0) {
            return (float)v;
        }
        return (float)((v + 1) / 2 * ((v & 1) == 0 ? 1 : -1));
    }

    private void SetColor(Color color) {
        m_color = color;
        m_renderer.material.color = m_color;
        Color[] pix = new Color[1];
        pix[0] = color;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        m_guiStyle.normal.background = tex;
    }

    void OnGUI()
    {
        Vector2 size = m_guiStyle.CalcSize(m_guiName);
        Vector3 coords = Camera.main.WorldToScreenPoint(transform.position);
        m_nameRect.x = coords.x - size.x * 0.5f - 5.0f;
        m_nameRect.y = Screen.height - coords.y - 40.0f;
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

    // Called when the user changes their name.
    void ChangeName(object sender, System.EventArgs e)
    {
        SetName(m_gamepad.Name);
    }
}
