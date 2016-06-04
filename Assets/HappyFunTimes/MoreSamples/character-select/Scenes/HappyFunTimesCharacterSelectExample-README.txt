# Example of allowing player to select a character

This is basically just the `Assets/HappyFunTimes/MoreSamples/Simple/Scenes/HappyFunTimesSimpleScene` example
except it has a custom controller in `Assets/WebPlayerTemplates/HappyFunTimes/controllers/character-select/controller.html`

If you look at the `ExampleCharacterSelectSpawner` you'll see it has a `PlayerSpawer` set the spawn
the `PrefabToSelectCharacters` prefab anytime a player connects.

That prefab runs the `ExampleCharacterSelect.cs` script. It's a standard script using a `NetPlayer`
to communicate with the controller. It waits for the player to pick a character. it then spawns
the selected prefab for that character and calls `SendMessage` to pass the `NetPlayer` for that
player over to the new prefab. Those prefabs run a `ExampleCharacterSelectPlayer.cs` script that
is pretty much a clone of the `ExampleSimplePlayer.cs` script from the simple example except
it starts when its `InitializeFromCharacterSelect` function is called.

For more info see [http://docs.happyfuntimes.net/docs/unity/character-selection.html](http://docs.happyfuntimes.net/docs/unity/character-selection.html).
