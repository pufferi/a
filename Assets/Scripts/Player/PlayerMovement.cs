using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance { get; private set; }

        private float moveSpeed=5f;
        private float groundDrag = 5f;

        private BoxCollider playerCollider;
        private float playerHeight;

        [SerializeField]
        private LayerMask Layer_Ground;
        private bool grounded;

        public Transform orientation;
        public InputActionAsset inputActions;

        private Vector2 moveInput;
        private InputAction moveAction;
        private InputAction jumpAction;

        Rigidbody playerRigidbody;

        public bool playerStill = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            playerRigidbody = GetComponent<Rigidbody>();
            playerCollider = GetComponent<BoxCollider>();

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
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 1f, Layer_Ground);
            GetInput();

            SpeedControl();
            if (grounded)
                playerRigidbody.drag = groundDrag;
            else
                playerRigidbody.drag = 0;

            PreventPlayerFromSuiside();
        }

        [SerializeField]
        TMPro.TextMeshProUGUI text;
        private void PreventPlayerFromSuiside()
        {
            if (transform.position.y < -10)
            {
                transform.position = new Vector3(0, 10, 0);
                playerRigidbody.velocity = Vector3.zero;
                text.text = "I know the world sucks, but it's not the time to kill yourself";
                StartCoroutine(HideTextAfterDelay(4));
            }
        }
        private IEnumerator HideTextAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            text.text = string.Empty;
        }

        private void FixedUpdate()
        {
            if (!playerStill) PlayerMove();
        }


        private void GetInput()
        {
            if (moveAction != null)
                moveInput = moveAction.ReadValue<Vector2>();
        }

        private void PlayerJump(InputAction.CallbackContext context)
        {
            if (context.performed && grounded)
                playerRigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
        }

        private void PlayerMove()
        {
            Vector3 moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;

            // Check for small steps and move forward if detected
            if (moveInput.y > 0 && IsSmallStepAhead())
            {
                transform.position += Vector3.up * 0.2f;
                transform.position += orientation.forward * moveSpeed * Time.deltaTime;
            }
        }

        private bool IsSmallStepAhead()
        {
            RaycastHit hit;
            Vector3 origin = transform.position + Vector3.up * 0.1f;
            Vector3 direction = transform.forward;
            float maxDistance = 0.5f;

            if (Physics.Raycast(origin, direction, out hit, maxDistance))
            {
                float stepHeight = hit.point.y - transform.position.y;
                if (stepHeight > 0 && stepHeight <= 0.2f && !hit.collider.isTrigger)//added !hit.collider.isTrigger
                {
                    return true;
                }
            }
            return false;
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

    }
}
