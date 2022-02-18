using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    public Animator anim;

    public int maxHealth = 100;

    double currentHealth;
    int timesPlayed = 0;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    private void Update() {
        if( timesPlayed < 0) {
            anim.SetBool("isDead", false);
            anim.SetBool("Dead", true);
            timesPlayed = 0;
            currentHealth = 0.01;
        }

    }

    public void takeDamage(int damage){

        currentHealth -= damage;
        anim.SetTrigger("Hurt");

        if(currentHealth <= 0)
        {
            Die();  
            
        }
    }

    void Die(){
    //disable
    anim.SetBool("isDead", true);
    timesPlayed++;
    //GetComponent<Collider2D>().enabled = false;
    this.enabled = false;

    
    
    }
   
}
