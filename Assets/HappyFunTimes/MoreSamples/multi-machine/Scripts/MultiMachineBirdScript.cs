using HappyFunTimes;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiMachineBirdScript : MonoBehaviour {

    public float maxSpeed = 10;
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public float jumpForce = 700f;
    public Transform nameTransform;

    // this is the base color of the avatar.
    // we need to know it because we need to know what color
    // the avatar will become after its hsv has been adjusted.
    public Color baseColor;

    private float m_direction = 0.0f;
    private bool m_jumpPressed = false;      // true if currently held down
    private bool m_jumpJustPressed = false;  // true if pressed just now
    private float m_groundRadius = 0.2f;
    private bool m_grounded = false;
    private bool m_facingRight = true;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody2d;
    private Material m_material;
    private GUIStyle m_guiStyle = new GUIStyle();
    private GUIContent m_guiName = new GUIContent("");
    private Rect m_nameRect = new Rect(0,0,0,0);
    private string m_playerName;
    private MessageSetColor m_color;

    // Manages the connection between this object and the phone.
    private NetPlayer m_netPlayer;
    private HFTPlayerNameManager m_playerNameManager;

    // Message when player presses or release jump button
    private class MessageJump
    {
        public bool jump = false;
    }

    // Message when player pressed left or right
    private class MessageMove
    {
        public int dir = 0;  // will be -1, 0, or +1
    }

    // Message to send to phone to tell it the color of the avatar
    // Note that it sends an hue, saturation, value **adjustment**
    // meaning that RGB values are first converted to HSV where H, S, and V
    // are each in the 0 to 1 range. Then this adjustment is added to those 3
    // values. Finally they are converted back to RGB.
    // The min/max values are a hue range. Anything outside that range will
    // not be adjusted.
    private class MessageSetColor
    {
        public MessageSetColor() { }  // for deserialization
        public MessageSetColor(float _h, float _s, float _v, float _min, float _max)
        {
            h = _h;
            s = _s;
            v = _v;
            rangeMin = _min;
            rangeMax = _max;

        }
        public float h; // hue
        public float s; // saturation
        public float v; // value
        public float rangeMin;
        public float rangeMax;
    }

    // Message to send when sending a player to another game
    private class HFTMessageSwitchGame
    {
        public Vector2 pos;
        public Vector2 vel;
        public string name;
        public MessageSetColor color;
        public float dir;
    }

    // Use this for initialization
    void Start ()
    {
        Init();
    }

    void Init() {
        if (m_animator == null) {
            m_animator = GetComponent<Animator>();
            m_rigidbody2d = GetComponent<Rigidbody2D>();
            m_material = GetComponent<Renderer>().material;
        }
    }

    // Called when player connects with their phone
    void InitializeNetPlayer(SpawnInfo spawnInfo)
    {
        Init();

        m_netPlayer = spawnInfo.netPlayer;
        m_netPlayer.OnDisconnect += Remove;

        // Setup events for the different messages.
        m_netPlayer.RegisterCmdHandler<MessageMove>("move", OnMove);
        m_netPlayer.RegisterCmdHandler<MessageJump>("jump", OnJump);

        m_playerNameManager = new HFTPlayerNameManager(m_netPlayer);
        m_playerNameManager.OnNameChange += HandleNameChange;

        // We always get a `data` so check if it has one key from our expected message
        Dictionary<string, object> dict = spawnInfo.data as Dictionary<string, object>;
        if (dict != null && dict.ContainsKey("dir")) {
            // This player was transferred from another game.

            // Turn the data back into our structure
            DeJson.Deserializer deserializer = new DeJson.Deserializer();
            HFTMessageSwitchGame data = deserializer.Deserialize<HFTMessageSwitchGame>(spawnInfo.data);

            // Choose a starting position based on the old position
            float x = (data.pos.x < MultiMachineLevelSettings.settings.leftEdgeOfLevel.position.x)
                ? MultiMachineLevelSettings.settings.rightEdgeOfLevel.position.x - 1 : MultiMachineLevelSettings.settings.leftEdgeOfLevel.position.x + 1;
            transform.localPosition = new Vector3(x, data.pos.y, 0f);

            // Set the initial velocity
            m_rigidbody2d.velocity = data.vel;

            // And the direction
            m_direction = data.dir;

            SetName(data.name);
            SetColor(data.color);
        } else {
            // This player just joined.
            MoveToRandomSpawnPoint();
            SetName(m_playerNameManager.Name);
            float hue = Random.value;
            float sat = (float)Random.Range(0, 3) * -0.25f;
            MessageSetColor color = new MessageSetColor(
                hue,
                sat,
                0.0f,
                m_material.GetFloat("_HSVRangeMin"),
                m_material.GetFloat("_HSVRangeMax"));
            SetColor(color);

            // Send it to the phone
            m_netPlayer.SendCmd("setColor", color);
        }
    }

    void Update()
    {
        // If we're on the ground AND we pressed jump (or space)
        if (m_grounded && (m_jumpJustPressed || Input.GetKeyDown("space")))
        {
            m_grounded = false;
            m_animator.SetBool("Ground", m_grounded);
            m_rigidbody2d.AddForce(new Vector2(0, jumpForce));
        }
        m_jumpJustPressed = false;
    }

    void MoveToRandomSpawnPoint()
    {
        // Pick a random spawn point
        int ndx = Random.Range(0, MultiMachineLevelSettings.settings.spawnPoints.Length - 1);
        transform.localPosition = MultiMachineLevelSettings.settings.spawnPoints[ndx].localPosition;
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

    void SetColor(MessageSetColor color) {
        m_color = color;
        Color[] pix = new Color[1];
        Vector4 hsva = ColorUtils.ColorToHSVA(baseColor);
        hsva.x += color.h;
        hsva.y += color.s;
        hsva.w += color.v;
        pix[0] = ColorUtils.HSVAToColor(hsva);
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixels(pix);
        tex.Apply();
        m_guiStyle.normal.background = tex;
        m_material.SetVector("_HSVAAdjust", new Vector4(color.h, color.s, color.v, 0.0f));
        m_material.SetFloat("_HSVRangeMin", color.rangeMin);
        m_material.SetFloat("_HSVRangeMax", color.rangeMax);
    }

    void Remove(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
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
        float move = m_direction + Input.GetAxis("Horizontal");

        // Pass that to the animator
        m_animator.SetFloat("Speed", Mathf.Abs(move));

        // and move us
        m_rigidbody2d.velocity = new Vector2(move * maxSpeed, m_rigidbody2d.velocity.y);
        if (move > 0 && !m_facingRight) {
            Flip();
        } else if (move < 0 && m_facingRight) {
            Flip();
        }

        if (transform.position.x < MultiMachineLevelSettings.settings.leftEdgeOfLevel.position.x) {
            SwitchGame(-1);
        } else if (transform.position.x > MultiMachineLevelSettings.settings.rightEdgeOfLevel.position.x) {
            SwitchGame(1);
        }
        if (transform.position.y < MultiMachineLevelSettings.settings.bottomOfLevel.position.y) {
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

    void SwitchGame(int dir)
    {
        HFTMessageSwitchGame data = new HFTMessageSwitchGame();
        data.pos = transform.position;
        data.vel = m_rigidbody2d.velocity;
        data.name = m_playerName;
        data.color = m_color;
        data.dir = m_direction;

        // Look up the id of the game we are in. Something like "level3"
        string gameId = MultiMachineLevelSettings.settings.playerSpawner.server.Id;
        // Extract the number part
        int numNdx = gameId.IndexOfAny("0123456789".ToCharArray());
        int gameNum = System.Convert.ToInt32(gameId.Substring(numNdx));
        // Increment or Decrement the number wraping at the ends
        gameNum = (gameNum + MultiMachineLevelSettings.settings.numGames + dir) % MultiMachineLevelSettings.settings.numGames;

        // Build the ID of the game we want to switch to
        gameId = gameId.Substring(0, numNdx) + gameNum;

        // Send this player to that game.
        // Note: No more messages will be received or sent to this player.
        m_netPlayer.SwitchGame(gameId, data);
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
        Vector3 coords = Camera.main.WorldToScreenPoint(nameTransform.position);
        m_nameRect.x = coords.x - size.x * 0.5f - 5f;
        m_nameRect.y = Screen.height - coords.y;
        m_guiStyle.normal.textColor = Color.black;
        m_guiStyle.contentOffset = new Vector2(4, 2);
        GUI.Box(m_nameRect, m_playerName, m_guiStyle);
    }

    void HandleNameChange(string name)
    {
        SetName(name);
    }

    void OnMove(MessageMove data)
    {
        m_direction = data.dir;
    }

    void OnJump(MessageJump data)
    {
        m_jumpJustPressed = data.jump && !m_jumpPressed;
        m_jumpPressed = data.jump;
    }
}
