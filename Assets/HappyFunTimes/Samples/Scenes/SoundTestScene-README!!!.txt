This scene shows the game telling the gamepad controller to play a sound

Typically you'd load sounds yourself in JavaScript using
whatever library you'd like to use. The HFTGamepad controller
though just loads all sounds in Assets/WebPlayerTemplates/HappyFunTimes

The HFTGlobalSoundHelper gets the names and paths of all the sounds.
The HFTSoundPlayer gets that list from HFTGlobalSoundHelper and
sends it to the controller (controllers/gamepad/controller.js).
The controller then loads all the sounds.

The SoundTest.cs script attached the the SoundTester prefab
that is birthed for each player that joins happyfuntimes.

Every second it tells the controller the play the next sound

Note that players much touch the screen at least once
for sounds to start on phones.

