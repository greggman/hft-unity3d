In the <color=blue>LevelManager</color> GameObject you'll find a <color=blue>PlayerSpanwer</color> that spawns
the <color=blue>HappyFunTimes/Samples/Prefabs/TestPlayer</color> prefab each time a player connects.

The <color=blue>TestPlayer</color> prefab has an <color=blue>HFTGamepad</color> component which talks
to the player's phone through <color=blue>Assets/WebPlayerTemplates/HappyFunTimes/controllers/gamepad/controller.js</color>
and allows you choose various styles of controller. Of course you are free to make your own controller.js and do whatever
you want.

There's also a <color=blue>HFTInput</color> component which emulates the built in Unity <color=blue>Input</color> class.

Finally there's a <color=blue>ControllerTestScript</color> that looks at the <color=blue>HFTInput</color> and displays
the various inputs.

Run it, connect a phone by going to <color=magenta>http://happyfuntimes.net</color>, then in Unity find the cloned
<color=blue>TestPlayer</color> prefab in the scene Hierarchy. Select it and you can change various aspects of the
controller from the inspector.

