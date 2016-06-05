using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class SyncedClockPlayer : MonoBehaviour {

    // We're not doing anything in the game.
    // Just kill us when we disconnect.
    void InitializeNetPlayer(SpawnInfo spawnInfo)
    {
        spawnInfo.netPlayer.OnDisconnect += Remove;
    }

    void Remove(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
}
