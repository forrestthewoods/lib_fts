// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class Target : MonoBehaviour {

    // Inspector fields
    [SerializeField] Transform _aimPos;
    [SerializeField] Parameters parameters;

    // Private fields
    Vector3 targetPos;

    // Properties
    public Transform aimPos { get { return _aimPos; } }
    public Vector3 velocity { get; set; }
    public bool moving { get; set; }

    void Update() {
        const float moveSpeed = 7.5f;
        const float targetDist = 40f;
        const float targetMaxHeight = 10f;

        if (Input.GetKeyDown(KeyCode.M)) {
            moving = !moving;
            if (moving)
                targetPos = new Vector3(transform.position.x, Random.Range(0f, targetMaxHeight), targetDist);
            else
                velocity = Vector3.zero;
        }

        if (moving) {
            float dt = Time.deltaTime;
            Vector3 diff = targetPos - transform.position;
            velocity = moveSpeed * diff.normalized;
            float delta = moveSpeed * dt;

            if (diff.magnitude < delta) {
                targetPos = new Vector3(targetPos.x + Random.Range(-5f, 5f), Random.Range(0f, targetMaxHeight), targetPos.z > 0 ? -targetDist : targetDist);
            }
            else
                transform.position += velocity * dt;

            transform.forward = velocity;
        }
        else
        {
            transform.position = new Vector3(parameters.targetDistance, parameters.targetHeight, 0);
        }
    }
}
