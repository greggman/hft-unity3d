HappyFunTimes-Unity3D
=====================

This is a sample Unity3D game for the [HappyFunTimes party games system](http://docs.happyfuntimes.net).

## Install

[Clear here to get it from the unity asset store](http://assetstore.unity3d.com/en/#!/content/19668).

## What is it?

This one emulates a bunch of simple controllers. This allows you to get started with HappyFunTimes in
your Unity project very quickly.

<img src="Assets/WebPlayerTemplates/HappyFunTimes/screenshot.png" />

Samples include a 2D platformer example inspired by [this tutorial from the unity website](https://unity3d.com/learn/tutorials/modules/beginner/2d)
unity website but with HappyFunTimes control added. There is also an example showing using orientation and
another example showing using the phone as a touch pad.

## Docs

[Some more docs can be found here](http://docs.happyfuntimes.net/docs/unity/gamepad.html).

You can get this from the [Unity Asset Store](http://u3d.as/content/greggman/happy-fun-times)
or you can [download it directly here](http://docs.happyfuntimes.net/docs/unity/samples.html?owner=greggman&repo=hft-unity-gamepad)

## Changelist

### 1.2

*   added HFTGamepad.BUTTON_TOUCH for whether or not the user is touching the screen
    on the orientation and touch controllers.

*   added ability to play sound through controller

*   added mulit-touch support to HFTInput via the standard `GetTouch` function
    as well as `axes` and `buttons`

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

    Note: The Touch.rawPosition is currently in screen pixels of Unity
    not the controller. It's not clear what the best way to handle this
    is.

    The Unity `Input` API says those value are in pixels but they are
    assuming the game is running on the phone. In the case of HappyFunTimes
    though each phone is different so having it be in phone screen pixels
    would make no sense unless you also knew the resolution of each phone.
    I could provide that but that would make it more complicated for you.

    Personally I'd prefer normalized values (0.0 to 1.0). If you want those
    then take  `Touch.rawPosition` and divide `x` by `Screen.width` and `y` by `Screen.height`
    as in

        HFTInput.Touch touch = m_hftInput.GetTouch(0);
        float normalizedX = touch.x / Screen.width;
        float normalziedY = touch.y / Screen.height;

    Also note the AXIS versions of these are already normalized to
    a -1.0 to +1.0 range.


