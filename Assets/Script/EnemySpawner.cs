using UnityEngine;
using Unity.Netcode;

public class EnemySpawner : NetworkBehaviour
{
    public GameObject[] enemyPrefabs; // Array of enemy variants
    public Transform spawnPoint;
    public float spawnRate = 2f;
    public int maxEnemies = 10;

    public AudioClip spawnSound; // Sound effect for spawning
    private AudioSource audioSource;

    private int enemiesSpawned = 0;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Get the AudioSource component
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InvokeRepeating(nameof(SpawnEnemyServerRpc), 1f, spawnRate);
        }
    }

    [ServerRpc]
    public void SpawnEnemyServerRpc()
    {
        if (enemiesSpawned >= maxEnemies)
        {
            DestroySpawnerServerRpc();
            return;
        }

        if (enemyPrefabs.Length == 0) return;

        // Pick a random enemy variant
        int randomIndex = Random.Range(0, enemyPrefabs.Length);
        GameObject selectedEnemy = enemyPrefabs[randomIndex];

        GameObject enemy = Instantiate(selectedEnemy, spawnPoint.position, Quaternion.identity);
        enemy.GetComponent<NetworkObject>().Spawn();
        enemiesSpawned++;

        // Play the spawn sound effect (only on the server)
        PlaySpawnSoundClientRpc();
    }

    [ClientRpc]
    void PlaySpawnSoundClientRpc()
    {
        if (spawnSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    [ServerRpc]
    void DestroySpawnerServerRpc()
    {
        CancelInvoke(nameof(SpawnEnemyServerRpc)); // Stop spawning
        Destroy(gameObject); // Destroy the spawner
    }
    
}

