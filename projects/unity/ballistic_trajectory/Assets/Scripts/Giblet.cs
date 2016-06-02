// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class Giblet : MonoBehaviour {

    // Inspector fields
	[SerializeField] BallisticMotion motion;

    // Methods
    void Awake() {
        // Apply velocity
        float x = Random.Range(.5f, 1.5f);
        float y = Random.Range(3f, 4f);

        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        var xz = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * x;

        Vector3 vel = new Vector3(xz.x, y, xz.y);
        motion.AddImpulse(vel);
    }
}
