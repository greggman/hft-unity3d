using UnityEngine;
using System.Collections;
using HappyFunTimes;

public class ExampleRotateBySyncClock : MonoBehaviour {

    public float speed = 1.0f;

    void Update ()
    {
        double time = HFTSyncedClock.Now;
        float rotation = (float)(time * (double)speed % 360.0);
        transform.eulerAngles = new Vector3(0.0f, rotation, 0.0f);
    }
}
