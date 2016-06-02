// LICENSE
//
//   This software is dual-licensed to the public domain and under the following
//   license: you are granted a perpetual, irrevocable license to copy, modify,
//   publish, and distribute this file as you see fit.

using UnityEngine;
using System.Collections;
using System.Linq;

public class Turret : MonoBehaviour {

    // Inspector fields
    [SerializeField] GameObject projPrefab;
    [SerializeField] Transform muzzle;
    [SerializeField] Parameters parameters;
    [SerializeField] Projector rangeIndicator;

    // Private fields
    Target curTarget;
    State state = State.Searching;
    float cooldownTime;
    uint solutionIndex;
    bool paused;

    // Helper enums
    enum State {
        Searching,
        Aiming,
        Firing,
        Waiting
    };
    
    // Methods
    void Update() {

        // Update turret height
        transform.position = new Vector3(transform.position.x, parameters.turretHeight, transform.position.z);

        // Grab params
        float projSpeed = parameters.velocity;
        float gravity = parameters.gravity;
        Vector3 projPos = muzzle.position;

        // Update range indicator
        float range = fts.ballistic_range(projSpeed, gravity,projPos.y);
        rangeIndicator.orthographicSize = range;

        // Update/check pause
        if (Input.GetKeyDown(KeyCode.P))
            paused = !paused;

        if (paused)
            return;

        // State "machine"
        if (state == State.Searching) {
            var targets = GameObject.FindGameObjectsWithTag("Target");
            foreach (var target in targets) {
                var t = target.GetComponent<Target>();
                if (t) {
                    curTarget = t;
                    state = State.Aiming;
                    break;
                }
            }

        }

        if (state == State.Aiming) {

            Vector3 targetPos = curTarget.aimPos.position;
            Vector3 diff = targetPos - projPos;
            Vector3 diffGround = new Vector3(diff.x, 0f, diff.z);

            if (parameters.aimMode == Parameters.AimMode.Normal) {
                Vector3[] solutions = new Vector3[2];
                int numSolutions;

                if (curTarget.velocity.sqrMagnitude > 0)
                    numSolutions = fts.solve_ballistic_arc(projPos, projSpeed, targetPos, curTarget.velocity, gravity, out solutions[0], out solutions[1]);
                else
                    numSolutions = fts.solve_ballistic_arc(projPos, projSpeed, targetPos, gravity, out solutions[0], out solutions[1]);

                if (numSolutions > 0) {
                    transform.forward = diffGround;

                    var proj = GameObject.Instantiate<GameObject>(projPrefab);
                    var motion = proj.GetComponent<BallisticMotion>();
                    motion.Initialize(projPos, gravity);

                    var index = solutionIndex % numSolutions;
                    var impulse = solutions[index];
                    ++solutionIndex;

                    motion.AddImpulse(impulse);

                    state = State.Firing;
                }
            }
            else if (parameters.aimMode == Parameters.AimMode.Lateral) {
                Vector3 fireVel, impactPos;

                if (fts.solve_ballistic_arc_lateral(projPos, projSpeed, targetPos, curTarget.velocity, parameters.arcPeak, out fireVel, out gravity, out impactPos)) {
                    transform.forward = diffGround;

                    var proj = GameObject.Instantiate<GameObject>(projPrefab);
                    var motion = proj.GetComponent<BallisticMotion>();
                    motion.Initialize(projPos, gravity);

                    motion.AddImpulse(fireVel);

                    state = State.Firing;
                }
            }
            else {
                state = State.Searching;
            }
        }

        if (state == State.Firing) {
            float cooldown = 1f / parameters.rateOfFire;
            cooldownTime = Time.time + cooldown;
            state = State.Waiting;
        }

        if (state == State.Waiting) {
            if (Time.time > cooldownTime)
                state = State.Searching;
        }
    }
}
