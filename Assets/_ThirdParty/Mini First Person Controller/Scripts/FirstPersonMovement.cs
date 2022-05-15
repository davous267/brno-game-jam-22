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

    private float defaultHeadYpos;
    private float lastHbAmplitude = -1.0f;
    private float sinArg = 0.0f;

    void Awake()
    {
        // Get the rigidbody on this.
        rigidbody = GetComponent<Rigidbody>();
        defaultHeadYpos = fpsCamera.transform.localPosition.y;
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
        Vector2 targetVelocity = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized * targetMovingSpeed;

        // Apply movement.
        rigidbody.velocity = transform.rotation * new Vector3(targetVelocity.x, rigidbody.velocity.y, targetVelocity.y);

        // Head bob ...
        bool isIdling = targetVelocity.magnitude <= float.Epsilon;
        float hbAmplitude = isIdling ? headBobIdling : (IsRunning ? headBobSprint : headBobNormal);
        
        // Return to default y-pos when you change movement type / amplitude with which you work...
        if (lastHbAmplitude >= 0.0f && lastHbAmplitude != hbAmplitude)
        {
            fpsCamera.transform.localPosition = new Vector3(fpsCamera.transform.localPosition.x, defaultHeadYpos, fpsCamera.transform.localPosition.z);
            lastHbAmplitude = hbAmplitude;
        }

        sinArg = (sinArg + ((isIdling ? 1.0f : targetMovingSpeed) * headBobFreqMultiplier) / Time.deltaTime) % (2.0f * Mathf.PI);
        fpsCamera.transform.localPosition = new Vector3(fpsCamera.transform.localPosition.x,
            defaultHeadYpos + Mathf.Sin(sinArg) * hbAmplitude,
            fpsCamera.transform.localPosition.z);
    }
}