// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class ProjectileTrigger : MonoBehaviour {

    // Inspector fields
    [SerializeField] GameObject root;
    [SerializeField] GameObject gibletPrefab;

    // Methods
    void OnTriggerEnter(Collider other) {
        var tt = other.GetComponent<TargetTrigger>();
        if (!tt)
            return;

        for (int i = 0; i < 10; ++i) {
            var go = GameObject.Instantiate<GameObject>(gibletPrefab);

            float x = .5f;
            float y = .25f;
            var spawnPos = transform.position + new Vector3(Random.Range(-x, x), Random.Range(-y, y), Random.Range(-x, x));
            go.GetComponent<BallisticMotion>().Initialize(spawnPos, 9.8f);
        }

        Destroy(root);
    }
}
