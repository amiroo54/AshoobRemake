using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class GunTypeWeapon : Weapon
{
    public int MagSize;
    public float ReloadTime;    
    private Bullet[] Bullets;
    public GameObject BulletPrefab;
    private int BulletIndex;
    [HideInInspector] public bool IsReloaded;
    [ServerRpc]
    public override void StartServerRpc() 
    {
        Debug.Log("Weapon inited");
        Bullets = new Bullet[MagSize];
        for (int i = 0; i < MagSize; i++)
        {
            Bullets[i] = GameObject.Instantiate(BulletPrefab).GetComponent<Bullet>();
            Bullets[i].gameObject.SetActive(false);
        }
        BulletIndex = 0;
    }
    [ServerRpc]
    public override void MainServerRpc(PlayerMovement PlayerToAttack, PlayerMovement AttackingPlayer)
    {
        if (IsReloaded)
        {
            Bullet bul = Bullets[BulletIndex];
            bul.transform.position = AttackingPlayer.fist.transform.position + AttackingPlayer.fist.transform.up * 3f;
            bul.transform.rotation = AttackingPlayer.fist.rotation;
            bul.Shoot();
            if (BulletIndex < MagSize - 1){
                BulletIndex++;
            }
            else
            {
                IsReloaded = false;
                BulletIndex = 0;
            }
        }
    }
    [ServerRpc]
    public override void SecondaryServerRpc()
    {
        Reload();
    }

    IEnumerator Reload()
    {
        Debug.Log("Reload Started");
        yield return new WaitForSeconds(ReloadTime);
        Debug.Log("Reload Finished");
        IsReloaded = true;
        BulletIndex = 0;
    }
    public override void NetworkSerialize<T>(BufferSerializer<T> serializer)
    {
        if (serializer.IsWriter)
        {
            serializer.GetFastBufferWriter().WriteValueSafe(IsReloaded);
            serializer.GetFastBufferWriter().WriteValueSafe(ReloadTime);
            serializer.GetFastBufferWriter().WriteValueSafe(BulletIndex);
        }
        else
        {
            serializer.GetFastBufferReader().ReadValueSafe(out IsReloaded);
            serializer.GetFastBufferReader().ReadValueSafe(out ReloadTime);
            serializer.GetFastBufferReader().ReadValueSafe(out BulletIndex);
        }
    }
}
