using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Special Weapon", menuName = "ScriptableObjects/SpecialWeapon", order = 2)]
public class SpecialWeapons : ScriptableObject
{
    public bool RapidFire;
    public Color OverArmor, UnderArmor;
    public GameObject Projectile, HalfChargedProjectile, FullyChargedProjectile;
    public int MaxEnergy;
}
