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
    private int currentHealth;

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

    // Function to change health. Amount to change health by given. 'void' specifies nothing will be returned

    void changeHealth(int change)
    {
        currentHealth = Mathf.Clamp(currentHealth + change, 0, maxHealth); // ensures health can't go above maximum health or below zero
        Debug.Log($"{currentHealth} / {maxHealth}");
    }
}