using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class CC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField, Range(0f, 0.5f)] private float deadzone = 0.15f;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        var gp = Gamepad.current;
        if (gp == null)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = gp.leftStick.ReadValue();

        if (moveInput.sqrMagnitude < deadzone * deadzone)
            moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        if (moveInput == Vector2.zero)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 displacement = moveInput * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + displacement);
    }
}   