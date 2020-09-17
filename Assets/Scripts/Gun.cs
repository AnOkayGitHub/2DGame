using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Guns/New Gun")]
public class Gun : ScriptableObject
{
    [SerializeField] private string weaponName;
    [SerializeField] private bool isOneHanded;
    [SerializeField] private bool isSingleReload;
    [SerializeField] private float fireRate;
    [SerializeField] private float spreadAmount;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private FireType fireType;
    [SerializeField] private int timesToFire;
    [SerializeField] private float reloadFinishSpeed;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxReserveAmmo;
    [SerializeField] private Sprite weaponPartOne;      // Frame
    [SerializeField] private Sprite weaponPartTwo;      // Moving / Sliding Part
    [SerializeField] private Sprite weaponPartThree;    // Ammo
    [SerializeField] private AnimatorOverrideController currentController;

    public enum FireType
    {
        Single, Burst, Spread
            
    }

    public string GetWeaponName()
    {
        return weaponName;
    }

    public bool GetOneHanded()
    {
        return isOneHanded;
    }

    public bool GetSingleReload()
    {
        return isSingleReload;
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public int GetTimesToFire()
    {
        return timesToFire;
    }

    public float GetSpreadAmount()
    {
        return spreadAmount;
    }

    public float GetReloadSpeed()
    {
        return reloadSpeed;
    }

    public float GetReloadFinishSpeed()
    {
        return reloadFinishSpeed;
    }

    public int GetMaxReserveAmmo()
    {
        return maxReserveAmmo;
    }

    public FireType GetFireType()
    {
        return fireType;
    }

    public int GetMaxAmmo()
    {
        return maxAmmo;
    }

    public Sprite[] GetCurrentSprites()
    {
        return new Sprite[] { weaponPartOne, weaponPartTwo, weaponPartThree };
    }

    public AnimatorOverrideController GetCurrentController()
    {
        return currentController;
    }


}
