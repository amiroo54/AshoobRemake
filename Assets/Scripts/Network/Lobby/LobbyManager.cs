using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Project.Network 
{
public class LobbyManager : NetworkBehaviour
{
    [SerializeField] MapSpawnManager _mainMap;
    [SerializeField] GameObject _playerMenuPrefab;
    [SerializeField] Transform _playerListContent;
    [SerializeField] GameObject _gameManager;
    [SerializeField] GameObject _lobbyPanel;
    [SerializeField] Button _closeHost;
    private void Start() {
        if (IsHost)
        GetComponent<NetworkObject>().Spawn();
        else Destroy(this.gameObject);
    }
    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnPlayerSpawn;
        _closeHost.onClick.AddListener(CloseHost);
        PlayerListObject p = Instantiate(_playerMenuPrefab, _playerListContent).GetComponent<PlayerListObject>();
        p.KickButton.enabled = false;
        p.name = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().PlayerName.Value.ToString();
        Debug.Log("Nothost");
        if (IsHost)
        {
            Debug.Log("started as host");
            NetworkManager.Singleton.LocalClient.PlayerObject.transform.position = transform.position + new Vector3(0, 20, 0);

        }
    }
    void OnPlayerSpawn(ulong player)
    {
        NetworkManager.Singleton.ConnectedClients[player].PlayerObject.transform.position = transform.position + new Vector3(0, 20, 0);
        PlayerListObject p = Instantiate(_playerMenuPrefab, _playerListContent).GetComponent<PlayerListObject>();
        p.name = NetworkManager.Singleton.ConnectedClients[player].PlayerObject.GetComponent<Player>().PlayerName.Value.ToString();
        p.PlayerId = player;
        if (!IsHost)
        {
            p.KickButton.enabled = false;
        }
        
    }
    public void StartGame()
    {
        _gameManager.SetActive(true);
        _lobbyPanel.SetActive(false);
        _mainMap.SpawnMaps();
        foreach (var player in NetworkManager.Singleton.ConnectedClientsList)
        {
            Transform t = player.PlayerObject.transform;
            t.position = new Vector3(UnityEngine.Random.Range(_mainMap.Res / -2, _mainMap.Res / 2), 20, UnityEngine.Random.Range(_mainMap.Res / -2, _mainMap.Res / 2));
        }
    }
    public void CloseHost()
    {
        NetworkManager.Singleton.Shutdown();
        Destroy(NetworkManager.Singleton.gameObject);
        Debug.Log("Shutdown");
        SceneManager.LoadScene("MainMenu");
        
    }
    
}
}