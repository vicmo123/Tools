using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    Rigidbody rb;

    //Moving stats
    [SerializeField] private float moveSpeed = 50f;
    [SerializeField] private float runSpeed = 75f;
    [SerializeField] private float viewSensitivity = 200f;
    [SerializeField] private float maxWalkSpeed = 25f;
    [SerializeField] private float maxRunSpeed = 35f;

    //Jumping stats
    [SerializeField] private float jumpAmount = 35;
    private Vector3 augmentedGravityScale = new Vector3(0, -12, 0);
    private bool isGrounded = false;
    private float groundCheckDistance = 1.05f;
    private int jumpsPerformed = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    void Update()
    {
        Jump();
        MovePlayer();
    }

    void MovePlayer()
    {
        bool isRunning = false;

        if (Input.GetKey(KeyCode.W))
        {
            rb.velocity += transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.velocity += -transform.forward * moveSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + -1.0f * viewSensitivity * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 1.0f * viewSensitivity * Time.deltaTime, 0);
        }   
        if (Input.GetKey(KeyCode.R))
        {
            rb.velocity += transform.forward * runSpeed * Time.deltaTime;
            isRunning = true;
        }

        if (rb.velocity.magnitude > maxWalkSpeed && !isRunning)
            rb.velocity = rb.velocity.normalized * maxWalkSpeed;
        else if(rb.velocity.magnitude > maxRunSpeed && isRunning)
            rb.velocity = rb.velocity.normalized * maxRunSpeed;

        if (isGrounded)
            jumpsPerformed = 0;
    }

    private void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, -transform.up, groundCheckDistance);

        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode.Impulse);
            jumpsPerformed = 1;
        }
        else if (!isGrounded && Input.GetKeyDown(KeyCode.Space) && jumpsPerformed < 2)
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode.Impulse);
            jumpsPerformed = 2;
        }
        else if (!isGrounded)
        {
            rb.AddForce(augmentedGravityScale, ForceMode.Acceleration);
        }
    }
}
