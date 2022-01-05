using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public int maxHP;
    public int currentHP;
    public float speed;
    public float tempSpeed;
    public Vector2 moveDirection;
    public Rigidbody2D body;
    public Animator animator;
    public bool dead = false;
    public Character()
    {
        

    }

    void Start()
    {
        tempSpeed = speed;
        currentHP = maxHP;
    }
    private void Update() {
        if(currentHP < 1 && !dead){Die(); dead=true;}
    }

    public void Move()
    {
        body.velocity = moveDirection * tempSpeed;
    }

    public abstract void Attack(Vector2 aimDirection);
    public abstract void TakeDamage(int incomingDamage);
    public abstract void Die();
}
