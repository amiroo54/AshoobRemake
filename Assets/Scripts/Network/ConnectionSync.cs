using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParrelSync;
using Unity.Netcode;
public class ConnectionSync : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(!ClonesManager.IsClone())
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
    }

   
}
