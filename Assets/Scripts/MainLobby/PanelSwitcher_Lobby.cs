using UnityEngine;

public class PanelSwitcher_Lobby : MonoBehaviour
{
    public GameObject MainLobbyPanel;
    public GameObject NamedPetPanel;

    void Start()
    {
        NamedPetPanel.SetActive(true);
        MainLobbyPanel.SetActive(false);
    }
}

   