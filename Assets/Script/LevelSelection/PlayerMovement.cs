using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Animator anim;

    private bool canMove = true;
    private bool wasMoving = false;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
{
    if(!canMove)
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isWalk", false);
        if (wasMoving)
        {
            AudioManager.instance.StopSFXWalk();
            wasMoving = false;
        }
        return;
    }

    Vector2 moveInput = Vector2.zero;

    if (Keyboard.current != null)
    {
        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) moveX -= 1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) moveX += 1f;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) moveY -= 1f;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) moveY += 1f;

        moveInput = new Vector2(moveX, moveY).normalized;

        bool isMoving = moveInput != Vector2.zero;
        anim.SetBool("isWalk", isMoving);

        if (isMoving && !wasMoving)
        {
            AudioManager.instance.PlaySFXWalk();
        }
        else if (!isMoving && wasMoving)
        {
            AudioManager.instance.StopSFXWalk();
        }

        wasMoving = isMoving;

        if(moveX < 0)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = true;
        }
        else if (moveX > 0)
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.flipX = false;
        }
    }

    rb.linearVelocity = moveInput * _speed;
}

    public void SetCanMove(bool value)
    {
        canMove = value;
    }
}