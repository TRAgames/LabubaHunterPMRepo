using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Zombie Type/Zombie Simple")]
public class ZombieSimple : Zombie
{
    [SerializeField] private Material _material;

    public override string Type { get => "Zombie Simple"; }
    public Material Material { get => _material; set => _material = value; }
}
