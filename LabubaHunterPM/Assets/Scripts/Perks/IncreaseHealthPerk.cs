using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Perk Type/Increase Health Perk")]
public class IncreaseHealthPerk : Perk
{
    public override string Type { get => "Increase_Health_Perk"; }
}
