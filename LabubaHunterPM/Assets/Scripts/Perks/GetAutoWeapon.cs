using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk Type/Get Auto Weapon")]
public class GetAutoWeapon : Perk
{
    [SerializeField] private int _timeBottom;
    [SerializeField] private int _timeTop;

    public AutoWeapon AutoWeapon;
    public override string Type { get => "Get_Auto_Weapon_Perk"; }
    public override string Name { get => AutoWeapon.Name; }
    public int TimeBottom { get => _timeBottom; set => _timeBottom = value; }
    public int TimeTop { get => _timeTop; set => _timeTop = value; }
}
