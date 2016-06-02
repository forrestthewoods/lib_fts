// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class Parameters : MonoBehaviour {

    // Inspector fields
    [SerializeField] GameObject uiRoot;
    [SerializeField] UnityEngine.UI.Slider velocitySlider;
    [SerializeField] UnityEngine.UI.Slider gravitySlider;
    [SerializeField] UnityEngine.UI.Slider turretHeightSlider;
    [SerializeField] UnityEngine.UI.Slider targetHeightSlider;
    [SerializeField] UnityEngine.UI.Slider targetDistanceSlider;
    [SerializeField] UnityEngine.UI.Slider arcPeakSlider;
    [SerializeField] UnityEngine.UI.Slider rateOfFireSlider;

    // Private fields
    AimMode _aimMode;
  

    // Properties
    public float velocity { get { return Mathf.Lerp(1f, 30f, velocitySlider.value); } }
    public float gravity { get { return Mathf.Lerp(0f, 20f, gravitySlider.value); } }
    public float turretHeight { get { return Mathf.Lerp(0f, 10f, turretHeightSlider.value); } }
    public float targetHeight { get { return Mathf.Lerp(0f, 10f, targetHeightSlider.value); } }
    public float targetDistance { get { return Mathf.Lerp(5f, 50f, targetDistanceSlider.value); } }
    public float rateOfFire { get { return Mathf.Lerp(1f, 10f, rateOfFireSlider.value); } }
    public float arcPeak { get { return Mathf.Lerp(.1f, 10f, arcPeakSlider.value); } }

    public AimMode aimMode {
        get { return _aimMode; }
        set {
            _aimMode = value;
            arcPeakSlider.transform.parent.gameObject.SetActive(aimMode == AimMode.Lateral);
        }
    }

    // Methods
    void Update() {
        if (Input.GetKeyDown(KeyCode.U))
            uiRoot.SetActive(!uiRoot.activeSelf);

        if (Input.GetKeyDown(KeyCode.F))
            aimMode = aimMode == AimMode.Normal ? AimMode.Lateral : AimMode.Normal;
    }

    // Enums
    public enum AimMode {
        Normal,
        Lateral
    };
}
