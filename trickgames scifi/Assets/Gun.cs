using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 15f;
    public float impactForce = 30f;

    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime;
    private bool isReloading = false;

    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public bool Automatic;

    private float nextTimeToFire;

    public Animator animator;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
            return;

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Automatic == true)
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }
        if(Automatic == false)
        {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }  
        
    }

    IEnumerator Reload ()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        animator.SetBool("Reloading", true);

        yield return new WaitForSeconds(reloadTime);

        animator.SetBool("Reloading", false);

        currentAmmo = maxAmmo;
        isReloading = false;
    }

    void Shoot ()
    {

        //particle when you shoot
        muzzleFlash.Play();

        currentAmmo--;

        //Raycast, using instead of bullet.
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);

            //Gets the Target and applies damage
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
        }

         GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        Destroy(impactGO, 0.5f);
    }
}
