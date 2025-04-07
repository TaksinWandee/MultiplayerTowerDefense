using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PlayerResources : NetworkBehaviour
{
    public static PlayerResources Instance;

    private NetworkVariable<int> gold = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public Text goldText;
    public AudioClip addGoldSound;
    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold.Value.ToString();
        }
    }

    public bool CanAfford(int cost)
    {
        return gold.Value >= cost;
    }

    public void SpendResources(int amount)
    {
        if (!IsServer) return;
        gold.Value -= amount;
    }

    public void AddResources(int amount)
    {
        if (!IsServer)
        {
            AddResourcesServerRpc(amount);
        }
        else
        {
            gold.Value += amount;
            PlayAddGoldSoundClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddResourcesServerRpc(int amount, ServerRpcParams rpcParams = default)
    {
        gold.Value += amount;
        UpdateGoldUIClientRpc();
        PlayAddGoldSoundClientRpc();
    }

    [ClientRpc]
    public void UpdateGoldUIClientRpc()
    {
        if (goldText != null)
        {
            goldText.text = "Gold: " + gold.Value.ToString();
        }
    }

    [ClientRpc]
    void PlayAddGoldSoundClientRpc()
    {
        if (audioSource != null && addGoldSound != null)
        {
            audioSource.PlayOneShot(addGoldSound);
        }
    }
}
