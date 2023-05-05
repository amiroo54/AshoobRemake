using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using Project.Network.Weapons;
using Unity.Netcode.Components;
using Unity.Collections;
namespace Project.Network
{
public class Player : NetworkBehaviour
{
    public int StartingHealth;
    public NetworkVariable<int> Health;
    public ulong PlayerIndex;
    public GameObject ThirdPerosnVC;
    public GameObject FirstPersonVC;
    private PlayerInput Input;
    [SerializeField] float speed;
    private bool IsThirdPerson = true;
    public Rigidbody torso;
    public Rigidbody fist;
    public static List<Player> PlayerList = new List<Player>();
    public Player ClosestPlayer;
    public Weapon CurrentWeapon;
    public Transform WeaponHolder;
    public NetworkVariable<FixedString128Bytes> PlayerName;
    private void Start() 
    {
        Health.Value = StartingHealth;
        if (!IsOwner) return;
        Input = new PlayerInput();
        Input.Enable();
        Input.Move.ChangePOV.performed += ChangeCam;
        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
        ThirdPerosnVC.SetActive(true);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        PlayerIndex = NetworkManager.Singleton.LocalClientId;
        PlayerList.Add(this);
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        PlayerList.Remove(this);
    }
    void Update()
    {
        if (!IsOwner) return;
        //moving
        MoveServerRpc(Input.Move.Move.ReadValue<Vector2>().x, Input.Move.Move.ReadValue<Vector2>().y);
        //getting the closet player
        foreach (Player player in PlayerList)
        {
            if (ClosestPlayer == null) {ClosestPlayer = player; return;}
            if (player == this) continue;
            else
            {
                if (Vector3.Distance(player.gameObject.transform.position, this.transform.position) >= Vector3.Distance(ClosestPlayer.gameObject.transform.position, this.transform.position))
                {
                    ClosestPlayer = player;
                }
            }
        }
        //attaking
        if (Input.Move.Attack.WasPerformedThisFrame())
        {
            Debug.Log("Attack from player");
            CurrentWeapon.MainServerRpc(ClosestPlayer.PlayerIndex, PlayerIndex);
        }
        if (Input.Move.Secondary.WasPerformedThisFrame())
        {
            CurrentWeapon.SecondaryServerRpc();
        }
        if (Health.Value <= 0)
        {
            Die();
        }
    }

    void ChangeCam(InputAction.CallbackContext context)
    {
        if (IsThirdPerson)
        {
            ThirdPerosnVC.SetActive(false);
            FirstPersonVC.SetActive(true);
            IsThirdPerson = false;
        }
        else
        {
            ThirdPerosnVC.SetActive(true);
            FirstPersonVC.SetActive(false);
            IsThirdPerson = true;
        }
    }
    [ServerRpc]
    public void ChangeWeaponServerRpc(ulong WeaponIndex)
    {
        CurrentWeapon = GetNetworkObject(WeaponIndex).GetComponent<Weapon>();
        CurrentWeapon.Parent = WeaponHolder;
        CurrentWeapon.StartServerRpc();
    }
    [ServerRpc]
    void MoveServerRpc(float x, float y)
    {
        torso.AddForce(new Vector3(x, 0, y) * speed * Time.deltaTime, ForceMode.Impulse);
    }

    private void Die()
    {
        Input.Disable();
        
    }
}
}