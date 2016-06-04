HappyFunTimes Unity Plugin and Example Scenes
=============================================

HappyFunTimes lets you use your phone as a controller
to control a game. This lets you make unique controllers
or make locally multiplayer games that support tens or 
hundreds of people.

## Docs

Docs and Videos are available from the menus at Window->HappyFunTimes->Docs
or at http://docs.happyfuntimes.net/docs/unity

## Samples

There are samples in both `Assets/HappyFunTimes/Samples/Scenes` and `Assets/HappyFunTimes/MoreSamples`
Be sure to check them all out.

## Publishing

Please see this page on certain things you should be aware of if you plan to publish a
commercial game using this plugin.

## Plugin Structure

```
Assets
├── HappyFunTimes
│   ├── HappyFunTimesCore           required
│   ├── MoreSamples                 just samples - deletable
│   ├── Resources                   required
│   ├── Samples                     deletable
│   ├── Scripts                     HFTGamepad etc.. see below
│   ├── Shaders                     deletable
│   └── Tests                       deletable
├── Plugins                         required
└── WebPlayerTemplates
    └── HappyFunTimes
        ├── 3rdparty                see below
        ├── controllers
        │   ├── character-select    deletable
        │   ├── gamepad             see below
        │   ├── multi-machine       deletable
        │   └── simple              deletable
        ├── hft                     required
        └── sample-ui               see below
```

## Stuff marked as See Below

HappyFunTimes is just the part that connects phones to Unity, serves them webpages
and facilitates communication between them. **EVERYTHING ELSE IS A SAMPLE MEANT TO BE MODIFIED**

Included in those samples are the `HFTGamepad` sample. A premade controller that supports
a bunch of different modes. See docs at http://docs.happyfuntimes.net/docs/unity/gamepad.html

To use the HFTGamepad controller in your own projects keep everything about that says "see below".
If you are not using the HFTGamepad but are making your own custom controller then you may
delete `HappyFunTimes/Scripts` except for `HFTPlayerNameManager.cs`. You can also delete
all of `WebPlayerTemplates/HappyFunTimes/3rdparty, gamepad, controllers`

The samples also include a sample-ui. This code tries to manage the phone, trying to take it fullscreen
if possible. Trying to set the orientation if possible. Displaying a message to the user to rotate the
phone if not possible. It asks the user for a name. It manages the `gear` icon menu. It displays
"Switching Games" when the user is disconnected. Y

ou are free to use that or delete it and write your own. If you write your own
feel free to delete `WebPlayerTemplates/HappyFunTimes/sample-ui` and `HFTPlayerNameManager.cs`

## Debugging

If you are making custom controllers I hope I'd provided enough info to get started. For example
if you send a message from the phone to the game and there is no handler assigned for that message
you'll get an error in the console.

That said, if you'd like to see every message passed between the phones and the game you can go to
`Window->HappyFunTimes->Settings` and check `showMessages`. You'll see the raw JSON being passed
for every message.

There is also a `debug` setting. Putting `*` in there will show all kinds of info from various
parts of happyfuntimes. To filter that you can also put specific class names. For example putting
`HFTWebServer` will show only info from the webserer. Putting `HFTGame` will show only messages
from the game. You may put more than one name (eg, `HFTGame HFTWebServer`). You may also put
prefixes (eg. `HFTG*` which would match `HFTGame`, `HFTGameManager` etc...)

## License

HappyFunTimes is licensed under the MIT license.

It also uses the `websocket-sharp` library (MIT)
https://github.com/sta/websocket-sharp

and the `DNS` library (MIT)
https://github.com/kapetan/dns

http://docs.happyfuntimes.net/docs/unity/running-your-own-happyfuntimes-net.html



