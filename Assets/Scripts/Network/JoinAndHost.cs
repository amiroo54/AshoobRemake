using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
public class JoinAndHost : MonoBehaviour
{
    [SerializeField] TMP_InputField _joinTextPort;
    [SerializeField] TMP_InputField _joinTextIP;
    [SerializeField] TMP_InputField _hostTextPort;
    [SerializeField] Transform _content; //for the rect slider.
    [SerializeField] GameObject _serverObjectPrefab;
    public void Join()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ushort.TryParse(_joinTextPort.text, out transport.ConnectionData.Port);
        transport.ConnectionData.Address = _joinTextIP.text;
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartClient();
    }
    public void Host()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ushort.TryParse(_hostTextPort.text, out transport.ConnectionData.Port);
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartHost();
    }

    public void Server()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        ushort.TryParse(_hostTextPort.text, out transport.ConnectionData.Port);
        SceneManager.LoadScene(1);
        NetworkManager.Singleton.StartServer();
    }
    public void RefreshServerList()
    {
        for (int ip = 1; ip < 254; ip++)
        {
            
        }
    }
    public void Addserver(ServerData data)
    {
        if (PlayerPrefs.HasKey("Servers"))
        {
            string prevval = PlayerPrefs.GetString("Servers");
            List<ServerData> datas = JsonUtility.FromJson<List<ServerData>>(prevval);
            datas.Add(data);
            PlayerPrefs.SetString("Servers", JsonUtility.ToJson(datas));
        }
        else
        {
            List<ServerData> list = new List<ServerData>();
            list.Add(data);
            PlayerPrefs.SetString("Servers", JsonUtility.ToJson(list));
        }
    }

    public void AddServerToServerList(ServerData data)
    {
        Instantiate(_serverObjectPrefab, _content);
    }
    public struct ServerData
    {
        public ushort Port;
        public string IP;
    }
}
