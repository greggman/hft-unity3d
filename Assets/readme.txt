HappyFunTimes Unity Plugin and Example Scene
============================================

HappyFunTimes lets you use your phone as a controller
to control a game. This lets you make unique controllers
or make locally multiplayer games that support tens or 
hundreds of people.

## Docs

Docs and Videos are available from the menus at Window->HappyFunTimes->Docs
or at http://docs.happyfuntimes.net/docs/unity

## Plugin Structure

```
Assets
├── HappyFunTimes
│   ├── HappyFunTimesCore
│   ├── MoreSamples                 (just samples - deletable)
│   ├── Resources                   (required)
│   ├── Samples                     (deletable)
│   ├── Scripts                     (HFTGamepad etc.. see below)
│   ├── Shaders                     (deletable)
│   └── Tests                       (deletable)
├── Plugins                         (required)
└── WebPlayerTemplates
    └── HappyFunTimes
        ├── 3rdparty                (see below)
        ├── controllers
        │   ├── character-select    (deletable)
        │   ├── gamepad
        │   ├── multi-machine       (deletable)
        │   └── simple              (deletable)
        ├── hft                     (required)
        └── sample-ui               (see below)
```

## Stuff marked as See Below

HappyFunTimes is just the part that connects phones to Unity, serves them webpages
and facilitates communication between them. **EVERYTHING ELSE IS A SAMPLE MEANT TO BE MODIFIED**

Included in those samples are the `HFTGamepad` sample. A premade controller that supports
a bunch of different modes. See docs at http://docs.happyfuntimes.net/docs/unity/gamepad.html

To use the HFTGamepad controller in your own projects keep everything about that says "see below".
If you are not using the HFTGamepad but are making your own custom controller then you may
delete `HappyFunTimes/Scripts` except for `HFTPlayerNameManager.cs`. You can also delete
all of `WebPlayerTemplates/HappyFunTimes/3rdparty & controllers`

The samples also include a sample-ui. This code tries to manage the phone, trying to take it fullscreen
if possible. Trying to set the orientation if possible. Displaying a message to the user to rotate the
phone if not possible. It asks the user for a name. It manages the `gear` icon menu. It displays
"Switching Games" when the user is disconnected. Y

ou are free to use that or delete it and write your own. If you write your own
feel free to delete `WebPlayerTemplates/HappyFunTimes/sample-ui` and `HFTPlayerNameManager.cs`



