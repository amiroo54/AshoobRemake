using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class NetworkManagerUI : MonoBehaviour
{
    public Button btnHost;
    public Button btnServe;
    public Button btnClient;
    // Start is called before the first frame update
    void Start()
    {
        btnHost.onClick.AddListener(() => {NetworkManager.Singleton.StartHost();});
        btnServe.onClick.AddListener(() => {NetworkManager.Singleton.StartServer();});
        btnClient.onClick.AddListener(() => {NetworkManager.Singleton.StartClient();});

    }

   
}
