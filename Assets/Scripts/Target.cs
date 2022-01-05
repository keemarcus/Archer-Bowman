using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Target : Enemy
{
    Text scoreCounter;
    GameController gameController;
    // type identifier, 0 = reg, 1 = time bonus, 2 = point bonus
    public int typeID = 0;
    private void Awake() {
        // get the score counter
        Text[] textObjects = FindObjectsOfType<Text>();
        foreach(Text x in textObjects){
            if(x.name == "Score"){scoreCounter = x;}
        }
        // get the gamecontroller object
        gameController = FindObjectOfType<GameController>();
        // set sprite sorting order
        spriteRenderer.sortingOrder = 1;
        // find the player
        player = FindObjectOfType<Player>();
    }
    public void UpdateType(int newType){
        // update the type identifier
        typeID = newType;
        // update the animator
        this.animator.SetInteger("Type", newType);
    }
    public override void TakeDamage(int incomingDamage)
    {
        this.currentHP -= incomingDamage;
    }

    public override void Die()
    {
        // change animation to impact animation
        animator.SetBool("Dead", true);

        // destroy this object after the animation is done
        Destroy(this.gameObject, 0.05f);

        if(typeID != 3){
            // increment the score counter by 1
        string [] scoreElements = scoreCounter.text.Split(':');
        int newScore = int.Parse(scoreElements[1]);
        if(typeID == 2){
            // add 5 to the score
            newScore += 5;
        }else if(typeID == 1){
            // add 5 seconds to the time remaining
            gameController.timeRemaining += 5f;
        }else{
            // add one to the score
            newScore += 1;
        }
        
        scoreCounter.text = scoreElements[0] + ": " + newScore.ToString();
        // use the GameController script to spawn a new target
        FindObjectOfType<GameController>().SpawnTarget();
        }
        
    }

    public override void Attack(Vector2 aimDirection)
    {
        // we don't need to do anything here yet
    }
}
