// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class WASDCameraController : MonoBehaviour {

    // Inspector fields
    [SerializeField] Camera _camera;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;

    // Methods
    void Update() {
        float dt = Time.deltaTime;

        // Forward/backward
        if (Input.GetKey(KeyCode.W))
            _camera.transform.position += _camera.transform.forward * moveSpeed * dt;
        else if (Input.GetKey(KeyCode.S))
            _camera.transform.position -= _camera.transform.forward * moveSpeed * dt;

        // Left/Right
        if (Input.GetKey(KeyCode.A))
            _camera.transform.position -= _camera.transform.right * moveSpeed * dt;
        else if (Input.GetKey(KeyCode.D))
            _camera.transform.position += _camera.transform.right * moveSpeed * dt;

        // Up/Down
        if (Input.GetKey(KeyCode.Q))
            _camera.transform.position -= _camera.transform.up * moveSpeed * dt;
        else if (Input.GetKey(KeyCode.E))
            _camera.transform.position += _camera.transform.up * moveSpeed * dt;

        // Freelook
        if (Input.GetMouseButton(2)) {
            float x = Input.GetAxis("Mouse X") * dt * 50 * rotateSpeed * Mathf.Deg2Rad;
            float y = Input.GetAxis("Mouse Y") * dt * 50 * rotateSpeed * Mathf.Deg2Rad;

            Vector3 angles = _camera.transform.localEulerAngles;
            angles.x -= y;
            angles.y += x;
            _camera.transform.localEulerAngles = angles;
        }
    }
}
