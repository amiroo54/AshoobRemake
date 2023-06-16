using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
namespace Project.Network.Weapons
{
public class GunTypeWeapon : Weapon
{
    public int MagSize;
    public float ReloadTime;    
    private Bullet[] Bullets;
    public GameObject BulletPrefab;
    private int BulletIndex;
    [SerializeField] Transform _barrel;
    public bool IsReloaded = true;
    [ServerRpc]
    public override void StartServerRpc() 
    {
        base.StartServerRpc();
        Bullets = new Bullet[MagSize];
        for (int i = 0; i < MagSize; i++)
        {
            Bullets[i] = Instantiate(BulletPrefab, transform.position, Quaternion.identity).GetComponent<Bullet>();
            Bullets[i].GetComponent<NetworkObject>().Spawn();
            Bullets[i].gameObject.SetActive(false);
        }
        BulletIndex = 0;
    }
    [ServerRpc]
    public override void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer)
    {
        if (IsReloaded)
        {
            Bullet bul = Bullets[BulletIndex];
            bul.transform.position = _barrel.position;
            bul.Damage = Damage;
            bul.Shoot(_barrel.transform.forward);
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
       StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(ReloadTime);
        IsReloaded = true;
        BulletIndex = 0;
    }
}
}