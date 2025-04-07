using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private NetworkVariable<int> lives = new NetworkVariable<int>(10, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Text livesText;
    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    public AudioClip gameOverSound;
    public AudioClip victorySound;
    private AudioSource audioSource;
    
    public GameObject playMode;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (livesText != null)
        {
            livesText.text = "Lives: " + lives.Value.ToString();
        }
        CheckForVictory();
    }

    public void EnemyReachedGoal()
    {
        if (!IsServer) return;

        lives.Value--;

        if (lives.Value <= 0)
        {
            GameOverClientRpc();
        }
    }

    [ClientRpc]
    private void GameOverClientRpc()
    {
        if (gameOverPanel != null)
        {
            playModeStop();
            gameOverPanel.SetActive(true);
        }

        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartGameServerRpc()
    {
        RestartGameClientRpc();
    }

    [ClientRpc]
    private void RestartGameClientRpc()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        NetworkManager.Singleton.Shutdown();
    }

    private void CheckForVictory()
    {
        if (GameObject.FindObjectOfType<EnemySpawner>() == null && GameObject.FindObjectOfType<EnemyPathing>() == null)
        {
            VictoryClientRpc();
        }
    }

    [ClientRpc]
    private void VictoryClientRpc()
    {
        if (victoryPanel != null)
        {
            playModeStop();
            victoryPanel.SetActive(true);
        }

        if (victorySound != null && audioSource != null)
        {
            audioSource.PlayOneShot(victorySound);
        }
    }
        void playModeStop()
    {
        playMode.gameObject.SetActive(false);
    }
}
