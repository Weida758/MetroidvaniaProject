using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInputsActions playerInputs;
    private InputActionMap inputMap;
    
    // ---------- Input Actions -------------
    private InputAction moveAction;
    private InputAction jumpAction;

    // ---------- Input Data ----------------
    public Vector2 moveInput { get; private set; }
    public bool jumpPressed { get; private set; }
    public bool jumpReleased { get; private set; }
    private void Awake()
    {
        playerInputs = new PlayerInputsActions();
        inputMap = playerInputs.Player;

        if (inputMap != null)
        {
            moveAction = inputMap.FindAction("Move");
            jumpAction = inputMap.FindAction("Jump");
        }
        else
        {
            Debug.LogError("No PlayerInputs found");
        }
    }

    private void OnEnable()
    {
        playerInputs.Enable();
    }

    private void OnDisable()
    {
        playerInputs.Disable();
    }


    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>().normalized;
        jumpPressed = jumpAction.WasPressedThisFrame();
        jumpReleased = jumpAction.WasReleasedThisFrame();
    }
    
    
    

}
