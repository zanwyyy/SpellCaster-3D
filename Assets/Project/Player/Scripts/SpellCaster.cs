using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class SpellCaster : MonoBehaviour
{
    public Transform castPoint;
    [Header("Spells")]
    public GameObject fireballPrefab;
    public GameObject healSpellPrefab;
    public float castForce = 25f;
    public float cooldown = 0.5f;
    public InputActionAsset inputActions;

    float lastCastTime;
    Camera cam;
    private InputAction attackAction;
    private InputAction healAction;

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

        // Heal action (optional)
        healAction = playerMap.FindAction("Heal");
        if (healAction == null)
        {
            Debug.LogWarning("Could not find 'Heal' action in Player action map. Healing spell will be disabled until you add it.");
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

        if (healAction != null)
        {
            healAction.Enable();
            healAction.performed += OnHeal;
            Debug.Log("Heal action enabled!");
        }
    }

    void OnDisable()
    {
        if (attackAction != null)
        {
            attackAction.performed -= OnAttack;
            attackAction.Disable();
        }

        if (healAction != null)
        {
            healAction.performed -= OnHeal;
            healAction.Disable();
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        Debug.Log("Attack input received! Phase: " + context.phase);
        if (context.performed)
        {
            Debug.Log("Attack performed - casting Fireball");
            CastFireball();
        }
    }

    void OnHeal(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Heal performed - casting HealSpell");
            CastHeal();
        }
    }

    void CastFireball()
    {
        // Check cooldown
        if (Time.time < lastCastTime + cooldown)
            return;

        // Check if prefab is assigned
        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball Prefab is not assigned in SpellCaster!");
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
            fireballPrefab,
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

    void CastHeal()
    {
        // No cooldown for heal yet (or you can reuse the same cooldown check here)

        if (healSpellPrefab == null)
        {
            Debug.LogError("Heal Spell Prefab is not assigned in SpellCaster!");
            return;
        }

        Instantiate(
            healSpellPrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log("Heal spell cast!");
    }
}
