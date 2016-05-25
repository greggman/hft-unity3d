In the `LevelManager` GameObject you'll find a `PlayerSpanwer` that spawns
the `HappyFunTimes/Samples/Prefabs/Character` prefab each time a player connects.

The `Character` prefab has an `HFTGamepad` component which talks
to the player's phone through `Assets/WebPlayerTemplates/HappyFunTimes/controllers/gamepad/controller.js`
and allows you choose various styles of controller. Of course you are free to make your own controller.js and do whatever
you want.

There's also a `HFTInput` component which emulates the built in `Input` class.

Finally there's a `BirdScript` component which controls each player's character. It interacts
with the `HFTInput` component to control the player as well as the `HFTGamepad`
component to let the game set the color of the controller and get the user's name.

[For more info about this sample see http://docs.happyfuntimes.net/docs/unity/gamepad.html](http://docs.happyfuntimes.net/docs/unity/gamepad.html).

