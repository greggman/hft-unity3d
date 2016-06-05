# Synced Clock Example

HappyFunTimes provides a synced clock you can use across
controllers and games.

The clock is synced to within about 50ms. You can use this for whatever you want.

See the `LevelManager` GameObject in the scene. It has a script `SyncedClockScript`
that accesses the `HFTSyncedClock.Now` to get the current synced time and draw a clock.

The `PlayerSpawner` on the `LevelManager` spawns the `SyncedClockPrefab` which
as a `SyncedClockPlayer` that basically in this cases does nothing but enables
players to connect.

The controller in `Assets/WebPlayerTemplates/HappyFunTimes/controllers/syncedclock`
also makes a synced clock on the controller and draws a matching clock so you can
compare them. You should see they are not too far off.

Looking in `Assets/WebPlayerTemplates/HappyFunTimes/controllers/syncedclock/scripts/controller.js` the revant code is

```
// true = sync, false = don't sync (no network used)
var clock = hft.SyncedClock.createClock(true);

...

// get the time in seconds
var time = clock.getTime();
```

One great thing to do with synced clocks is large multi-view simulations.
This demo [https://www.youtube.com/watch?v=64TcBiqmVko](https://www.youtube.com/watch?v=64TcBiqmVko)
runs on 8 machines using a synced clock. The fish all based their positions on
the clock so as long as the clocks are in sync each machine can have its
camera set to a particular view and it looks like more is going on than actually is.

See [http://docs.happyfuntimes.net/unity/docs/talking-between-games.html](http://docs.happyfuntimes.net/unity/docs/talking-between-games.html) for more info.

