define([
    './hft/scripts/gameclient',
    './hft/scripts/syncedclock',
  ], function (
    GameClient,
    SyncedClock) {
    window.hft = window.hft || {};
    var api = window.hft;
    api.GameClient = GameClient;
    api.SyncedClock = SyncedClock;
});
