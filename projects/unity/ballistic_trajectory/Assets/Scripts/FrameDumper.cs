// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;

public class FrameDumper : MonoBehaviour {

    // Inspector fields
    [SerializeField] string folder = "C:\\temp\\UnityCapture\\";
    [SerializeField] int frameRate = 60;
    [SerializeField] int sizeMultiplier = 1;

    // Private fields
    bool dumping;
    string realFolder;
    int count;
    int frame;

    void Update() {

        bool toggled = false;
        if (Input.GetKeyDown(KeyCode.C)) {
            dumping = !dumping;
            toggled = true;

            Time.captureFramerate = dumping ? frameRate : 0;
        }

        if (toggled && dumping) {
            realFolder = folder + count;
            while (System.IO.Directory.Exists(realFolder)) {
                realFolder = folder + count++;
            }

            System.IO.Directory.CreateDirectory(realFolder);
            frame = 0;
        }

        if (dumping) {
            var name = string.Format("{0}/shot{1:D04}.png", realFolder, frame++ );
            Application.CaptureScreenshot(name, 1);
        }
    }
}
