using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{

   private Transform playerTransform;

   public float offsetX;

   private void Start() {
      playerTransform = GameObject.FindGameObjectWithTag("Player").transform; 
   }
    

    // Update is called once per frame
    void Update()
    {

        Vector3 temp = transform.position;

        temp.x = playerTransform.position.x;

        temp.x += offsetX;

        temp.y = playerTransform.position.y + 2;


        transform.position = temp;
       
        
        
    }
}
