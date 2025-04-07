using UnityEngine;
using Unity.Netcode;

public class TowerPlacement : NetworkBehaviour
{
    [System.Serializable]
    public class TowerOption
    {
        public GameObject prefab;
        public int cost;
    }

    public TowerOption[] towerOptions;
    public LayerMask placementLayerMask;
    public AudioClip towerPlacementSound;
    private TextMesh costText;    
    private Camera mainCamera;
    private AudioSource audioSource;

    private int selectedTowerIndex = 0;
    private GameObject previewObject;

    private void Start()
    {
        mainCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {

        HandleTowerSwitching();
        UpdatePreviewPosition();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceTower();
        }
    }

    void HandleTowerSwitching()
    {
        for (int i = 0; i < towerOptions.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                selectedTowerIndex = i;
                CreatePreviewObject();
                Debug.Log($"Selected Tower: {towerOptions[selectedTowerIndex].prefab.name}");
            }
        }
    }

void CreatePreviewObject()
{
    if (previewObject != null) Destroy(previewObject);

    GameObject prefab = towerOptions[selectedTowerIndex].prefab;
    previewObject = Instantiate(prefab);
    Destroy(previewObject.GetComponent<NetworkObject>());
    previewObject.GetComponent<Collider2D>().enabled = false;

    // Make it transparent
    foreach (var renderer in previewObject.GetComponentsInChildren<SpriteRenderer>())
    {
        Color color = renderer.color;
        color.a = 0.5f;
        renderer.color = color;
    }

    // Add a floating TextMesh to show the cost
    GameObject costDisplay = new GameObject("CostDisplay");
    costDisplay.transform.SetParent(previewObject.transform);
    costDisplay.transform.localPosition = new Vector3(0, 1f, 0); // position above tower

    costText = costDisplay.AddComponent<TextMesh>();
    costText.text = $"${towerOptions[selectedTowerIndex].cost}";
    costText.characterSize = 0.25f;
    costText.fontSize = 30;
    costText.alignment = TextAlignment.Center;
    costText.anchor = TextAnchor.MiddleCenter;
    costText.color = Color.yellow;
}

    void UpdatePreviewPosition()
    {
        if (previewObject == null) return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        previewObject.transform.position = mousePos;

        // Optional: Color change if valid/invalid spot
        bool valid = Physics2D.OverlapPoint(mousePos, placementLayerMask);
        Color color = valid ? Color.green : Color.red;

        foreach (var renderer in previewObject.GetComponentsInChildren<SpriteRenderer>())
        {
            Color c = renderer.color;
            c.r = color.r;
            c.g = color.g;
            c.b = color.b;
            renderer.color = c;
        }
    }

    void TryPlaceTower()
    {
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(mousePos, placementLayerMask);
        TowerOption selectedTower = towerOptions[selectedTowerIndex];

        if (hitCollider != null && PlayerResources.Instance.CanAfford(selectedTower.cost))
        {
            RequestTowerPlacementServerRpc(mousePos, selectedTowerIndex);
            PlayTowerPlacementSoundClientRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestTowerPlacementServerRpc(Vector2 position, int towerIndex, ServerRpcParams rpcParams = default)
    {
        TowerOption tower = towerOptions[towerIndex];
        GameObject towerInstance = Instantiate(tower.prefab, position, Quaternion.identity);
        towerInstance.GetComponent<NetworkObject>().Spawn();
        PlayerResources.Instance.SpendResources(tower.cost);
    }

    [ClientRpc]
    void PlayTowerPlacementSoundClientRpc()
    {
        if (towerPlacementSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(towerPlacementSound);
        }
    }
} 