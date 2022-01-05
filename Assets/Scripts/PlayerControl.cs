using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : Player
{
    void Start()
    {
        tempSpeed = speed;
        currentHP = maxHP;
        currentState = PlayerState.Default;
        crosshair = Instantiate(crosshairRef, new Vector3(transform.position.x, transform.position.y, transform.position.z),Quaternion.identity);
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    void Update()
    {
        switch(currentState)
        {
            case PlayerState.Default:
                GetMouse();
                
                // check for movement input
                moveDirection.x = Input.GetAxisRaw("Horizontal"); // -1 is left
                moveDirection.y = Input.GetAxisRaw("Vertical"); // -1 is down
                moveDirection.Normalize();

                // check for attack attack
                if(Input.GetMouseButtonDown(0))
                {
                    // start aiming
                    currentState = PlayerState.Aiming;
                    StartCoroutine(AimWait());
                }
            break;

            case PlayerState.EndGame:
                // stop player movement
                moveDirection = Vector2.zero;
            break;

            case PlayerState.Aiming:
                GetMouse();
                Aim();
            break;
        }

        // update animator
        UpdateAnimator();
        
    }

    void GetMouse(){
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
        mousePosition.Normalize();
        moveDirection = mousePosition;
        mousePosition = mousePosition * 1.5f;
        crosshair.transform.position = (Vector2)body.transform.position + mousePosition;
        var crosshairAngle = Mathf.Atan2(mousePosition.x * -1, mousePosition.y) * Mathf.Rad2Deg;
        crosshair.transform.rotation = Quaternion.AngleAxis(crosshairAngle, Vector3.forward);
    }

    void FixedUpdate()
    {
        // move player
        Move(); 
    }
}
