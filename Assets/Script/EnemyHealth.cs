using UnityEngine;
using Unity.Netcode;

public class EnemyHealth : NetworkBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;
    public AudioClip hitSound; // Sound effect for hitting target
    
    public AudioClip deathSound;
    private AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
        currentHealth = maxHealth;
    }

    [ServerRpc]  
    public void TakeDamageServerRpc(int damage) 
    {
        if (!IsServer) return;
        PlayHitSoundClientRpc();
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            PlayDeathSoundClientRpc();
             Invoke(nameof(Die), 0.5f);
        }
    }

    void Die()
    {
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
    
    [ClientRpc]
    void PlayHitSoundClientRpc()
    {
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }
    [ClientRpc]
    void PlayDeathSoundClientRpc()
    {
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
    }
}
