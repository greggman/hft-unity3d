In the `LevelManager` GameObject you'll find a `PlayerSpanwer` that spawns
the `HappyFunTimes/Samples/Prefabs/OrientationPlayer` prefab each time a player connects.

The `OrientationPlayer` prefab has an `HFTGamepad` component which talks
to the player's phone through `Assets/WebPlayerTemplates/HappyFunTimes/controllers/gamepad/controller.js`
and allows you choose various styles of controller. Of course you are free to make your own controller.js and do whatever
you want.

There's also a `HFTInput` component which emulates the built in Unity `Input` class.

Finally there's a `PhonePlayerScript` that looks at the `HFTInput` gyro and acceleration
values to orient the phone in the game to match the phone. Shake the phone to move forward.

