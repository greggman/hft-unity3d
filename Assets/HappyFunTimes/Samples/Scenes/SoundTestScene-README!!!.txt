This scene shows the game telling the gamepad controller to play a sound

Typically you'd load sounds yourself in JavaScript using
whatever library you'd like to use. In this example though the <color=blue>HFTGamepad</color> controller
loads all sounds in <color=blue>Assets/WebPlayerTemplates/HappyFunTimes</color>
and below.

The <color=blue>HFTGlobalSoundHelper</color> gets the names and paths of all the sounds.
The <color=blue>HFTSoundPlayer</color> gets that list from <color=blue>HFTGlobalSoundHelper</color> and
sends it to the controller (controllers/gamepad/controller.js). The controller then loads all the sounds.
Each file is identified by its filename meaning they must have unique names.

The <color=blue>SoundTest.cs</color> script attached the the <color=blue>SoundTester</color>
prefab that is birthed for each player that joins happyfuntimes. Every second it tells the controller the play the next sound

Note that <color="magenta">players much touch the screen at least once</color>
for sounds to start on phones.

