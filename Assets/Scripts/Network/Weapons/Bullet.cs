using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Network;
namespace Project.Network.Weapons
{
public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Rigidbody _rb;
    public int Damage;
    public void Shoot(Vector3 Direction) {
        this.gameObject.SetActive(true);
        _rb.AddForce(Direction * _speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<Player>(); //TODO: implement damage system
        }
        this.gameObject.SetActive(false);
    }

    public void DamagePlayer(ulong PlayerId)
    {
       Weapon.GetPlayerByID(PlayerId).GetComponent<Player>().Health.Value -= this.Damage;
    }
}
}