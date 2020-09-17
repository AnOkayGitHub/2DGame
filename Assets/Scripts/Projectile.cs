using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;

    private void Start()
    {
        Destroy(this, 3f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }
}
