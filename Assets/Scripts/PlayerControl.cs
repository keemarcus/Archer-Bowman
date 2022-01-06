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
        crosshair = Instantiate(crosshairRef, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
    }

    void Update()
    {
        switch (currentState)
        {
            case PlayerState.Default:
                GetInputDefault(controllerMode);
                break;

            case PlayerState.Aiming:
                GetInputAiming(controllerMode);
                break;

            case PlayerState.Paused:
                // stop player movement
                moveDirection = Vector2.zero;
                break;
        }

        // update animator
        UpdateAnimator();

    }

    void GetInputDefault(bool controllerMode)
    {
        if(controllerMode){
            moveDirection = new Vector2(Input.GetAxis("Horizontal_LS"), moveDirection.y = Input.GetAxis("Vertical_LS"));
            aimDirection = new Vector2(Input.GetAxis("Horizontal_RS"), Input.GetAxis("Vertical_RS"));

            // check for attack attack
            if (Input.GetAxisRaw("RT") > 0)
            {
                // start aiming
                currentState = PlayerState.Aiming;
                StartCoroutine(AimWait());
            }
        }else{
            moveDirection = new Vector2(Input.GetAxisRaw("Horizontal"), moveDirection.y = Input.GetAxisRaw("Vertical"));
            aimDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;

            // check for attack attack
            if (Input.GetMouseButtonDown(0))
            {
                // start aiming
                currentState = PlayerState.Aiming;
                StartCoroutine(AimWait());
            }
        }
        moveDirection.Normalize();
        aimDirection.Normalize();
        aimDirection = aimDirection * 1.5f;
        crosshair.transform.position = (Vector2)body.transform.position + aimDirection;
        var crosshairAngle = Mathf.Atan2(aimDirection.x * -1, aimDirection.y) * Mathf.Rad2Deg;
        crosshair.transform.rotation = Quaternion.AngleAxis(crosshairAngle, Vector3.forward);
    }

    void GetInputAiming(bool controllerMode)
    {
        if(controllerMode){
            aimDirection = new Vector2(Input.GetAxis("Horizontal_RS"), Input.GetAxis("Vertical_RS"));
        }else{
            aimDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - body.transform.position;
        }
        aimDirection.Normalize();
        aimDirection = aimDirection * 1.5f;
        crosshair.transform.position = (Vector2)body.transform.position + aimDirection;
        var crosshairAngle = Mathf.Atan2(aimDirection.x * -1, aimDirection.y) * Mathf.Rad2Deg;
        crosshair.transform.rotation = Quaternion.AngleAxis(crosshairAngle, Vector3.forward);

        Aim(controllerMode);
    }

    void FixedUpdate()
    {
        // move player
        Move();
    }
}
