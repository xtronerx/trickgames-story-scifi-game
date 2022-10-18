using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    public int selectWeapon = 0;


    // Start is called before the first frame update
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {

        int previousSelectedWeapons = selectWeapon;

        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (selectWeapon >= transform.childCount - 1)
                selectWeapon = 0;
            else
                selectWeapon++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (selectWeapon <= 0)
                selectWeapon = transform.childCount - 1;
            else
                selectWeapon--;
        }

        if (previousSelectedWeapons !=selectWeapon)
        {
            SelectWeapon();
        }


    }

    void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }
}
