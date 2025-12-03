using UnityEngine;

public class SimpleObjectHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float startingHealth = 50f;
    private float currentHealth;

    [Header("Explosion")]
    public GameObject explosionPrefab;
    private ParticleSystem explosionParticles;
    private AudioSource explosionAudio;

    private bool isDead = false;

    private void Awake()
    {
        currentHealth = startingHealth;

        if (explosionPrefab != null)
        {
            explosionParticles = Instantiate(explosionPrefab).GetComponent<ParticleSystem>();
            explosionAudio = explosionParticles.GetComponent<AudioSource>();
            explosionParticles.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (explosionParticles != null)
            Destroy(explosionParticles.gameObject);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0f && !isDead)
            Die();
    }

    private void Die()
    {
        isDead = true;

        if (explosionParticles != null)
        {
            explosionParticles.transform.position = transform.position;
            explosionParticles.gameObject.SetActive(true);
            explosionParticles.Play();
            explosionAudio?.Play();
        }

        Destroy(gameObject);
    }
}

