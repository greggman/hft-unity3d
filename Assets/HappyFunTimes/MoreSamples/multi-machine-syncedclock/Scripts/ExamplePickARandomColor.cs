using UnityEngine;
using System.Collections;

// Becauase I'm too lazy to make 48 materials
public class ExamplePickARandomColor : MonoBehaviour {

    void Start()
    {
        var material = GetComponent<Renderer>().material;
        // We need to make it not really random otherwise they won't match across machines
        int seed = 0;
        foreach (var c in gameObject.name) {
            seed += c;
        }
        Random.seed = seed;
        material.color = Random.ColorHSV(0.0f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f);
    }
}
