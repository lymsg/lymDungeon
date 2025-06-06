using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    private Vector2 curMovementInput;
    public float jumpPower;
    public LayerMask groundLayerMask;

    [Header("Look")]
    public Transform cameraContainer;
    public float minXLook;
    public float maxXLook;
    private float camCurXRot;
    public float lookSensitivity;
    private Vector2 mouseDelta;
    private bool isThirdPerson = false;
    private Vector3 firstPersonView = new Vector3(0f,0f,0f);
    private Vector3 thirdPersonView = new Vector3(0f, 0.17f, -1.7f);
    private Transform mainCamera;
    
    [Header("Jump")]
    public float jumpStamina;
    public bool doubleJumpAble;
    public bool isDoubleJump;

    [Header("Run")]
    public float runSpeed;
    private bool isRunning;
    public float runStamina;
    
    [Header("Climb")]
    public LayerMask climbLayerMask;
    private Vector3 wallNormal;
    private bool ClimbingMode = false;
    public float climbingStamina;
    [SerializeField] private bool isFalling = false;
    
    [HideInInspector]
    public bool canLook = true;
    public Action bag;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        mainCamera = transform.Find("Capsule/CameraContainer/Main Camera");
    }

    void Update()
    {
        IsGrounded();
        
        if (IsGrounded()) isDoubleJump = false;
        
        if (isRunning)
        {
            if (curMovementInput.magnitude > 0f)
            {
                float staminaToConsume = runStamina * Time.deltaTime;     //업뎃문에선 프레임시간 상관해야한다
                bool success = CharacterManager.Instance.Player.condition.useStamina(staminaToConsume);
                if (!success)
                {
                    isRunning = false;
                }
            }
        }

        if (ClimbingMode)
        {
            float staminaToConsume = climbingStamina * Time.deltaTime;
            bool success = CharacterManager.Instance.Player.condition.useStamina(staminaToConsume);
            if (!success)
            {
                ExitClimbigMode();
                isFalling = true;
                InputSystem.DisableDevice(Keyboard.current); //떨어지는동안 키입력 안되게
            }
        }

        if (isFalling)
        {
            if (IsGrounded())   //땅밟으면 isFalling변수 변경
            {
                isFalling = false;
                InputSystem.EnableDevice(Keyboard.current);
            }
        }
    }

    private void FixedUpdate()
    {
        Move();
        CheckWallClimbing();
    }

    private void LateUpdate()
    {
        if (canLook)
        {
            CameraLook();
        }
    }

    public void OnLookInput(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed)
        {
            curMovementInput = context.ReadValue<Vector2>();
        }
        else if(context.phase == InputActionPhase.Canceled)
        {
            curMovementInput = Vector2.zero;
        }
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started && IsGrounded() && CharacterManager.Instance.Player.condition.useStamina(jumpStamina))
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
            Debug.Log("Jump");
        }
        if (context.phase == InputActionPhase.Started && doubleJumpAble && !IsGrounded()) //더블점프가 가능한상태고 공중일때
        {
            if (!isDoubleJump && CharacterManager.Instance.Player.condition.useStamina(jumpStamina))
            {
                rb.AddForce(Vector2.up * jumpPower, ForceMode.Impulse);
                Debug.Log("doubleJump");
                isDoubleJump = true;
            }
        }
    }

    public void OnRunInput(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed && IsGrounded())
        {
            isRunning = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            isRunning = false;
        }
    }

    public void OnSwitchInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            Debug.Log("Switch");
            isThirdPerson = !isThirdPerson;
            Vector3 targetView = isThirdPerson ? thirdPersonView : firstPersonView;
            mainCamera.localPosition = targetView;
        }
    }

    private void Move()
    {
        float curSpeed = isRunning ? runSpeed : moveSpeed;
        
        Vector3 dir = transform.forward * curMovementInput.y + transform.right * curMovementInput.x;
        dir *= curSpeed;
        dir.y = rb.velocity.y;

        rb.velocity = dir;
    }

    void CameraLook()
    {
        camCurXRot += mouseDelta.y * lookSensitivity;
        camCurXRot = Mathf.Clamp(camCurXRot, minXLook, maxXLook);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);

        transform.eulerAngles += new Vector3(0, mouseDelta.x * lookSensitivity, 0);
    }

    bool IsGrounded()
    {
        Ray[] rays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.02f), Vector3.down),
            new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.02f), Vector3.down),
            new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.02f), Vector3.down),
            new Ray(transform.position + (-transform.right * 0.2f) +(transform.up * 0.02f), Vector3.down)
        };

        for(int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i], 0.3f, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    bool IsWall()
    {
        RaycastHit hit;
        Ray[] rays = new Ray[2]
        {
            new Ray(transform.position + (transform.up * 0.2f) - (transform.forward * 0.01f) , transform.forward),
            new Ray(transform.position - (transform.forward * 0.01f), transform.forward),
        };
        for (int i = 0; i < rays.Length; i++)
        {
            if (Physics.Raycast(rays[i],out hit, 0.6f, climbLayerMask))
            {
                wallNormal = hit.normal;
                return true;
            }
        }
    
        return false;
        
    }

    void CheckWallClimbing()
    {
        if (IsWall())
        {
            if (!isFalling)
            {
                if (!ClimbingMode)
                {
                    EnterClimbingMode();
                }

                if (!isThirdPerson)
                {
                    mainCamera.localPosition = thirdPersonView;
                }

                rb.velocity = Vector3.zero;
                rb.useGravity = false;
                rb.AddForce(-wallNormal * 10f, ForceMode.Force);

                if (Input.GetKey(KeyCode.W))
                {
                    rb.velocity = new Vector3(0f, 3f, 0f);
                }
                else if (Input.GetKey(KeyCode.S))
                {
                    rb.velocity = new Vector3(0f, -3f, 0f);
                }
            }
        }
        else
        {
            ExitClimbigMode();
        }
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.phase == InputActionPhase.Started)
        {
            bag?.Invoke();
            ToggleCursor();
        }
    }
    public void ToggleCursor()
    {
        bool toggle = Cursor.lockState == CursorLockMode.Locked;
        Cursor.lockState = toggle ? CursorLockMode.None : CursorLockMode.Locked;
        canLook = !toggle;
    }

    public void ForceJump(float forceJumpPower)
    {
        rb.AddForce(Vector2.up * forceJumpPower, ForceMode.Impulse);
    }

    public void EnterClimbingMode()
    {
        ClimbingMode = true;
        rb.useGravity = false;
        rb.velocity = Vector3.zero;
    }
    public void ExitClimbigMode()
    {
        ClimbingMode = false;
        rb.useGravity = true;
        if (!isThirdPerson)
        {
            mainCamera.localPosition = firstPersonView;
        }
    }

    private IEnumerator OnDoubleJump(float time)
    {
        float startTime = 0f;
        while (startTime < time)
        {
            startTime += Time.deltaTime;
            doubleJumpAble = true;
            yield return null;
        }
        doubleJumpAble = false;
    }
    public void StartOnDoubleJump(float time)
    {
        StartCoroutine(OnDoubleJump(time));
    }
    
    public void DecreaseSpeed(float amount)
    {
        moveSpeed -= amount;
    }
    public void UnDecreaseSpeed(float amount)
    {
        moveSpeed += amount;
    }
}
