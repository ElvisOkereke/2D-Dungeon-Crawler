using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    
    public Transform DoubleAttackHB;

    public Transform HeavyAttackHB;

    public float doubleAttackRange = 0.5f;

    public float heavyAttackRange = 0.5f;

    public LayerMask enemyLayers;


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K)) { //double attack
            doubleAttack();
        }

        if(Input.GetKeyDown(KeyCode.J)) { //heavy attack
            heavyAttack();
        }
        
    }

    void doubleAttack(){

       Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(DoubleAttackHB.position, doubleAttackRange, enemyLayers);

       foreach(Collider2D enemy in hitEnemies)
       {
           enemy.GetComponent<Enemy>().takeDamage(7);
           enemy.GetComponent<Enemy>().takeDamage(7);
       }
    }

    void heavyAttack(){

       Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(HeavyAttackHB.position, heavyAttackRange, enemyLayers);

       foreach(Collider2D enemy in hitEnemies)
       {
           enemy.GetComponent<Enemy>().takeDamage(23);
       }
    }

    private void OnDrawGizmos() {

        if(DoubleAttackHB == null)
            return;

        Gizmos.DrawWireSphere(DoubleAttackHB.position, doubleAttackRange);
        Gizmos.DrawWireSphere(HeavyAttackHB.position, heavyAttackRange);


    }
}
