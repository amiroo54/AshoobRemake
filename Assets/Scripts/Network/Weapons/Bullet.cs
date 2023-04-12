using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Project.Network;
public class Bullet : MonoBehaviour
{
    [SerializeField] float _speed;
    [SerializeField] Rigidbody _rb;
    public void Shoot(Vector3 Direction) {
        this.gameObject.SetActive(true);
        _rb.AddForce(Direction * _speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<PlayerMovement>(); //TODO: implement damage system
        }
        this.gameObject.SetActive(false);
    }
}
