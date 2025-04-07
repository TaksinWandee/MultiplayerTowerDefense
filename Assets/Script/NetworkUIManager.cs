using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUIManager : MonoBehaviour
{
    public Button hostButton;
    public Button clientButton;
    public Button serverButton;
    public GameObject title;
    public GameObject playMode;
    void Start()
    {
        hostButton.onClick.AddListener(StartHost);
        clientButton.onClick.AddListener(StartClient);
        serverButton.onClick.AddListener(StartServer);
    }

    void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        playModeStart();
        HideText();
        HideButtons();
    }

    void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        playModeStart();
        HideText();
        HideButtons();
    }

    void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        playModeStart();
        HideText();
        HideButtons();
    }

    void HideButtons()
    {
        hostButton.gameObject.SetActive(false);
        clientButton.gameObject.SetActive(false);
        serverButton.gameObject.SetActive(false);
    }
        void HideText()
    {
        title.gameObject.SetActive(false);
    }
    void playModeStart()
    {
        playMode.gameObject.SetActive(true);
    }
}
