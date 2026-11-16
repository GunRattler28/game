using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement
    
    public InputAction moveAction;
    public InputAction jumpAction;
    public LayerMask ground;
    public float moveSpeed = 5f;
    public float jumpForce = 7.5f;
    public float groundCheckRadius = 0.2f;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collisionBox;
    private SpriteRenderer spriteRenderer;
    private float move;
    private bool grounded;
    private bool jumpRequested;

    // Damage Knockback
    
    public float xRebound = 4f;
    public float yRebound = 4f;
    public float kbTime = 0.5f;
    private float kbCooldown;
    private bool kbActive;
    private float xKb;

    // Health

    public int maxHealth = 100;
    public int currentHealth { get; private set;} // Makes currentHealth read only for other scripts so that they can still see the value but can't change it. 'private set' Allows this script to directly change health though

    // Invicibility Frames

    public float iTime = 1f;
    private bool isInvincible = false;
    private float damageCooldown;

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable() {
        moveAction.Disable(); 
        jumpAction.Disable();   
    }

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        collisionBox = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        move = moveAction.ReadValue<float>();

        // Finding whether player is on ground

        Vector2 feetPosition = new Vector2(transform.position.x, transform.position.y - (collisionBox.size.y / 2));

        if (Physics2D.OverlapCircle(feetPosition, groundCheckRadius, ground)) {
            grounded = true;
        } else {
            grounded = false;
        }

        // Save that player wants to jump until fixed update

        if (jumpAction.IsPressed()) {
            jumpRequested = true;
        }
        
        // Flip sprite when turning around. JUST visuals.

        if (move > 0) {
            spriteRenderer.flipX = false;
        } else if (move < 0) {
            spriteRenderer.flipX = true;
        }

        // Time iframes

        if (isInvincible ){
            damageCooldown -= Time.deltaTime;
            if (damageCooldown <= 0){
                isInvincible = false;
            }
        }

        // Time kb

        if (kbActive) {
            kbCooldown -= Time.deltaTime;
            if (kbCooldown <= 0){
                kbActive = false;
            }
        }
    }

    void FixedUpdate()
    {
        
        float xMovement = move * moveSpeed;

        // Combines user input and knockback to control the player

        if (kbActive) {
            xMovement = xKb + xMovement;
        }

        // Caps horizontal speed so that knockback + user input don't make the player move too fast

        if (xMovement < -moveSpeed) {
            xMovement = -moveSpeed;
        } else if (xMovement > moveSpeed) {
            xMovement = moveSpeed;
        }
        
        float yMovement = rigidBody.linearVelocity.y;  // `rigidBody.linearVelocity.y` means if no jump, gravity continues as normal

        if (jumpRequested && grounded) {
            yMovement = jumpForce;
            grounded = false;
        }
        
        jumpRequested = false;

        rigidBody.linearVelocity = new Vector2(xMovement, yMovement);
    }

    // Function to change health. Amount to change health by given. 'void' specifies nothing will be returned. 'public' lets other scripts call the function

    public void ChangeHealth(int change, Vector2 damagePosition)
    {
        // Only check invicibility and add kb if taking damage
        
        if (change < 0) {

            if (isInvincible) {
                return;
            }

            isInvincible = true;
            damageCooldown = iTime;

            currentHealth = Mathf.Clamp(currentHealth + change, 0, maxHealth); // Ensures health can't go above maximum health or below zero
            Debug.Log($"{currentHealth} / {maxHealth}");

            // Knockback logic

            xKb = xRebound;
            float yKb = yRebound;

            // Bases kb direction on damage position so that kb is always away from the hazard

            if (transform.position.x < damagePosition.x) {
                xKb = -xRebound;
            }

            if (transform.position.y < damagePosition.y) {
                yKb = -yRebound;
            }

            rigidBody.linearVelocity = new Vector2(xKb, yKb);
            kbActive = true;
            kbCooldown = kbTime;
        }
    }
}

// scale the kb down over time