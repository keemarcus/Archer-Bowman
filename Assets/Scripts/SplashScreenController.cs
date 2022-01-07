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
        // set up the default values for playerprefs if they don't exist
        if(!PlayerPrefs.HasKey("highScore")){
            PlayerPrefs.SetInt("highScore", 0);
        }
        if(!PlayerPrefs.HasKey("musicVol")){
            PlayerPrefs.SetInt("musicVol", 10);
        }
        if(!PlayerPrefs.HasKey("sfxVol")){
            PlayerPrefs.SetInt("sfxVol", 10);
        }
        if(!PlayerPrefs.HasKey("inputMode")){
            PlayerPrefs.SetInt("inputMode", 0);
        }
        PlayerPrefs.Save();
        
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
