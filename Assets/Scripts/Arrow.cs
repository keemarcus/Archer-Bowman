using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;
    public Sprite arrowImpact;
    public AudioClip targetHit;
    public AudioClip wallHit;
    public AudioClip bowFired;
    public float initializationTime;
    public float timeSinceInitialization;
    public int damage = 1;
    Player player;
    public int volumeLevel;
    private void Awake() {
        // get the sfx volume
        volumeLevel = PlayerPrefs.GetInt("sfxVol");
        
        // play the fired sound effect
        AudioSource.PlayClipAtPoint(bowFired, transform.position, volumeLevel * .1f);

        // start the timer for despawning
        initializationTime = Time.timeSinceLevelLoad;

        // set sprite sorting order
        spriteRenderer.sortingOrder = 1;

        // find the player
        player = FindObjectOfType<Player>();
    }
    void Update()
    {
        timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;

        if(timeSinceInitialization > 20f){
            Destroy(this.gameObject);
        }

        if(player.transform.position.y - 1 > this.transform.position.y){
            spriteRenderer.sortingOrder = 3;
        }else{
            spriteRenderer.sortingOrder = 1;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        body.isKinematic = true;
        body.velocity = Vector2.zero;
        spriteRenderer.sprite = arrowImpact;

        // get the object that was hit
        Enemy enemy = other.collider.GetComponentInParent<Target>();
        if(enemy){
            enemy.TakeDamage(damage);
            Destroy(this.gameObject, 0.05f);

            // play the target hit sound effect
            AudioSource.PlayClipAtPoint(targetHit, transform.position, volumeLevel * .1f);
        }
        else{
            // play the wall hit sound effect
            AudioSource.PlayClipAtPoint(wallHit, transform.position, volumeLevel * .1f);
        }
    }
}
