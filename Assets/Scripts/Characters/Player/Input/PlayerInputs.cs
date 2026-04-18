using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    private PlayerInputsActions playerInputs;
    private InputActionMap inputMap;
    
    // ---------- Input Actions -------------
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction downAction;
    private InputAction shiftAction;
    private InputAction oneAction;
    private InputAction twoAction;
    private InputAction threeAction;
    private InputAction fourAction;
    private InputAction attackAction;

    // ---------- Input Data ----------------
    public Vector2 moveInput { get; private set; }

    public bool jumpPressed { get; private set; }
    public bool jumpReleased { get; private set; }

    public bool downPressed { get; private set; }
    public bool shiftPressed { get; private set; }
    public bool shiftReleased { get; private set; }

    public bool onePressed { get; private set; }
    public bool twoPressed { get; private set; }
    public bool threePressed { get; private set; }
    public bool fourPressed { get; private set; }

    public bool attackPressed { get; private set; }

    private void Awake()
    {
        playerInputs = new PlayerInputsActions();
        inputMap = playerInputs.Player;

        if (inputMap != null)
        {
            moveAction = inputMap.FindAction("Move");
            jumpAction = inputMap.FindAction("Jump");
            downAction = inputMap.FindAction("Down");
            shiftAction = inputMap.FindAction("Shift");
            oneAction = inputMap.FindAction("Sword");
            twoAction = inputMap.FindAction("Dagger");
            threeAction = inputMap.FindAction("Spear");
            fourAction = inputMap.FindAction("Hammer");
            attackAction = inputMap.FindAction("Attack");
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
        downPressed = downAction.WasPressedThisFrame();
        shiftPressed = shiftAction.WasPressedThisFrame();
        shiftReleased = shiftAction.WasReleasedThisFrame();

        onePressed = oneAction.WasPressedThisFrame();
        twoPressed = twoAction.WasPressedThisFrame();
        threePressed = threeAction.WasPressedThisFrame();
        fourPressed = fourAction.WasPressedThisFrame();
        attackPressed = attackAction.WasPressedThisFrame();

    }
    
    
    

}
