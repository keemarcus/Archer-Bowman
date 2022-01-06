using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenController : Player
{
    public float initializationTime;
    public float timeSinceInitialization;
    public GameObject target;
    private bool spawned = false;
    private bool attacked = false;
    
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<SpriteRenderer>().sortingOrder = 2;
        aimDirection = Vector3.zero - body.transform.position;
        aimDirection.Normalize();

        // start the timer
        initializationTime = Time.timeSinceLevelLoad;
        this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        if(timeSinceInitialization > 3.5f){
            SceneManager.LoadScene("Menu");
        }
        else if(!attacked && timeSinceInitialization > 2.5f){
            Attack(moveDirection);
            attacked = true;
            currentState = PlayerState.Default;
            moveDirection = Vector2.zero;
        } else if(!spawned && timeSinceInitialization > 1f){
            this.gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
            GameObject newTarget = Instantiate(target, new Vector3(5,0,0), Quaternion.identity);
            newTarget.GetComponentInChildren<Target>().UpdateType(3);
            currentState = PlayerState.Aiming;
            moveDirection = aimDirection;
            spawned = true;
        }
        
        // update animator
        UpdateAnimator();
    }
}
