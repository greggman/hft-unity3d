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

using HappyFunTimes;
using UnityEngine;
using System;
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
  public const int AXIS_TOUCH0_X = 13;
  public const int AXIS_TOUCH0_Y = 14;
  public const int AXIS_TOUCH1_X = 15;
  public const int AXIS_TOUCH1_Y = 16;
  public const int AXIS_TOUCH2_X = 17;
  public const int AXIS_TOUCH2_Y = 18;
  public const int AXIS_TOUCH3_X = 19;
  public const int AXIS_TOUCH3_Y = 20;
  public const int AXIS_TOUCH4_X = 21;
  public const int AXIS_TOUCH4_Y = 22;
  public const int AXIS_TOUCH5_X = 23;
  public const int AXIS_TOUCH5_Y = 24;
  public const int AXIS_TOUCH6_X = 25;
  public const int AXIS_TOUCH6_Y = 26;
  public const int AXIS_TOUCH7_X = 27;
  public const int AXIS_TOUCH7_Y = 28;
  public const int AXIS_TOUCH8_X = 29;
  public const int AXIS_TOUCH8_Y = 30;
  public const int AXIS_TOUCH9_X = 31;
  public const int AXIS_TOUCH9_Y = 32;

  public const int BUTTON_DPAD0_LEFT = 14;
  public const int BUTTON_DPAD0_RIGHT = 15;
  public const int BUTTON_DPAD0_TOP = 12;
  public const int BUTTON_DPAD0_BOTTOM = 13;
  public const int BUTTON_DPAD1_LEFT = 18;
  public const int BUTTON_DPAD1_RIGHT = 19;
  public const int BUTTON_DPAD1_TOP = 16;
  public const int BUTTON_DPAD1_BOTTOM = 17;
  public const int BUTTON_TOUCH = 18;
  public const int BUTTON_TOUCH0 = 18;
  public const int BUTTON_TOUCH1 = 19;
  public const int BUTTON_TOUCH2 = 20;
  public const int BUTTON_TOUCH3 = 21;
  public const int BUTTON_TOUCH4 = 22;
  public const int BUTTON_TOUCH5 = 23;
  public const int BUTTON_TOUCH6 = 24;
  public const int BUTTON_TOUCH7 = 25;
  public const int BUTTON_TOUCH8 = 26;
  public const int BUTTON_TOUCH9 = 27;

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
      askForName = src.askForName;
      showMenu = src.showMenu;
    }

    // Because I don't understand the whole Equals, GetHashCode C# mess
    // I named this SameValues
    public bool SameValues(ControllerOptions other) {
      return controllerType == other.controllerType &&
             provideOrientation == other.provideOrientation &&
             provideAcceleration == other.provideAcceleration &&
             provideRotationRate == other.provideRotationRate &&
             askForName == other.askForName &&
             showMenu == other.showMenu;
    }

    public ControllerType controllerType = ControllerType.c_1dpad_2button;
    public bool provideOrientation = false;
    public bool provideAcceleration = false;
    public bool provideRotationRate = false;
    public bool askForName = true;
    public bool showMenu = true;
    // NOTE: If you add a field you need to add to the constructor and SameValues above!
  }

  public string playerName;
  public bool autoChooseColor = true;
  public Color color = new Color(0.0f, 1.0f, 0.0f);
  public ControllerOptions controllerOptions;

  HFTGamepad() {
    axes = new float[33];
    buttons = new Button[28];

    for (int ii = 0; ii < buttons.Length; ++ii) {
      buttons[ii] = new Button();
    }
  }

  // For backward compatibility
  // use gamepad.color instead
  public Color Color {
      get {
          return color;
      }
      set {
          color = value;
      }
  }

  // For backward compatibility
  // use gamepad.playerName instead
  public string Name {
    get {
      return playerName;
    }
    set {
      playerName = value;
    }
  }

  public NetPlayer NetPlayer {
    get {
      return m_netPlayer;
    }
  }

  public event System.EventHandler<System.EventArgs> OnNameChange;
  public event Action OnDisconnect;

  [System.NonSerialized]
  public float[] axes;
  [System.NonSerialized]
  public Button[] buttons;

  // Manages the connection between this object and the phone.
  private NetPlayer m_netPlayer;
  private HFTPlayerNameManager m_playerNameManager;
  private static int s_colorCount = 0;

  private Color m_oldColor;
  private ControllerOptions m_oldControllerOptions = new ControllerOptions();

  private class MessageOptions {
    public MessageOptions(ControllerOptions _controllerOptions) {
      controllerOptions = _controllerOptions;
    }

    public ControllerOptions controllerOptions;
  }

  private class MessageButton {
    public int id = 0;
    public bool pressed = false;
  }

  private class MessageColor {
    public MessageColor(Color _color) {
      color = _color;
    }
    public Color color;
  }

  private class MessageDPad {
    public int pad = 0;
    public int dir = -1;
  }

  private class MessageTouch {
    public int id = 0;
    public int x = 0;
    public int y = 0;
  }

  private class MessageOrient {
    public float a = 0.0f;
    public float b = 0.0f;
    public float g = 0.0f;
    public bool abs = false;
  }

  private class MessageAccel {
    public float x = 0.0f;
    public float y = 0.0f;
    public float z = 0.0f;
  }

  private class MessageRot {
    public float a = 0.0f;
    public float b = 0.0f;
    public float g = 0.0f;
  }


  void InitializeNetPlayer(SpawnInfo spawnInfo) {
    m_netPlayer = spawnInfo.netPlayer;
    m_netPlayer.OnDisconnect += HandleDisconnect;

    // Setup events for the different messages.
    m_netPlayer.RegisterCmdHandler<MessageButton>("button", HandleButton);
    m_netPlayer.RegisterCmdHandler<MessageDPad>("dpad", HandleDPad);
    m_netPlayer.RegisterCmdHandler<MessageOrient>("orient", HandleOrient);
    m_netPlayer.RegisterCmdHandler<MessageAccel>("accel", HandleAccel);
    m_netPlayer.RegisterCmdHandler<MessageRot>("rot", HandleRot);
    m_netPlayer.RegisterCmdHandler<MessageTouch>("touch", HandleTouch);

    m_playerNameManager = new HFTPlayerNameManager(m_netPlayer);
    m_playerNameManager.OnNameChange += HandleNameChange;

    // If the controller is showing the player "game full"
    // then tell it can play.
    m_netPlayer.SendCmd("play");
    SendControllerOptions();
    SendColor();
  }

  void Awake()
  {
    if (autoChooseColor)
    {
      SetDefaultColor();
    }
  }

  void OnDestroy() {
    Cleanup();
  }

  void Cleanup() {
    m_netPlayer.OnDisconnect -= HandleDisconnect;
    if (m_playerNameManager != null) {
      m_playerNameManager.Close();
      m_playerNameManager = null;
    }
  }

  void HandleDisconnect(object sender, System.EventArgs e)
  {
    Action handler = OnDisconnect;
    if (handler != null)
    {
      handler();
    }
  }

  void SendColor()
  {
    if (m_netPlayer != null)
    {
      m_netPlayer.SendCmd("color", new MessageColor(color));
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
    color = HFTColorUtils.HSVAToColor(hsva);
  }

  public void ReturnPlayer()
  {
      if (m_netPlayer != null) {
        HFTGamepadHelper.helper.playerSpawner.ReturnPlayer(m_netPlayer);
      }
  }

  void Update()
  {
    // Seems kind of dumb to do it this way but it's easier for users
    if (!m_oldControllerOptions.SameValues(controllerOptions))
    {
      m_oldControllerOptions = new ControllerOptions(controllerOptions);
      SendControllerOptions();
    }

    if (m_oldColor != color)
    {
      m_oldColor = color;
      SendColor();
    }
  }

  void SendControllerOptions()
  {
    if (m_netPlayer != null)
    {
      m_netPlayer.SendCmd("options", new MessageOptions(controllerOptions));
    }
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
    int index = data.id * 2;
    axes[AXIS_TOUCH_X + index] = (float)data.x / 500.0f - 1.0f;
    axes[AXIS_TOUCH_Y + index] = (float)data.y / 500.0f - 1.0f;
  }

  void HandleNameChange(string name) {
    playerName = name;
    System.EventHandler<System.EventArgs> handler = OnNameChange;
    if (handler != null) {
        handler(this, new System.EventArgs());
    }
  }
}

