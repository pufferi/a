using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float groundDrag=5f;

        [Header("Ground Check")]
        private BoxCollider playerCollider;
        private float playerHeight;
        public LayerMask Layer_Ground;
        bool grounded;

        public Transform orientation;
        public InputActionAsset inputActions;

        private Vector2 moveInput;
        private InputAction moveAction;
        private InputAction jumpAction;


        Rigidbody playerRigidbody;

        private void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider=GetComponent<BoxCollider>();

            playerHeight = playerCollider.size.y;
            playerRigidbody.freezeRotation = true;

            var playerMap = inputActions.FindActionMap("Player");

            moveAction = playerMap.FindAction("Move");
            jumpAction = playerMap.FindAction("Jump");

            moveAction.Enable();
            jumpAction.Enable();

            jumpAction.performed += PlayerJump;
        }

        private void Update()
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Layer_Ground);
            Debug.Log(grounded);
            GetInput();

            SpeedControl();
            if (grounded)
                playerRigidbody.drag = groundDrag;
            else
                playerRigidbody.drag = 0;
        }

        private void FixedUpdate()
        {
            PlayerMove();
        }

        private void GetInput()
        {
            if (moveAction != null)
                moveInput = moveAction.ReadValue<Vector2>();
        }

        public void PlayerJump(InputAction.CallbackContext context)
        {
            if (context.performed &&grounded)
                playerRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }

        private void PlayerMove()
        {
            Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
            playerRigidbody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(playerRigidbody.velocity.x, 0f, playerRigidbody.velocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                playerRigidbody.velocity = new Vector3(limitedVel.x, playerRigidbody.velocity.y, limitedVel.z);
            }
        }

        private void OnDestroy()
        {
            if (jumpAction != null)
                jumpAction.performed -= PlayerJump;
        }
    }
}