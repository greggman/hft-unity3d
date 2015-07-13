using HappyFunTimes;
using UnityEngine;
using System.Collections;

public class BirdScript : MonoBehaviour {

    public float maxSpeed = 10;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float jumpForce = 700f;
    public Transform nameTransform;

    // this is the base color of the avatar.
    // we need to know it because we need to know what color
    // the avatar will become after its hsv has been adjusted
    // so we can color the controller and the names above
    // the player.
    public Color baseColor;

    private float m_groundRadius = 0.2f;
    private bool m_grounded = false;
    private bool m_facingRight = true;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2d;
    private Material m_material;
    private HFTGamepad m_gamepad;
    private HFTInput m_hftInput;
    private GUIStyle m_guiStyle = new GUIStyle();
    private GUIContent m_guiName = new GUIContent("");
    private Rect m_nameRect = new Rect(0,0,0,0);
    private string m_playerName;

    private static int m_playerNumber = 0;

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_rigidbody2d = GetComponent<Rigidbody2D>();
        m_material = GetComponent<Renderer>().material;
        m_gamepad = GetComponent<HFTGamepad>();
        m_hftInput = GetComponent<HFTInput>();

        SetColor(m_playerNumber++);
        SetName(m_gamepad.Name);

        // Notify us if the name changes.
        m_gamepad.OnNameChange += ChangeName;
    }

    void Update()
    {
        bool jumpJustPressed = m_hftInput.GetButtonDown("fire1") || Input.GetKeyDown("space");
        // If we're on the ground AND we just pressed jump (or space)
        if (m_grounded && jumpJustPressed)
        {
            m_grounded = false;
            m_animator.SetBool("Ground", m_grounded);
            m_rigidbody2d.AddForce(new Vector2(0, jumpForce));
        }
    }

    void MoveToRandomSpawnPoint()
    {
        // Pick a random spawn point
        int ndx = Random.Range(0, LevelSettings.settings.spawnPoints.Length - 1);
        transform.localPosition = LevelSettings.settings.spawnPoints[ndx].localPosition;
    }

    void SetName(string name)
    {
        m_playerName = name;
        gameObject.name = "Player-" + m_playerName;
        m_guiName = new GUIContent(m_playerName);
        Vector2 size = m_guiStyle.CalcSize(m_guiName);
        m_nameRect.width  = size.x + 12;
        m_nameRect.height = size.y + 5;
    }

    void SetColor(int colorNdx) {
        // Pick a color
        float hueAdjust = (((colorNdx & 0x01) << 5) |
                           ((colorNdx & 0x02) << 3) |
                           ((colorNdx & 0x04) << 1) |
                           ((colorNdx & 0x08) >> 1) |
                           ((colorNdx & 0x10) >> 3) |
                           ((colorNdx & 0x20) >> 5)) / 64.0f;
        float valueAdjust = (colorNdx & 0x20) != 0 ? -0.5f : 0.0f;
        float satAdjust   = (colorNdx & 0x10) != 0 ? -0.5f : 0.0f;

        // get the hsva for the baseColor
        Vector4 hsva = HFTColorUtils.ColorToHSVA(baseColor);

        // adjust that base color by the amount we picked
        hsva.x += hueAdjust;
        hsva.y += satAdjust;
        hsva.z += valueAdjust;

        // now get the adjusted color.
        Color playerColor = HFTColorUtils.HSVAToColor(hsva);

        // Tell the gamepad to change color
        m_gamepad.Color = playerColor;

        // Create a 1 pixel texture for the OnGUI code to draw the label
        Color[] pix = new Color[1];
        pix[0] = playerColor;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        m_guiStyle.normal.background = tex;

        // Set the HSVA material of the character to the color adjustments.
        m_material.SetVector("_HSVAAdjust", new Vector4(hueAdjust, satAdjust, valueAdjust, 0.0f));
    }

    // Update is called once per frame
    void FixedUpdate () {
        // Check if the center under us is touching the ground and
        // pass that info to the Animator
        m_grounded = Physics2D.OverlapCircle(groundCheck.position, m_groundRadius, whatIsGround);
        m_animator.SetBool("Ground", m_grounded);

        // Pass our vertical speed to the animator
        m_animator.SetFloat("vSpeed", m_rigidbody2d.velocity.y);

        // Get left/right input (get both phone and local input)
        float move = m_hftInput.GetAxis("Horizontal") + Input.GetAxis("Horizontal");

        // Pass that to the animator
        m_animator.SetFloat("Speed", Mathf.Abs(move));

        // and move us
        m_rigidbody2d.velocity = new Vector2(move * maxSpeed, m_rigidbody2d.velocity.y);
        if (move > 0 && !m_facingRight) {
            Flip();
        } else if (move < 0 && m_facingRight) {
            Flip();
        }

        if (transform.position.y < LevelSettings.settings.bottomOfLevel.position.y) {
            MoveToRandomSpawnPoint();
        }
    }

    void Flip()
    {
        m_facingRight = !m_facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnGUI()
    {
        // If someone knows a better way to do
        // names in Unity3D please tell me!
        Vector2 size = m_guiStyle.CalcSize(m_guiName);
        Vector3 coords = Camera.main.WorldToScreenPoint(nameTransform.position);
        m_nameRect.x = coords.x - size.x * 0.5f - 5f;
        m_nameRect.y = Screen.height - coords.y;
        m_guiStyle.normal.textColor = Color.black;
        m_guiStyle.contentOffset = new Vector2(4, 2);
        GUI.Box(m_nameRect, m_playerName, m_guiStyle);
    }

    // Called when the user changes their name.
    void ChangeName(object sender, System.EventArgs e)
    {
        SetName(m_gamepad.Name);
    }

}
