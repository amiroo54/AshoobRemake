using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed;
    public Rigidbody rb;
    public void Shoot() {
        this.gameObject.SetActive(true);
        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerMovement>(); //TODO: implement damage system
        }
        this.gameObject.SetActive(false);
    }
}
