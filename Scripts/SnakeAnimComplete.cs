using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAnimComplete : MonoBehaviour
{
    GameObject playerObj;
    GameObject enemyObj;
    private void Start() {
        playerObj = GameObject.FindGameObjectWithTag("Player");
        enemyObj = GameObject.FindGameObjectWithTag("Enemy");

      //  Debug.Log("Get Name: "+playerObj.name + " " + enemyObj.name);
    }
   public void SnakeEatAnimationCompleted()
   {
        Debug.Log("Enable");
        Invoke("DelayInEnable",0.4f);
      
  }

  public void DelayInEnable()
  {
      if(playerObj.transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            playerObj.transform.GetChild(0).gameObject.SetActive(true);
           // Debug.Log("Enable Player");

        }
        else if(enemyObj.transform.GetChild(0).gameObject.activeInHierarchy == false)
        {
            enemyObj.transform.GetChild(0).gameObject.SetActive(true);       
           // Debug.Log("Enable Enemy");

        }

       if(PropsMovement.instance != null)
       {
        PropsMovement.instance.CheckPlayersOnSamePoint();
       } 
  }
}

