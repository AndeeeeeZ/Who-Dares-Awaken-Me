using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private SoundEffectPlayer soundEffectPlayer; 

    [SerializeField]
    private float moveSpeed, rotationSpeed, jumpForce;

    [SerializeField, Min(0f)]
    private int maxNumJump;

    private Rigidbody rb;
    private InputActions input;
    private InputAction lookAction;

    private float xMovement, zMovement;
    private float pendingRotation;
    private int remainingJumps;

    private void Awake()
    {
        input = new InputActions();
        lookAction = input.Player.Look;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        xMovement = 0f;
        zMovement = 0f;
        pendingRotation = 0f;
        remainingJumps = maxNumJump;
    }

    private void FixedUpdate()
    {
        if (pendingRotation != 0f)
        {
            Quaternion deltaRotation = Quaternion.Euler(0f, pendingRotation, 0f);
            rb.MoveRotation(rb.rotation * deltaRotation);
            pendingRotation = 0f;
        }

        // Get movement in local space
        Vector3 move = new Vector3(xMovement, 0f, zMovement).normalized;

        // Convert to world space relative to player facing direction
        Vector3 moveDir = transform.TransformDirection(move);

        rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, rb.linearVelocity.y, moveDir.z * moveSpeed);
    }

    public void RotateBy(float yRotationDelta)
    {
        pendingRotation += yRotationDelta; 
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Move.performed += OnMovePerformed;
        input.Player.Move.canceled += OnMoveCanceled;
        input.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Move.performed -= OnMovePerformed;
        input.Player.Move.canceled -= OnMoveCanceled;
        input.Player.Jump.performed -= OnJump;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        xMovement = context.ReadValue<Vector2>().x;
        zMovement = context.ReadValue<Vector2>().y;
        soundEffectPlayer.Play(); 
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        xMovement = 0f;
        zMovement = 0f;
        soundEffectPlayer.StopPlaying(); 
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (remainingJumps > 0)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            remainingJumps--;
        }
        else
        {
            Debug.Log("Player can't jump again in the air");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        remainingJumps = maxNumJump;
    }
}