using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;
using Project.Network;
using UnityEngine.SceneManagement;
public class PlayerListObject : MonoBehaviour
{
    public ulong PlayerId;
    public TMP_Text PlayerNameText;
    public Button KickButton;
    private void Start() {
        KickButton.onClick.AddListener(KickPlayer);
        PlayerNameText.text = NetworkManager.Singleton.ConnectedClients[PlayerId].PlayerObject.GetComponent<Player>().PlayerName.Value.ToString();
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }
    public void KickPlayer()
    {
        NetworkManager.Singleton.DisconnectClient(PlayerId);
        Destroy(this.gameObject);
    }

    public void OnClientDisconnect(ulong player)
    {
        SceneManager.LoadScene("MainMenu");
    }

}
