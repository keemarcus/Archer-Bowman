using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Character
{
    public SpriteRenderer spriteRenderer;
    protected Player player;
    private void Update() {
        if(currentHP < 1 && !dead){Die(); dead=true;}

        if(player.transform.position.y - .5 > this.transform.position.y){
            spriteRenderer.sortingOrder = 3;
        }else{
            spriteRenderer.sortingOrder = 1;
        }
    }
}
