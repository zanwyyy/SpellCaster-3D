using UnityEngine;

/// <summary>
/// Simple heal spell: when spawned, it immediately heals the player that cast it,
/// then destroys itself (can later be replaced with particle effects, etc.).
/// </summary>
public class HealSpell : MonoBehaviour
{
    public float healAmount = 30f;
    public float lifeTime = 2f;

    void Start()
    {
        // Try to heal the player (assumes the spell is spawned as a child of the player or at least knows the player via tag)
        PlayerHealth playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
        else
        {
            Debug.LogWarning("HealSpell could not find PlayerHealth in the scene.");
        }

        // Optional small lifetime for VFX
        Destroy(gameObject, lifeTime);
    }
}


