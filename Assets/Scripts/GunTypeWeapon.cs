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
    public override void MainServerRpc(ulong PlayerToAttack, ulong AttackingPlayer)
    {
        if (IsReloaded)
        {
            Bullet bul = Bullets[BulletIndex];
            PlayerMovement p = GetPlayerByID(AttackingPlayer).GetComponent<PlayerMovement>();
            bul.transform.position = p.fist.transform.position + p.fist.transform.up * 3f;
            bul.transform.rotation = p.fist.rotation;
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
}
