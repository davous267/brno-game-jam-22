using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField]
    private FirstPersonLook fpsCamera;

    [SerializeField]
    private float headBobIdling = 0.002f;

    [SerializeField]
    private float headBobNormal = 0.01f;

    [SerializeField]
    private float headBobSprint = 0.02f;

    [SerializeField]
    private float headBobFreqMultiplier = 2.0f;

    public float speed = 5;

    [Header("Running")]
    public bool canRun = true;
    public bool IsRunning { get; private set; }
    public float runSpeed = 9;
    public KeyCode runningKey = KeyCode.LeftShift;
    new Rigidbody rigidbody;
    /// <summary> Functions to override movement speed. Will use the last added override. </summary>
    public List<System.Func<float>> speedOverrides = new List<System.Func<float>>();

    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Update IsRunning from input.
        IsRunning = canRun && Input.GetKey(runningKey);

        // Get targetMovingSpeed.
        float targetMovingSpeed = IsRunning ? runSpeed : speed;
        if (speedOverrides.Count > 0)
        {
            targetMovingSpeed = speedOverrides[speedOverrides.Count - 1]();
        }

        // Get targetVelocity from input.
        Vector2 targetVelocity =new Vector2( Input.GetAxis("Horizontal") * targetMovingSpeed, Input.GetAxis("Vertical") * targetMovingSpeed);

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);

        // Head bob ...
        float hbAmplitude = targetVelocity.magnitude <= float.Epsilon ? headBobIdling : (IsRunning ? headBobSprint : headBobNormal);
        float hbFerqMult = targetVelocity.magnitude <= float.Epsilon ? 1.0f : headBobFreqMultiplier;

        fpsCamera.transform.Translate(new Vector3(0.0f, Mathf.Sin(Time.time * targetMovingSpeed * hbFerqMult) * hbAmplitude, 0.0f), Space.Self);
    }
}