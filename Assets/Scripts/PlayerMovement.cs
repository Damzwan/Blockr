using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMovement : MonoBehaviour
{
    public float mass = 10;
    public float impactConsumptionAmount = 5f;
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
    private Vector3 impact = Vector3.zero;

    private bool isFrozen;
    private float freezeCooldown = 0.1f;
    private float freezeTimer;
    private float velocityAmplifier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        c = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeTimer > 0) freezeTimer -= Time.deltaTime;
        handleMovement();
    }


    void handleMovement()
    {
        // Now we read the inputs and move our character accordingly
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0f, vertical).normalized;

        var inputMotion = Vector3.zero;
        var impactMotion = Vector3.zero;

        // Moving
        if (direction.magnitude >= 0.1f)
        {
            if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical")) unFreeze();
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
                turnSmoothTime);

            transform.rotation = Quaternion.Euler(0f, angle, 0);
            var moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            inputMotion = moveDir.normalized * (speed * Time.deltaTime) * velocityAmplifier;
        }


        if (impact.magnitude > 0.2) impactMotion = impact * Time.deltaTime;
        // consumes the impact energy each cycle
        impact = Vector3.Lerp(impact, Vector3.zero, impactConsumptionAmount * Time.deltaTime);

        // Jumping & Gravity
        groundedPlayer = c.isGrounded;
        if ((groundedPlayer || isFrozen) && playerVelocity.y < 0) playerVelocity.y = 0f;
        playerVelocity.y += gravity * gravityScale * Time.deltaTime; // We move the controller down with gravity
        if (Input.GetButtonDown("Jump") && (groundedPlayer || isFrozen))
        {
            unFreeze();
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3 * gravity);
        }

        // we use the move function only once to get a correct characterController.velocity 
        c.Move(isFrozen ? Vector3.zero : playerVelocity * Time.deltaTime + inputMotion + impactMotion);
    }

    public void freeze()
    {
        if (freezeTimer > 0) return;
        isFrozen = true;
    }

    void unFreeze()
    {
        isFrozen = false;
        freezeTimer = freezeCooldown;
    }

    public void setVelocityAmplifier(float newVelocity)
    {
        velocityAmplifier = newVelocity;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        setVelocityAmplifier(1); //TODO maybe not the best idea?
        if (hit.gameObject.CompareTag($"Interactable"))
            hit.gameObject.GetComponent<InteractableBlock>().interact(gameObject);
    }

    public void addImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }
}