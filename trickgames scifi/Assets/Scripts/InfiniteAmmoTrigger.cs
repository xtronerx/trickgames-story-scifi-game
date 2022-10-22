using System.Collections;
using UnityEngine;

public class InfiniteAmmoTrigger : MonoBehaviour
{

    private float effectDuration = 20f;
    private bool effectActive = false;
    private GameObject currentGun;
    private GameObject weaponSwitcher;

    private void Start()
    {
        StartCoroutine(InfiniteAmmoTimer());
    }

    IEnumerator InfiniteAmmoTimer()
    {
        yield return new WaitUntil(() => Input.GetButtonDown("Skill1"));                                            //Wait for the player input of Skill1; default value given is "t", can be changed in the Project Settings -> Input Manager
        effectActive = true;                                                                                        //this is used for the coroutine that checks if the gun is changed

        currentGun = FindObjectOfType<WeaponSwitcher>().gameObject.GetComponentInChildren<Gun>().gameObject;
        currentGun.GetComponent<Gun>().InfiniteAmmoChanger();

        StartCoroutine(CheckGun());
        Debug.Log("Countdown started...");
        yield return new WaitForSeconds(effectDuration);                                                            //the time will count down, then it'll proceed
        Debug.Log("Countdown finished!");

        StopCoroutine(CheckGun());
        effectActive = false;                                                                                       //ends the loop of the "CheckGun" method
        currentGun.GetComponent<Gun>().InfiniteAmmoChanger();                                                       //ends the effect
    }

    IEnumerator CheckGun()
    {
        GameObject gunCheck;                                                                                             //creates the comparative variable
        while (effectActive)                                                                                             //effectActive is set to true before the method is started
        {
            yield return new WaitUntil(() => Input.GetAxis("Mouse ScrollWheel") != 0f);
            gunCheck = FindObjectOfType<WeaponSwitcher>().gameObject.GetComponentInChildren<Gun>().gameObject;           //comparative variable takes the gameobject of the current active "Gun"
            if (currentGun != gunCheck && effectActive)                                                                  //when the gun that was active when the power up was enabled is different from the current gun in the scene, and the effect is still active
            {
                currentGun.GetComponent<Gun>().InfiniteAmmoChanger();                                                    //disables the infiniteAmmo variable for that gun, so that if the effect ends, and the player changes gun, it won't stay with infinite ammo
                currentGun = gunCheck;                                                                                   //assigning the new gun to the variable
                currentGun.GetComponent<Gun>().InfiniteAmmoChanger();                                                    //enabling the infinite ammo effect for the new gun
            }
        }
    }
}
