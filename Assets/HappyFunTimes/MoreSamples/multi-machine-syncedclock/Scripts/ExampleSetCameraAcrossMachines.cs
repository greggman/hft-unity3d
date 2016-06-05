using UnityEngine;
using System.Collections;
using HappyFunTimes;
using DeJson;

public class ExampleSetCameraAcrossMachines : MonoBehaviour
{
    class Options
    {
        // top left offset in rectangle of fullWidth x fullHeight for this instance.
        // width and height will be determined by the size of the canvas.
        public float x = 0;
        public float y = 0;
        public float width = Screen.width;
        public float height = Screen.height;

        // fullWidth and fullHeight define the size in css pixels of the entire display covering how ever
        // many windows/machines you have. For example, if you had 9 machines and each one
        // had a display 1024x768 and you put them all next to each other in a 3x3 grid
        // the fullWidth would be 1024*3 or 3072 and the fullHeight would be 768*3 or 2304
        public float fullWidth  = 1000f;
        public float fullHeight = 1000f;

        // this is the field of view for fullHeight in degrees.
        public float fieldOfView = 60f;

        public float zNear = 0.3f;
        public float zFar = 1000;

        // Set to use Screen.width and Screen.height live
        // Why would not set this? Because your different
        // machines different monitors have a different
        // DPI. In that case to get the images to match
        // you'd need to use different sizes.
        public bool useScreenSize = true;

        // this allows us to pass in JSON. See below
        public string args = "";
    }

    class FovMsg {
        public FovMsg() {}  // for deserailization
        public FovMsg(float fov)
        {
            this.fov = fov;
        }
        public float fov;
    }


    // Use this for initialization
    void Start ()
    {
        HFTArgParser p = HFTArgParser.GetInstance();
        string argStr = "";
        if (p.TryGet<string> ("mm-args", ref argStr))
        {
            Deserializer d = new Deserializer();
            m_options = d.Deserialize<Options>(argStr);
        }
        else
        {
            m_options = new Options();
        }

        // This will read command line arguments
        // prefixed with `--mm` for each of the fields
        // in Options so for example `--mm-full-width=1280`
        // It will also except environment variables
        // like `MM_FULL_WIDTH=1280`. If any argument
        // that starts with `-mm` does not match a field
        // it will print an error. Unfortunately
        // there's no way to get those errors back to
        // the OSX/Windows console.
        //
        // camelCaseField is converted to `--mm-camel-case-field`
        // for arguments and `MM_CAMEL_CASE_FIELD` for environment
        // variables.
        if (!HFTArgsToFields.Apply("mm", m_options))
        {
            Debug.Log("could not parse arguments");
        }

        HFTNoPlayers noPlayers = GetComponent<HFTNoPlayers>();
        m_gameServer = noPlayers.GameServer;

        // call HandleFovChange when an fov message arrives
        m_gameServer.RegisterCmdHandler<FovMsg>("fov", HandleFovChange);

        if (!Screen.fullScreen)
        {
            Screen.SetResolution((int)m_options.width, (int)m_options.height, false);
        }

        UpdateCamera();
    }

    Matrix4x4 MakeFrustum(float left, float right, float bottom, float top, float near, float far)
    {
        var x = 2 * near / ( right - left );
        var y = 2 * near / ( top - bottom );

        var a = ( right + left ) / ( right - left );
        var b = ( top + bottom ) / ( top - bottom );
        var c = - ( far + near ) / ( far - near );
        var d = - 2 * far * near / ( far - near );

        Matrix4x4 m = new Matrix4x4();

        m[0] = x;  m[4] = 0;  m[ 8] = a;   m[12] = 0;
        m[1] = 0;  m[5] = y;  m[ 9] = b;   m[13] = 0;
        m[2] = 0;  m[6] = 0;  m[10] = c;   m[14] = d;
        m[3] = 0;  m[7] = 0;  m[11] = - 1; m[15] = 0;

        return m;
    }

    void Update()
    {
        if (m_options.useScreenSize)
        {
            if (Screen.width  != m_options.width ||
                Screen.height != m_options.height)
            {
                m_options.width = Screen.width;
                m_options.height = Screen.height;

                UpdateCamera();
            }
        }
    }

    void UpdateCamera()
    {
        ///
        /// Computes a sub frustum as an offset in a larger virtual
        /// display. This is useful for multi-window or
        /// multi-monitor/multi-machine setups.
        ///
        /// For example, if you have 3x2 monitors and each monitor is 1920x1080 and
        /// the monitors are in grid like this
        ///
        ///   +---+---+---+
        ///   | A | B | C |
        ///   +---+---+---+
        ///   | D | E | F |
        ///   +---+---+---+
        ///
        /// then for all machines you'd set fullWidth and fullHeight to
        /// 5760x2160 and for each monitor you'd set for example
        ///
        ///    monitor A: x = 0, y = 0, width = 1980, height = 1080
        ///    monitor B: x = 1920, y = 0, width = 1980, height = 1080
        ///    monitor C: x = 3840, y = 0, width = 1980, height = 1080
        ///    monitor D: x = 0, y = 1080, width = 1980, height = 1080
        ///    monitor E: x = 1920, y = 1080, width = 1980, height = 1080
        ///    monitor G: x = 3840, y = 1080, width = 1980, height = 1080
        ///
        ///  Note there is no reason monitors have to be the same size
        ///  or in a grid it's just easier to explain that way.
        ///

        float aspect = m_options.fullWidth / m_options.fullHeight;
        float top = Mathf.Tan(Mathf.Deg2Rad * m_options.fieldOfView * 0.5f) * m_options.zNear;
        float bottom = - top;
        float left = aspect * bottom;
        float right = aspect * top;
        float width = Mathf.Abs(right - left);
        float height = Mathf.Abs(top - bottom);

        Matrix4x4 m = MakeFrustum(
            left + m_options.x * width / m_options.fullWidth,
            left + (m_options.x + m_options.width) * width / m_options.fullWidth,
            top - (m_options.y + m_options.height) * height / m_options.fullHeight,
            top - m_options.y * height / m_options.fullHeight,
            m_options.zNear,
            m_options.zFar
        );
        Camera.main.projectionMatrix = m;
    }

    void HandleFovChange(FovMsg data, string otherGameId)
    {
        // If we're currently dragging the slider
        // we don't want to muck with its value
        if (otherGameId != m_gameServer.Id)
        {
            m_options.fieldOfView = data.fov;
        }

        UpdateCamera();
    }

    void OnGUI()
    {
        float oldFieldOfView = m_options.fieldOfView;
        GUILayout.BeginArea (new Rect (15, 15, 200, 60));
        GUILayout.BeginVertical();
        GUILayout.Box("fov: " + Mathf.Round(m_options.fieldOfView));
        m_options.fieldOfView = GUILayout.HorizontalSlider (m_options.fieldOfView, 10.0f, 170.0f);
        GUILayout.EndVertical();
        GUILayout.EndArea();

        // Only send if it changed
        if (oldFieldOfView != m_options.fieldOfView)
        {
            // broadcast the new fov to all games
            m_gameServer.BroadcastCmdToGames("fov", new FovMsg(m_options.fieldOfView));
        }

        // NOTICE we don't set the camera's fov here.
        // we just broadcast the messages. We'll get that
        // message back ourselves.
    }

    Options m_options;
    GameServer m_gameServer;
}
