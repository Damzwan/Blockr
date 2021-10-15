using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public float turnSmoothTime = 0.1f;
    public float jumpHeight = 1.0f;
    public float gravityScale = 3;

    private float turnSmoothVelocity;
    private float gravity = -9.81f;
    private Vector3 playerVelocity;

    private Transform cam;
    private bool groundedPlayer;
    private CharacterController c;


    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }


    void handleMovement()
    {
        var originalPos = transform.position; // We save this to revert to it in case we do illegal movement (e.g drown)

        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Moving
        if (direction.magnitude >= 0.1f)
        {
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0);
            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            c.Move(moveDir.normalized * (speed * Time.deltaTime));
        }

        // Jumping & Gravity
        groundedPlayer = c.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0) playerVelocity.y = 0f;
        playerVelocity.y += gravity * gravityScale * Time.deltaTime; // We move the controller down with gravity
        if (Input.GetButtonDown("Jump") && groundedPlayer) playerVelocity.y += Mathf.Sqrt(jumpHeight * -3 * gravity);
        c.Move(playerVelocity * Time.deltaTime);
    }

    public void test()
    {
        Debug.Log("crazy");
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag($"Interactable")) hit.gameObject.GetComponent<InteractableBlock>().interact(gameObject);
    }
}