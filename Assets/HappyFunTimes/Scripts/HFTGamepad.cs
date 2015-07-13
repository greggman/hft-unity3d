using HappyFunTimes;
using UnityEngine;
using System.Collections;

public class HFTGamepad : MonoBehaviour {

  public const int AXIS_DPAD0_X = 0;
  public const int AXIS_DPAD0_Y = 1;
  public const int AXIS_DPAD1_X = 0;
  public const int AXIS_DPAD1_Y = 1;
  public const int AXIS_ORIENTATION_ALPHA = 4;
  public const int AXIS_ORIENTATION_BETA  = 5;
  public const int AXIS_ORIENTATION_GAMMA = 6;
  public const int AXIS_ACCELERATION_X = 7;
  public const int AXIS_ACCELERATION_Y = 8;
  public const int AXIS_ACCELERATION_Z = 9;
  public const int AXIS_ROTATION_RATE_ALPHA = 10;
  public const int AXIS_ROTATION_RATE_BETA  = 11;
  public const int AXIS_ROTATION_RATE_GAMMA = 12;
  public const int AXIS_TOUCH_X = 13;
  public const int AXIS_TOUCH_Y = 14;

  public const int BUTTON_DPAD0_LEFT = 14;
  public const int BUTTON_DPAD0_RIGHT = 15;
  public const int BUTTON_DPAD0_TOP = 12;
  public const int BUTTON_DPAD0_BOTTOM = 13;
  public const int BUTTON_DPAD1_LEFT = 18;
  public const int BUTTON_DPAD1_RIGHT = 19;
  public const int BUTTON_DPAD1_TOP = 16;
  public const int BUTTON_DPAD1_BOTTOM = 17;

  public class Button {
    public bool pressed = false;
    public float value = 0.0f;
  }

  public enum ControllerType {
    c_1button,
    c_2button,
    c_1dpad_1button,
    c_1dpad_2button,
    c_1dpad,
    c_2dpad,
    c_1lrpad_1button,
    c_1lrpad_2button,
    c_1lrpad,
    c_touch,
    c_orient,
  }

  [System.Serializable]
  public class ControllerOptions {
    public ControllerOptions() {
    }

    public ControllerOptions(ControllerOptions src) {
      controllerType = src.controllerType;
      provideOrientation = src.provideOrientation;
      provideAcceleration = src.provideAcceleration;
      provideRotationRate = src.provideRotationRate;
    }

    // Because I don't understand the whole Equals, GetHashCode C# mess
    // I named this SameValues
    public bool SameValues(ControllerOptions other) {
      return controllerType == other.controllerType &&
             provideOrientation == other.provideOrientation &&
             provideAcceleration == other.provideAcceleration &&
             provideRotationRate == other.provideRotationRate;
    }

    public ControllerType controllerType = ControllerType.c_1dpad_2button;
    public bool provideOrientation = false;
    public bool provideAcceleration = false;
    public bool provideRotationRate = false;
  }

  public ControllerOptions controllerOptions;

  HFTGamepad() {
    for (int ii = 0; ii < buttons.Length; ++ii) {
      buttons[ii] = new Button();
    }
  }

  public Color Color {
      get {
          return m_color;
      }
      set {
          m_color = value;
          SendColor();
      }
  }

  public string Name {
    get {
      return m_netPlayer.Name;
    }
  }

  public NetPlayer NetPlayer {
    get {
      return m_netPlayer;
    }
  }

  public event System.EventHandler<System.EventArgs> OnNameChange;

  [HideInInspector]
  public float[] axes = new float[15];
  [HideInInspector]
  public Button[] buttons = new Button[18];

  // Manages the connection between this object and the phone.
  private NetPlayer m_netPlayer;
  private Color m_color = new Color(0.0f, 1.0f, 0.0f);
  private static int s_colorCount = 0;

  private ControllerOptions m_oldControllerOptions = new ControllerOptions();

  private class MessageOptions : MessageCmdData {
    public MessageOptions(ControllerOptions _controllerOptions) {
      controllerOptions = _controllerOptions;
    }

    public ControllerOptions controllerOptions;
  }

  private class MessageButton : MessageCmdData {
    public int id = 0;
    public bool pressed = false;
  }

  private class MessageColor : MessageCmdData {
    public MessageColor(Color _color) {
      color = _color;
    }
    public Color color;
  }

  private class MessageDPad : MessageCmdData {
    public int pad = 0;
    public int dir = -1;
  }

  private class MessageTouch : MessageCmdData {
    public int x = 0;
    public int y = 0;
  }

  private class MessageOrient : MessageCmdData {
    public float a = 0.0f;
    public float b = 0.0f;
    public float g = 0.0f;
    public bool abs = false;
  }

  private class MessageAccel : MessageCmdData {
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
  }

  private class MessageRot : MessageCmdData {
    public float a = 0.0f;
    public float b = 0.0f;
    public float g = 0.0f;
  }

  void InitializeNetPlayer(SpawnInfo spawnInfo) {
    m_netPlayer = spawnInfo.netPlayer;
    m_netPlayer.OnDisconnect += Remove;

    // Setup events for the different messages.
    m_netPlayer.RegisterCmdHandler<MessageButton>("button", HandleButton);
    m_netPlayer.RegisterCmdHandler<MessageDPad>("dpad", HandleDPad);
    m_netPlayer.RegisterCmdHandler<MessageOrient>("orient", HandleOrient);
    m_netPlayer.RegisterCmdHandler<MessageAccel>("accel", HandleAccel);
    m_netPlayer.RegisterCmdHandler<MessageRot>("rot", HandleRot);
    m_netPlayer.RegisterCmdHandler<MessageTouch>("touch", HandleTouch);

    m_netPlayer.OnNameChange += ChangeName;

    // If the controller is showing the player "game full"
    // then tell it can play.
    m_netPlayer.SendCmd("play");
    SendControllerOptions();
    SendColor();
  }

  void Awake() {
    SetDefaultColor();
  }

  void SendColor() {
    if (m_netPlayer != null) {
      m_netPlayer.SendCmd("color", new MessageColor(m_color));
    }
  }

  void SetDefaultColor() {
    int colorNdx = s_colorCount++;

    // Pick a color
    float hue = (((colorNdx & 0x01) << 5) |
                 ((colorNdx & 0x02) << 3) |
                 ((colorNdx & 0x04) << 1) |
                 ((colorNdx & 0x08) >> 1) |
                 ((colorNdx & 0x10) >> 3) |
                 ((colorNdx & 0x20) >> 5)) / 64.0f;
    float sat   = (colorNdx & 0x10) != 0 ? 0.5f : 1.0f;
    float value = (colorNdx & 0x20) != 0 ? 0.5f : 1.0f;
    float alpha = 1.0f;

    Vector4 hsva = new Vector4(hue, sat, value, alpha);
    m_color = HFTColorUtils.HSVAToColor(hsva);
  }

  void Remove(object sender, System.EventArgs e)
  {
      Destroy(gameObject);
  }

  public void ReturnPlayer() {
      HFTGamepadHelper.helper.playerSpawner.ReturnPlayer(m_netPlayer);
  }

  void Update() {
    // Seems kind of dumb to do it this way but it's easier for users
    if (!m_oldControllerOptions.SameValues(controllerOptions)) {
      m_oldControllerOptions = new ControllerOptions(controllerOptions);
      SendControllerOptions();
    }
  }

  void SendControllerOptions() {
    m_netPlayer.SendCmd("options", new MessageOptions(controllerOptions));
  }

  void UpdateButton(int ndx, bool pressed) {
    Button button = buttons[ndx];
    button.pressed = pressed;
    button.value   = pressed ? 1.0f : 0.0f;
  }

  void HandleButton(MessageButton data) {
    UpdateButton(data.id, data.pressed);
  }

  static int[][] axisButtonMap = new int[][] {
    new int[] { 14, 15, 12, 13, },
    new int[] { 18, 19, 16, 17, },
  };

  void HandleDPad(MessageDPad data) {
    int axisOffset = data.pad * 2;
    int[] buttonIndices = axisButtonMap[data.pad];
    DirInfo dirInfo = Direction.GetDirectionInfo(data.dir);
    UpdateButton(buttonIndices[0], (dirInfo.bits & 0x2) != 0);
    UpdateButton(buttonIndices[1], (dirInfo.bits & 0x1) != 0);
    UpdateButton(buttonIndices[2], (dirInfo.bits & 0x4) != 0);
    UpdateButton(buttonIndices[3], (dirInfo.bits & 0x8) != 0);

    axes[axisOffset + 0] =  dirInfo.dx;
    axes[axisOffset + 1] = -dirInfo.dy;
  }

  void HandleOrient(MessageOrient data) {
    axes[AXIS_ORIENTATION_ALPHA] = data.a; // data.a / 180 - 1;  // range is suspposed to be 0 to 359
    axes[AXIS_ORIENTATION_BETA]  = data.b; // data.b / 180;      // range is suspposed to be -180 to 180
    axes[AXIS_ORIENTATION_GAMMA] = data.g; // data.g / 90;       // range is suspposed to be -90 to 90
  }

  void HandleAccel(MessageAccel data) {
    // These values are supposed to be in meters per second squared but I need to convert them to 0 to 1 values.
    // A quick test seems to make them go to +/- around 50 at least on my iPhone5s but different on my android.
    // Maybe I should keep track of max values and reset over time with some threshold?
    // actually I'm just going to pass them through as is.
    axes[AXIS_ACCELERATION_X] = data.x; //clamp(data.x / maxAcceleration, -1, 1);
    axes[AXIS_ACCELERATION_Y] = data.y; //clamp(data.y / maxAcceleration, -1, 1);
    axes[AXIS_ACCELERATION_Z] = data.z; //clamp(data.z / maxAcceleration, -1, 1);
  }

  void HandleRot(MessageRot data) {
    axes[AXIS_ROTATION_RATE_ALPHA] = data.a;
    axes[AXIS_ROTATION_RATE_BETA]  = data.b;
    axes[AXIS_ROTATION_RATE_GAMMA] = data.g;
  }

  void HandleTouch(MessageTouch data) {
    axes[AXIS_TOUCH_X] = (float)data.x / 500.0f - 1.0f;
    axes[AXIS_TOUCH_Y] = (float)data.y / 500.0f - 1.0f;
  }

  void ChangeName(object sender, System.EventArgs e)
  {
      System.EventHandler<System.EventArgs> handler = OnNameChange;
      if (handler != null) {
          handler(this, e);
      }
  }

}

