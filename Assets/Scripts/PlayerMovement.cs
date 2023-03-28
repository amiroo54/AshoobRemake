using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour, INetworkSerializable
{
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
        PlayerList.Add(this);
        RandomPositionServerRpc(100);
        WeaponChange(CurrentWeapon);
        
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
            CurrentWeapon.MainServerRpc(ClosestPlayer, this);
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
    [ServerRpc]
    void MoveServerRpc(float x, float y)
    {
        torso.AddForce(new Vector3(x, 0, y) * speed * Time.deltaTime, ForceMode.Impulse);
    }

    [ServerRpc]
    void RandomPositionServerRpc(int RandomHeight)
    {
        torso.transform.position = new Vector3(UnityEngine.Random.Range(GameManager.instance.Res / 2, GameManager.instance.Res), 100, UnityEngine.Random.Range(GameManager.instance.Res / 2, GameManager.instance.Res));
    }    
    public void WeaponChange(Weapon weapon)
    {
        CurrentWeapon = weapon;
        CurrentWeapon.StartServerRpc();
        WeaponMeshFileter.mesh = CurrentWeapon.displayMesh;
        WeaponMeshRenderer.material = CurrentWeapon.material;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(ClosestPlayer);
            serializer.GetFastBufferWriter().WriteValueSafe(CurrentWeapon);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out ClosestPlayer);
            serializer.GetFastBufferWriter().WriteValueSafe(CurrentWeapon);
        }
    }
}
