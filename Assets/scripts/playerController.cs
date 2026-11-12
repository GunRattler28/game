using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement
    
    public InputAction moveAction;
    public InputAction jumpAction;
    public LayerMask ground;
    public float moveSpeed;
    public float jumpForce;
    public float groundCheckRadius;
    private Rigidbody2D rigidBody;
    private BoxCollider2D collisionBox;
    private SpriteRenderer spriteRenderer;
    private float move;
    private bool grounded;
    private bool jumpRequested;

    // Health

    public int maxHealth;
    public int currentHealth { get; private set;} // Makes currentHealth read only for other scripts so that they can still see the value but can't change it. 'private set' Allows this script to directly change health though

    // Invicibility Frames

    public float iTime = 1f;
    private bool isInvincible;
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

        if (isInvincible){
            damageCooldown -= Time.deltaTime;
            if (damageCooldown <= 0){
                isInvincible = false;
            }
        }
    }

    void FixedUpdate()
    {
        
        float yMovement = rigidBody.linearVelocity.y;  // `rigidBody.linearVelocity.y` means if no jump, gravity continues as normal

        if (jumpRequested && grounded) {
            yMovement = jumpForce;
            grounded = false;
        }
        
        jumpRequested = false;

        rigidBody.linearVelocity = new Vector2(move * moveSpeed, yMovement);
    }

    // Function to change health. Amount to change health by given. 'void' specifies nothing will be returned. 'public' lets other scripts call the function

    public void ChangeHealth(int change)
    {
        // Only check invicibility if taking damage
        
        if (change < 0){
            if (isInvincible){
                return;
            }
            isInvincible = true;
            damageCooldown = iTime;
        }

        currentHealth = Mathf.Clamp(currentHealth + change, 0, maxHealth); // Ensures health can't go above maximum health or below zero
        Debug.Log($"{currentHealth} / {maxHealth}");
    }
}