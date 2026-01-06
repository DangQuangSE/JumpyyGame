using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Header("Jump")]
    [SerializeField] private float jumpVelocity = 10f;
    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private Animator animator;
    private Rigidbody2D rb;
    private float moveInput;
    private bool jumpPressed;
    private bool isGrounded;
    private GameManager gameManager;
    private AudioManager audioManager;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void Update()
    {
        if (gameManager.IsGameOver() || gameManager.IsGameWin()) return;
        moveInput = Input.GetAxisRaw("Horizontal");
        isGrounded = CheckGrounded();
        if (Input.GetButtonDown("Jump"))
        {
            jumpPressed = true;
        }
        UpdateFacing(moveInput);
      
    }
    private void FixedUpdate()
    {
        ApplyMovement();

        if (jumpPressed)
        {
            TryJump();
            jumpPressed = false;
        }
        UpdateAnimation();
    }

    private void ApplyMovement()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
    }

    private void TryJump()
    {
        if (!isGrounded) return;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        audioManager.PlayJumpSound();
    }

    private bool CheckGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void UpdateFacing(float xInput)
    {
        if (xInput > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (xInput < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        bool isJumping = !isGrounded;
        animator.SetBool("isRunning", isRunning);
        animator.SetBool("isJumping", isJumping);
    }
}
