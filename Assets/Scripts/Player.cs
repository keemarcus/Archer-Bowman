using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    public PlayerState currentState;
    public Vector2 aimDirection;
    public bool waiting = false;
    
    public GameObject arrow;
    public GameObject crosshairRef;
    public bool controllerMode;
    protected GameObject crosshair;
    public float projectileSpeed = 2000;
    public void Aim(bool controllerMode)
    {
        // disable movement
        tempSpeed = 0f;
        if(controllerMode){
            if(Input.GetAxisRaw("RT") == 0 && !waiting)
            {
                // fire
                Attack(aimDirection);
                currentState = PlayerState.Default;
                tempSpeed = speed;
            }
        } else{
            if(Input.GetMouseButton(0) == false && !waiting)
            {
                // fire
                Attack(aimDirection);
                currentState = PlayerState.Default;
                tempSpeed = speed;
            }
        }
    }

    public void UpdateAnimator()
    {
        switch(currentState)
        {
            case PlayerState.Default:
                animator.SetBool("Aiming", false);
                if(moveDirection != Vector2.zero)
                {
                    animator.SetBool("Walking", true);
                    animator.SetFloat("X", moveDirection.x);
                    animator.SetFloat("Y", moveDirection.y);
                }
                else
                {
                    animator.SetBool("Walking", false);
                }
            break;

            case PlayerState.Paused:
                animator.SetBool("Walking", false);
                animator.SetFloat("Y", -1);
                animator.SetFloat("X", 0);
            break;

            case PlayerState.Aiming:
                animator.SetBool("Aiming", true);
                animator.SetFloat("X", aimDirection.x);
                animator.SetFloat("Y", aimDirection.y);
            break;
        }
    }

    public override void Attack(Vector2 aimDirection)
    {
        var arrowAngle = Mathf.Atan2(aimDirection.x * -1, aimDirection.y) * Mathf.Rad2Deg;
        GameObject newArrow = Instantiate(arrow, new Vector3(transform.position.x, transform.position.y, transform.position.z),Quaternion.AngleAxis(arrowAngle, Vector3.forward));
        newArrow.GetComponent<Rigidbody2D>().freezeRotation = true;
        Physics2D.IgnoreCollision(newArrow.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        newArrow.GetComponent<Rigidbody2D>().AddForce(aimDirection * projectileSpeed);

        GameObject [] arrows = GameObject.FindGameObjectsWithTag("Arrow");
        if(arrows.Length > 5){
            // find the oldest arrow
            GameObject oldestArrow = arrows[0];
            foreach(GameObject arrow in arrows){
                if(arrow.GetComponent<Arrow>().timeSinceInitialization > oldestArrow.GetComponent<Arrow>().timeSinceInitialization){oldestArrow = arrow;}
            }
            // destroy oldest arrow
            Destroy(oldestArrow);
        }
    }

    public override void TakeDamage(int incomingDamage)
    {
        // we don't need to do anything here yet
        
    }

    public override void Die()
    {
        // we don't need to do anything here yet
        
    }

    public IEnumerator AimWait()
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(0.5f);
        waiting = false;
    }

    public enum PlayerState
    {
        Default,
        Paused,
        Aiming
    }
}
