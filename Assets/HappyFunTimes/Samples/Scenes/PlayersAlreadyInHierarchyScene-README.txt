This is the same as the `2DPlatformScene` except there are 3 players already in the scene.

If you select the `LevelManager` you'll see it has a `PlayerConnector`
Script which lists the 3 player prefabs. The player prefabs are exactly the same as the prefabs
in the 2DPlatformScene except they've have their `deleteWhenDisconnected` setting
unchecked so that when a player's phone disconnects the player's GameObject will not be deleted.

The `PlayerConnector` conncets players' phones to GameObjects already in the scene.

