using System.Collections;
using UnityEngine;

public class InfiniteAmmoTrigger : MonoBehaviour
{


    //------------------------------------------------UNTESTED------------------------------------------------UNTESTED------------------------------------------------UNTESTED------------------------------------------------UNTESTED------------------------------------------------//

                                                                //This script was made for if the Infinite Ammo ability is an item that the player picks up
    private float effectDuration = 20f;
    private bool effectActive = false;
    private Gun currentGun;

    private void OnCollisionEnter(Collision collision)                  //this method is if the infinite ammo powerup is an item the player picks up
    {
        currentGun = collision.gameObject.GetComponent<Gun>();          //assigning the current gun to a variable
        currentGun.infiniteAmmo = true;                                 //straight up just changing the value of the infiniteAmmo variable
        effectActive = true;                                            //assigning true to the boolean used to end the "CheckGun" method
        StartCoroutine(InfiniteAmmoTimer());                            //calling the timer to disable the effect
        CheckGun();                                                     //calling a method for if the gun the player is holding changed, so the effect can stay

    }

    IEnumerator InfiniteAmmoTimer()
    {
        yield return new WaitForSeconds(effectDuration);                //the time will count down, then it'll proceed
        effectActive = false;                                           //ends the loop of the "CheckGun" method
        currentGun.infiniteAmmo = false;                                //ends the effect
    }

    void CheckGun()
    {
        GameObject gunCheck;                                            //creates the comparative variable
        while (effectActive)                                            //effectActive is set to true before the method is started
        {
            gunCheck = FindObjectOfType<Gun>().gameObject;              //comparative variable takes the gameobject of the current active "Gun"
            if (currentGun.gameObject != gunCheck)                      //simple question for when the gun that was active when the power up was enabled, is different from the current gun in the scene
            {
                currentGun.infiniteAmmo = false;                        //disables the infiniteAmmo variable for that gun, so that if the effect ends, and the player changes gun, it won't stay with infinite ammo
                currentGun = gunCheck.GetComponent<Gun>();              //assigning the new gun to the variable
                currentGun.infiniteAmmo = true;                         //enabling the infinite ammo effect for the new gun
            }
        }
    }
}
