using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public ulong PlayerIndex;
    public GameObject ThirdPerosnVC;
    public GameObject FirstPersonVC;
    private PlayerInput Input;
    [SerializeField] float speed;
    private bool IsThirdPerson = true;
    public Rigidbody torso;
    public Rigidbody fist;
    public static List<PlayerMovement> PlayerList = new List<PlayerMovement>();
    public PlayerMovement ClosestPlayer;
    public Weapon CurrentWeapon;
    [SerializeField] MeshFilter WeaponMeshFileter;
    [SerializeField] MeshRenderer WeaponMeshRenderer;
    private void Start() {
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
        RandomPositionServerRpc(100);        
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        PlayerList.Remove(this);
    }
    void Update()
    {
        if (!IsOwner) return;
        MoveServerRpc(Input.Move.Move.ReadValue<Vector2>().x, Input.Move.Move.ReadValue<Vector2>().y);
        foreach (PlayerMovement player in PlayerList)
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
        if (Input.Move.Attack.WasPerformedThisFrame())
        {
            CurrentWeapon.MainServerRpc(ClosestPlayer.PlayerIndex, PlayerIndex);
        }
        if (Input.Move.Secondary.WasPerformedThisFrame())
        {
            CurrentWeapon.SecondaryServerRpc();
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
    [ServerRpc(RequireOwnership=false)]
    void MoveServerRpc(float x, float y)
    {
        torso.AddForce(new Vector3(x, 0, y) * speed * Time.deltaTime, ForceMode.Impulse);
    }

    [ServerRpc(RequireOwnership=false)]
    void RandomPositionServerRpc(int RandomHeight)
    {
        torso.transform.position = new Vector3(UnityEngine.Random.Range(GameManager.instance.Res / 2, GameManager.instance.Res), 100, UnityEngine.Random.Range(GameManager.instance.Res / 2, GameManager.instance.Res));
    }    

}
