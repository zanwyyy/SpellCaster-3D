using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class SpellCaster : MonoBehaviour
{
    public Transform castPoint;
    public GameObject spellPrefab;
    public float castForce = 25f;
    public float cooldown = 0.5f;
    public InputActionAsset inputActions;

    float lastCastTime;
    Camera cam;
    private InputAction attackAction;

    void Awake()
    {
        cam = GetComponentInChildren<Camera>();
        
        // Get attack action from InputActionAsset
        if (inputActions == null)
        {
            Debug.LogError("Input Actions Asset is not assigned in SpellCaster! Please assign it in Inspector.");
            return;
        }

        InputActionMap playerMap = inputActions.FindActionMap("Player");
        if (playerMap == null)
        {
            Debug.LogError("Could not find 'Player' action map in Input Actions Asset!");
            Debug.Log("Available action maps: " + string.Join(", ", inputActions.actionMaps.Select(m => m.name)));
            return;
        }

        attackAction = playerMap.FindAction("Attack");
        if (attackAction == null)
        {
            Debug.LogError("Could not find 'Attack' action in Player action map!");
            Debug.Log("Available actions in Player map: " + string.Join(", ", playerMap.actions.Select(a => a.name)));
            return;
        }

        Debug.Log("Attack action found successfully!");
    }

    void OnEnable()
    {
        if (attackAction != null)
        {
            attackAction.Enable();
            attackAction.performed += OnAttack;
            Debug.Log("Attack action enabled!");
        }
        else
        {
            Debug.LogError("Attack action is NULL! Make sure Input Actions is assigned in Inspector.");
        }
    }

    void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.performed -= OnAttack;
            attackAction.Disable();
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack input received! Phase: " + context.phase);
        if (context.performed)
        {
            Debug.Log("Attack performed - calling Cast()");
            Cast();
        }
    }

    void Cast()
    {
        // Check cooldown
        if (Time.time < lastCastTime + cooldown)
            return;

        // Check if prefab is assigned
        if (spellPrefab == null)
        {
            Debug.LogError("Spell Prefab is not assigned in SpellCaster!");
            return;
        }

        // Check if cast point is assigned
        if (castPoint == null)
        {
            Debug.LogError("Cast Point is not assigned in SpellCaster!");
            return;
        }

        // Instantiate spell
        GameObject spell = Instantiate(
            spellPrefab,
            castPoint.position,
            Quaternion.LookRotation(cam.transform.forward)
        );

        // Change color to bright red/orange for visibility
        Renderer renderer = spell.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = new Material(renderer.material);
            mat.color = new Color(1f, 0.3f, 0f, 1f); // Bright orange-red
            renderer.material = mat;
        }

        // Get Rigidbody and make the fireball fly straight along the camera forward
        Rigidbody rb = spell.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Fireball prefab is missing Rigidbody component! Adding one now...");
            rb = spell.AddComponent<Rigidbody>();
        }

        // Disable gravity so the fireball doesn't drop and make it move straight forward
        rb.useGravity = false;
        rb.linearVelocity = cam.transform.forward * castForce;

        lastCastTime = Time.time;
        Debug.Log("Fireball cast! Position: " + castPoint.position);
    }
}
