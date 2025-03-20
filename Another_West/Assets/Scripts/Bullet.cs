using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10;
    Rigidbody rb;
    private int damage;
    public float timeDestroyBullet = 5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
        Destroy(gameObject,timeDestroyBullet);
    }

    public void SetDamage(int value)
    {
        this.damage = value;
    }

    public int GetDamage()
    {
        return damage;
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.Damage(damage);
            Destroy(gameObject);
        }
    }
}
