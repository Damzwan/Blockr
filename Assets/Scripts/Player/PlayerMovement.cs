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
    private CharacterController cc;
    private Vector3 impact = Vector3.zero;

    private bool isFrozen;
    private float freezeCooldown = 0.1f;
    private float freezeTimer;
    private float velocityAmplifier = 1f;

    private GameObject teleportBlock;
    private Vector3 respawnPos;

    private int movingPlatformAmount;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        respawnPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeTimer > 0) freezeTimer -= Time.deltaTime;
        handleMovement();
        if (Input.GetKeyDown(KeyCode.F)) teleport();
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
        groundedPlayer = cc.isGrounded;
        if ((groundedPlayer || isFrozen) && playerVelocity.y < 0) playerVelocity.y = 0f;
        playerVelocity.y += gravity * gravityScale * Time.deltaTime; // We move the controller down with gravity
        if (Input.GetButtonDown("Jump") && (groundedPlayer || isFrozen))
        {
            unFreeze();
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3 * gravity);
        }

        // we use the move function only once to get a correct characterController.velocity 
        cc.Move(isFrozen ? Vector3.zero : playerVelocity * Time.deltaTime + inputMotion + impactMotion);
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

    public void setTeleportBlock(GameObject newTeleportBlock)
    {
        teleportBlock = newTeleportBlock;
    }

    public void kill()
    {
        cc.enabled = false;
        transform.position = respawnPos;
        cc.enabled = true;
    }
    
    //TODO look at layers...
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        setVelocityAmplifier(1); //TODO maybe not the best idea?
        if (hit.gameObject.CompareTag($"Body Interactable"))
            hit.gameObject.GetComponent<IInteractableBlock>().interact(gameObject); //TODO no reason to use a tag
    }

    void teleport()
    {
        if (teleportBlock is null) return;
        cc.enabled = false;
        Destroy(teleportBlock);
        transform.position = teleportBlock.transform.position;
        teleportBlock = null;
        cc.enabled = true;
    }

    public void addImpact(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    public int getMovingPlatformAmount()
    {
        return movingPlatformAmount;
    }

    public void incrementMovingPlatformAmount()
    {
        movingPlatformAmount++;
    }

    public void decrementMovingPlatformAmount()
    {
        movingPlatformAmount--;
    }


}