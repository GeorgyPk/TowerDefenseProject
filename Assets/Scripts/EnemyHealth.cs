using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Stats")]
    public float maxHp = 10f;
    public int reward = 1;

    [Header("Audio")]
    public AudioClip hitSound;
    [Range(0f, 1f)] public float hitVolume = 0.6f;

    private float currentHp;
    private AudioSource audioSource;
    private float lastHitSoundTime = -999f;
    public float hitSoundCooldown = 0.1f;

    private void Awake()
    {
        currentHp = maxHp;
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.spatialBlend = 0f;
    }

    private void OnEnable()
    {
        currentHp = maxHp;
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;

        currentHp -= amount;

        PlayHitSound();

        if (currentHp <= 0f)
        {
            Die();
        }
    }

    private void PlayHitSound()
    {
        if (hitSound == null || audioSource == null) return;
        if (Time.time - lastHitSoundTime < hitSoundCooldown) return;

        lastHitSoundTime = Time.time;
        audioSource.PlayOneShot(hitSound, hitVolume);
    }

    private void Die()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(reward);

        var tracker = GetComponent<WaveEnemyTracker>();
        if (tracker != null && tracker.manager != null)
            tracker.manager.NotifyEnemyGone();

        Destroy(gameObject);
    }
}