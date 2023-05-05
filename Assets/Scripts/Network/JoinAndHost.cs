using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;
using System.Net;
using System.Net.Sockets;
using Project.Network;
public class JoinAndHost : NetworkBehaviour
{
    [SerializeField] TMP_InputField _joinTextPort;
    [SerializeField] TMP_InputField _joinTextIP;
    [SerializeField] TMP_InputField _hostTextPort;
    [SerializeField] TMP_InputField _userName;
    [SerializeField] Transform _content; //for the rect slider.
    [SerializeField] GameObject _serverObjectPrefab;

    private void Start() {
        _userName.onValueChanged.AddListener((string name) => PlayerPrefs.SetString("Name", name));
        _userName.text = PlayerPrefs.GetString("Name", "");
    }
    public void Join()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (_joinTextIP.text != null)
        {
            ushort.TryParse(_joinTextIP.text, out transport.ConnectionData.Port);
        }else
        {
            transport.ConnectionData.Port = 7777;
        }
        transport.ConnectionData.Address = _joinTextIP.text;
        NetworkManager.Singleton.StartClient();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().PlayerName.Value = PlayerPrefs.GetString("Name", "Player" + Random.Range(0, 1000).ToString());
        //NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
    public void Host()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (_hostTextPort.text != null)
        {
            ushort.TryParse(_hostTextPort.text, out transport.ConnectionData.Port);
        }else
        {
            transport.ConnectionData.Port = 7777;
        }
        transport.ConnectionData.Address = GetLocalIPAddress();
        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<Player>().PlayerName.Value = PlayerPrefs.GetString("Name", "Player" + Random.Range(0, 1000).ToString());
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void Server()
    {
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (_hostTextPort.text != null)
        {
            ushort.TryParse(_hostTextPort.text, out transport.ConnectionData.Port);
        }else
        {
            transport.ConnectionData.Port = 7777;
        }
        NetworkManager.Singleton.StartServer();
        NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);

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
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                 //hintText.text = ip.ToString();
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}
