using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Type/Boss")]
public class BossType : HoldingType
{
    [SerializeField] private bool _disableInstakill;
    public override string Type { get => "Boss"; }
    public bool DisableInstakill { get => _disableInstakill; set => _disableInstakill = value; }
}
