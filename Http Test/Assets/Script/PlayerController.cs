using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float jumpForce = 12f; // Increased jump force
    public float fallMultiplier = 2.5f; // Increased gravity when falling
    public float lowJumpMultiplier = 2f; // Increased gravity for short jumps
    public float coyoteTime = 0.1f; // Time allowed to jump after leaving the ground
    public float jumpBufferTime = 0.1f; // Time allowed to queue a jump before landing

    private Rigidbody2D rb;
    private bool isGrounded;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;

    public GameManager gameManager;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; 
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        // Jump buffer
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
        {
            rb.linearVelocity = Vector2.up * jumpForce;
            jumpBufferCounter = 0; 
        }
        if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space) && !Input.GetMouseButton(0))
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            gameManager.GameOver();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}