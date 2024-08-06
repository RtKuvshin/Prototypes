using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody rb;
    public float acceleration = 500f;
    public float steering = 300f;
    public float maxSpeed = 20f;
    
    // Drag and drop the wheel transforms in the Inspector
    public Transform[] wheels;

    private float moveInput;
    private float steerInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        moveInput = Input.GetAxis("Vertical"); // W/S or Up/Down arrow keys
        steerInput = Input.GetAxis("Horizontal"); // A/D or Left/Right arrow keys
    }

    void FixedUpdate()
    {
        // Apply acceleration force
        if (rb.velocity.magnitude < maxSpeed)
        {
            Vector3 force = transform.forward * moveInput * acceleration * Time.deltaTime;
            rb.AddForce(force);
            Debug.Log("Applying Force: " + force);
        }

        // Apply steering
        float turn = steerInput * steering * Time.deltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        Debug.Log("Applying Steering: " + turn);

        // Simulate wheel rotation (visual effect)
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(Vector3.right, moveInput * acceleration * Time.deltaTime);
        }
    }

}

