using TMPro;
using UnityEngine;
using System.Collections;

public class ProjectileGun : MonoBehaviour
{
    //bullet
    public GameObject bullet;

    //bullet force
    public float shootForce, upwardforce;

    [Header("Gun Stats")]
    public float fireRate;
    public float verticalSpread;
    public float horizontalSpread;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;

    int bulletsLeft, bulletsShot;
    float timeBetweenShooting;

    bool shooting, readyToShoot, reloading;

    public Transform attackPoint;

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
        readyToShoot = false;

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;

        Vector3 targetPoint;

        if(Physics.Raycast(ray, out hit)) targetPoint = hit.point;
        else targetPoint = ray.GetPoint(75);
        

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        //calculate Spread;

        float x = Random.Range(horizontalSpread, -horizontalSpread);
        float y = Random.Range(verticalSpread, -verticalSpread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x,y,0);
        
        //INSTATIATE BULLET
        GameObject currentBullet = Instantiate(bullet, attackPoint.position,Quaternion.identity);
        currentBullet.transform.forward = directionWithSpread.normalized;

        //add Forces
        currentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);

        if(muzzleFlash != null){
            GameObject muzzle = Instantiate(muzzleFlash,attackPoint.position,Quaternion.identity);
            muzzle.transform.forward = directionWithSpread.normalized;
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



}
