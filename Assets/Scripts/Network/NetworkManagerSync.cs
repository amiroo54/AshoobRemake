using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using ParrelSync;
namespace Project.Network
{
public class NetworkManagerSync : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (ClonesManager.IsClone())
        {
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            NetworkManager.Singleton.StartHost();
        }
    }

   
}
}