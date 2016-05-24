In the `LevelManager` GameObject you'll find a `PlayerSpanwer` that spawns
the `HappyFunTimes/Samples/Prefabs/TestPlayer` prefab each time a player connects.

The `TestPlayer` prefab has an `HFTGamepad` component which talks
to the player's phone through `Assets/WebPlayerTemplates/HappyFunTimes/controllers/gamepad/controller.js`
and allows you choose various styles of controller. Of course you are free to make your own controller.js and do whatever
you want.

There's also a `HFTInput` component which emulates the built in Unity `Input` class.

Finally there's a `ControllerTestScript` that looks at the `HFTInput` and displays
the various inputs.

Run it, connect a phone by going to <color=magenta>http://happyfuntimes.net</color>, then in Unity find the cloned
`TestPlayer` prefab in the scene Hierarchy. Select it and you can change various aspects of the
controller from the inspector.

