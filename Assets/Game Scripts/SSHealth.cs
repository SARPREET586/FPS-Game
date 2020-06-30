using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SSHealth : MonoBehaviour
{
    public float health = 100;
    public GameObject deathEffect;
    void Start()
    {
        
    }

    
    public void ApplyDamage( float damage)
    {
        health = health - damage;
        Debug.Log("Now health is:" + health);

        if(health <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        Instantiate(deathEffect, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
