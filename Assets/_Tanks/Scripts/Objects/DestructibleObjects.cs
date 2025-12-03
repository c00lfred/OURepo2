using UnityEngine;

namespace Tanks.Complete
{
    /// <summary>
    /// Attach this to any obstacle / map object you want to be shootable/destructible.
    /// Set m_StartingHealth per-object in the inspector.
    /// Optionally assign m_DestructionPrefab (particle + audio) which will be instantiated when destroyed.
    /// </summary>
    [DisallowMultipleComponent]
    public class DestructibleObject : MonoBehaviour
    {
        [Tooltip("How much health this object starts with")]
        public float m_StartingHealth = 50f;

        [Tooltip("Optional particle/audio prefab to instantiate on destruction (e.g. break effect).")]
        public GameObject m_DestructionPrefab;

        [Tooltip("If true the object will be destroyed (Destroy). If false it will be deactivated (SetActive(false)).")]
        public bool m_DestroyOnDeath = true;

        private float m_CurrentHealth;
        private bool m_Dead;

        private void Awake()
        {
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
        }

        private void OnEnable()
        {
            // Reset health when re-enabled (useful for pooling)
            m_CurrentHealth = m_StartingHealth;
            m_Dead = false;
        }

        /// <summary>
        /// Apply damage to this object. If health <= 0 will trigger death.
        /// </summary>
        /// <param name="amount">Damage amount (positive value).</param>
        public void TakeDamage(float amount)
        {
            if (m_Dead) return;

            m_CurrentHealth -= amount;
            if (m_CurrentHealth <= 0f)
            {
                OnDeath();
            }
        }

        private void OnDeath()
        {
            m_Dead = true;

            // Spawn destruction prefab if one is set
            if (m_DestructionPrefab != null)
            {
                GameObject fx = Instantiate(m_DestructionPrefab, transform.position, Quaternion.identity);
                // If prefab has a ParticleSystem, play it and destroy the prefab after its duration
                var ps = fx.GetComponentInChildren<ParticleSystem>();
                if (ps != null)
                {
                    var main = ps.main;
                    Destroy(fx, main.duration + main.startLifetime.constantMax);
                }
                else
                {
                    // Fallback: destroy after 5 seconds if no particle system present
                    Destroy(fx, 5f);
                }
            }

            // Remove or disable this object
            if (m_DestroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Optionally expose a way to change max/cur health at runtime.
        /// </summary>
        public void SetHealth(float newHealth)
        {
            m_StartingHealth = newHealth;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0f, m_StartingHealth);
            if (m_CurrentHealth <= 0f && !m_Dead)
                OnDeath();
        }

        /// <summary>
        /// For debugging and editor convenience.
        /// </summary>
        public float CurrentHealth => m_CurrentHealth;
        public float MaxHealth => m_StartingHealth;
        public bool IsDead => m_Dead;
    }
}