using UnityEngine;
using UnityEngine.InputSystem;

public class BodyBob : MonoBehaviour
{
    public float bobSpeed = 6f;
    public float bobAmount = 0.05f;
    public InputActionAsset inputActions;

    private Vector3 startPos;
    private InputAction moveAction;

    void Awake()
    {
        // Get move action from InputActionAsset
        if (inputActions != null)
        {
            InputActionMap playerMap = inputActions.FindActionMap("Player");
            if (playerMap != null)
            {
                moveAction = playerMap.FindAction("Move");
            }
        }
    }

    void Start()
    {
        startPos = transform.localPosition;
    }

    void OnEnable()
    {
        moveAction?.Enable();
    }

    void OnDisable()
    {
        moveAction?.Disable();
    }

    void Update()
    {
        Vector2 moveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
        float move = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);

        if (move > 0.1f)
        {
            float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
            transform.localPosition = startPos + Vector3.up * bob;
        }
        else
        {
            transform.localPosition = startPos;
        }
    }
}
