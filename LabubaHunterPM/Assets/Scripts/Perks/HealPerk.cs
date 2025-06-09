using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk Type/Heal Perk")]
public class HealPerk : Perk
{
    public override string Type { get => "Heal_Perk"; }
}
