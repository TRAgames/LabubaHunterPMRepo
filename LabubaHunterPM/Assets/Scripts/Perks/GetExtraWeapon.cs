using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk Type/Get Extra Weapon")]
public class GetExtraWeapon : Perk
{
    [SerializeField] private int _time;

    public ExtraWeapon ExtraWeapon;
    public override string Type { get => "Get_Extra_Weapon_Perk"; }
    public override string Name { get => ExtraWeapon.Name; }

    public float Time { get => _time; }
}
