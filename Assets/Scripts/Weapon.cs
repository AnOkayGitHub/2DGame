using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEditor.Animations;

public class Weapon : MonoBehaviour
{
    public float offset;
    public Player player;
    public GameObject projectile;
    public Transform projSpawn;
    public TextMeshProUGUI currentAmmoUI;
    public TextMeshProUGUI currentWeaponNameUI;
    public Animator weaponAnimator;
    public SpriteRenderer[] weaponSpriteRenderers;
    public GameObject secondHand;

    public string shootAnimation = "Shoot";
    public string reloadAnimation = "Reload";
    public string reloadStartAnimation = "Reload Start";
    public string reloadFinishAnimation = "Reload Finish";
    public string idleAnimation = "Idle";

    public bool isReloading = false;
    public bool isShooting = false;

    private Vector2 scale;
    private Vector2 negScale;
    

    public Gun[] guns;
    public int ID;

    [SerializeField] private string weaponName;
    [SerializeField] private bool isOneHanded;
    [SerializeField] private bool isSingleReload;
    [SerializeField] private float fireRate;
    [SerializeField] private int timesToFire;
    [SerializeField] private float spreadAmount;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private float reloadTransitionTime;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxReserveAmmo;
    [SerializeField] private int ammo;
    [SerializeField] private int reserveAmmo;
    [SerializeField] private Gun.FireType fireType;

    [SerializeField] private Sprite[] weaponSprites;
    [SerializeField] private AnimatorOverrideController currentController;

    private void Start()
    {
        scale = transform.localScale;
        negScale = new Vector2(scale.x, -scale.y);
    }

    public int GetAmmo()
    {
        return ammo;
    }

    public int GetReserveAmmo()
    {
        return reserveAmmo;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    // Update attributes of the current weapon with ID
    public void InitializeWeapon()
    {
        weaponName = guns[ID].GetWeaponName();
        fireRate = guns[ID].GetFireRate();
        fireType = guns[ID].GetFireType();
        spreadAmount = guns[ID].GetSpreadAmount();
        reloadSpeed = guns[ID].GetReloadSpeed();
        timesToFire = guns[ID].GetTimesToFire();
        reloadTransitionTime = guns[ID].GetReloadFinishSpeed();
        maxAmmo = guns[ID].GetMaxAmmo();
        maxReserveAmmo = guns[ID].GetMaxReserveAmmo();
        ammo = guns[ID].GetMaxAmmo();
        reserveAmmo = guns[ID].GetMaxReserveAmmo();
        weaponSprites = guns[ID].GetCurrentSprites();
        currentController = guns[ID].GetCurrentController();
        isOneHanded = guns[ID].GetOneHanded();
        isSingleReload = guns[ID].GetSingleReload();

        weaponAnimator.runtimeAnimatorController = currentController;

        for (int i = 0; i < weaponSprites.Length; i++)
        {
            weaponSpriteRenderers[i].sprite = weaponSprites[i];
        }

        secondHand.SetActive(!isOneHanded);

        UpdateUI();
    }

    // Update
    void Update()
    {
        Vector3 distanceToCursor = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float rotation = Mathf.Atan2(distanceToCursor.x, distanceToCursor.y) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, -rotation + offset);
        transform.localScale = ((player.lookDirection == 1) ? scale : negScale);

        UpdateUI();

    }

    // Shooting -> Start FireRate Coroutine
    public void Shoot()
    {
        isShooting = true;
        StartCoroutine(FireRate(0));
    }

    // Reload -> Start Reload Coroutines
    public void Reload()
    {
        // Single Reload -> One animation for reload
        // Multi Reload  -> Looping animation for reload

        if (isSingleReload)
        {
            isReloading = true;
            StartCoroutine("WaitForSingleReload");
        }
        else
        {
            isReloading = true;

            StartCoroutine("WaitForReloadStart");
        }
    }

    // Updates UI elements such as ammo and name
    public void UpdateUI()
    {
        currentAmmoUI.text = string.Format("<size=24>{0}</size><size=12>{1}</size>", ammo.ToString(), reserveAmmo.ToString());
        currentWeaponNameUI.text = weaponName.ToString();
    }

    // FireRate 
    public IEnumerator FireRate(int shot)
    {
        // FireTypes:
        // Single           -> Shoots one projectile per trigger pull
        // Burst            -> Shoots multiple projectiles one after another
        // Spread           -> Shoots multiple projectiles at once
        // Spread Amount    -> Randomly rotates the object on the z axis by a random number between -Spread Amount and Spread Amount
        
        // Determine how to fire
        switch (fireType)
        {
            case Gun.FireType.Single:
                weaponAnimator.Play(shootAnimation, -1, 0f);
                GameObject proj = (GameObject) Instantiate(projectile);
                proj.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + UnityEngine.Random.Range(-spreadAmount, spreadAmount));
                proj.transform.position = projSpawn.transform.position;
                ammo -= 1;
                shot++;
                yield return new WaitForSeconds(fireRate);
                break;
            case Gun.FireType.Burst:
                if (shot < timesToFire)
                {
                    weaponAnimator.Play(shootAnimation, -1, 0f);
                    GameObject burstProj = (GameObject) Instantiate(projectile);
                    burstProj.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + UnityEngine.Random.Range(-spreadAmount, spreadAmount));
                    burstProj.transform.position = projSpawn.transform.position;
                    ammo -= 1;
                    shot++;
                    Debug.Log("called");
                    yield return new WaitForSeconds(fireRate);
                    StartCoroutine(FireRate(shot));
                    break;
                }
                break;
            case Gun.FireType.Spread:
                ammo -= 1;

                for (int i = 0; i < timesToFire; i++)
                {
                    shot++;
                    weaponAnimator.Play(shootAnimation, -1, 0f);
                    GameObject spreadProj = (GameObject) Instantiate(projectile);
                    spreadProj.transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + UnityEngine.Random.Range(-spreadAmount, spreadAmount));
                    spreadProj.transform.position = projSpawn.transform.position;
                }
                yield return new WaitForSeconds(fireRate);
                break;
        }

        // Done firing all shots
        if (shot == timesToFire)
        {
            isShooting = false;
            player.canShoot = true;
            player.canReload = true;
            weaponAnimator.Play(idleAnimation, -1, 0f);
        }
        

        
    }

    // Handles looping reloads
    public IEnumerator WaitForMultiReload()
    {
        // Determine if we still need to reload
        if (ammo < maxAmmo && reserveAmmo > 0)
        {
            isReloading = true;
            weaponAnimator.Play(reloadAnimation, -1, 0f);
        }

        
        yield return new WaitForSeconds(reloadSpeed);

        // Add ammo
        reserveAmmo -= 1;
        ammo++;
        
        // Determine if we need to loop again, or finish
        if (ammo == maxAmmo || reserveAmmo == 0)
        {
            StartCoroutine("WaitForReloadFinish");
        }
        else
        {
            StartCoroutine("WaitForMultiReload");
        }
    }

    // Finish reloading and return to idle animation
    public IEnumerator WaitForReloadFinish()
    {
        weaponAnimator.Play(reloadFinishAnimation, -1, 0f);

        yield return new WaitForSeconds(reloadTransitionTime);

        player.canShoot = true;
        player.canReload = true;
        isReloading = false;
        weaponAnimator.Play(idleAnimation, -1, 0f);

    }

    // Start reloading and switch to reload animation
    public IEnumerator WaitForReloadStart()
    {
        weaponAnimator.Play(reloadStartAnimation, -1, 0f);

        yield return new WaitForSeconds(reloadTransitionTime);

        StartCoroutine("WaitForMultiReload");
    }

    // Reload with a single animation
    public IEnumerator WaitForSingleReload()
    {
        weaponAnimator.Play(reloadAnimation, -1, 0f);

        yield return new WaitForSeconds(reloadSpeed);

        // Determine how much ammo to add to the clip and subtract from the reserve ammo
        if (ammo == 0)
        {
            if (reserveAmmo >= maxAmmo)
            {
                ammo = maxAmmo;
                reserveAmmo -= ammo;
            }
            else
            {
                ammo = reserveAmmo;
                reserveAmmo = 0;
            }
        }
        else
        {
            int neededAmmo = maxAmmo - ammo;
            if (reserveAmmo >= neededAmmo)
            {
                ammo = maxAmmo;
                reserveAmmo -= neededAmmo;
            }
            else
            {
                ammo = reserveAmmo;
                reserveAmmo = 0;
            }
        }

        // Return to idle animation
        player.canShoot = true;
        player.canReload = true;
        isReloading = false;
        weaponAnimator.Play(idleAnimation, -1, 0f);
    }
}
