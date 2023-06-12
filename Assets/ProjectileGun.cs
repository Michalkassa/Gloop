using TMPro;
using UnityEngine;
using System.Collections;

public class ProjectileGun : MonoBehaviour
{
    [Header("Gun Stats")]
    public float fireRate;
    public float verticalSpread;
    public float horizontalSpread;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    public float bulletDamage;
    public float headMultiplier;
    public float legsMultiplier;
    public float bodyMultiplier;

    int bulletsLeft, bulletsShot;
    float timeBetweenShooting;

    bool shooting, readyToShoot, reloading;

    public Transform attackPoint;
    private TakeDamage TakeDmg;
    public GameObject ImpactParticleSystem;
    public TrailRenderer BulletTrail;
    public Transform BulletSpawnPoint;

    //Graphics
    public TextMeshProUGUI ammoDisplay;
    public GameObject muzzleFlash;


    //bug fixing
    public bool allowInvoke = true;

    void Awake(){
       bulletsLeft = magazineSize;
       readyToShoot = true;
    }

    void Update(){
        timeBetweenShooting = 1/(fireRate/60);
        MyInput();


        if (ammoDisplay != null){
            ammoDisplay.SetText(bulletsLeft/bulletsPerTap + " / " + magazineSize / bulletsPerTap);
        }
    }

    void MyInput(){
        //check if button held down
        if(allowButtonHold) shooting = Input.GetButton("Fire1"); 
        else shooting = Input.GetButtonDown("Fire1");
        //reloading
        if(Input.GetKeyDown(KeyCode.R) && !reloading && bulletsLeft < magazineSize) Reload();

        //reload automaticaly when without ammo;
        if(readyToShoot && !reloading && bulletsLeft <=0) Reload();


        //shooting
        if(readyToShoot && shooting && !reloading && bulletsLeft>0){
            bulletsShot = 0;
            Shoot();
        }
    }

    private void Shoot(){
        TakeDmg = null;
        readyToShoot = false;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;

        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit)){
            CheckHit(hit);
            TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail,hit));
        }
        //else{
        //     TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
        //     StartCoroutine(SpawnTrail(trail,hit));
        // }


        if(muzzleFlash != null){
            GameObject muzzle = Instantiate(muzzleFlash,attackPoint.position,Quaternion.identity);
            Destroy(muzzle, 2f);
        }

        bulletsLeft--;
        bulletsShot++;

        //Play Animation
        ShootAnimation();

        //invoke reset shot function
        if(allowInvoke){
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }


        if(bulletsShot < bulletsPerTap && bulletsLeft > 0){
            Invoke("Shoot", timeBetweenShots);
        }

    }

    void CheckHit(RaycastHit hit){
        if(hit.rigidbody != null){
            hit.rigidbody.AddForce(-hit.normal * 10f);
        }
        try{
            TakeDmg = hit.transform.GetComponent<TakeDamage>();
            switch (TakeDmg.damageType){
                case TakeDamage.collisionType.head : TakeDmg.HIT(bulletDamage * headMultiplier);
                break;
                case TakeDamage.collisionType.body :  TakeDmg.HIT(bulletDamage * bodyMultiplier);
                break;
                case TakeDamage.collisionType.legs : TakeDmg.HIT(bulletDamage * legsMultiplier);
                break;
            }
        }catch{
            Debug.Log("something went wrong");
        }
    }

    void ShootAnimation(){
        GetComponent<Animator>().SetTrigger("Shoot");
    }

    void ResetShot(){
        readyToShoot = true;
        allowInvoke = true;
    }

    void Reload(){
        reloading = true;
        GetComponent<Animator>().SetBool("Reloading",reloading);
        Invoke("ReloadingFinished", reloadTime);
    }

    void ReloadingFinished(){
        bulletsLeft = magazineSize;
        reloading = false;
        GetComponent<Animator>().SetBool("Reloading",reloading);
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit){
        float time = 0;
        Vector3 startPosition = Trail.transform.position;
        while(time < 1){
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        Trail.transform.position= Hit.point;
        Instantiate(ImpactParticleSystem,Hit.point,Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }



}
