In the <color=blue>LevelManager</color> GameObject you'll find a <color=blue>PlayerSpanwer</color> that spawns
the <color=blue>HappyFunTimes/Samples/Prefabs/Character</color> prefab each time a player connects.

The <color=blue>Character</color> prefab has an <color=blue>HFTGamepad</color> component which talks
to the player's phone through <color=blue>Assets/WebPlayerTemplates/HappyFunTimes/controllers/gamepad/controller.js</color>
and allows you choose various styles of controller. Of course you are free to make your own controller.js and do whatever
you want.

There's also a <color=blue>HFTInput</color> component which emulates the built in <color=blue>Input</color> class.

Finally there's a <color=blue>BirdScript</color> component which controls each player's character. It interacts
with the <color=blue>HFTInput</color> component to control the player as well as the <color=blue>HFTGamepad</color>
component to let the game set the color of the controller and get the user's name.
