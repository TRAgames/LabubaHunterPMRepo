using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Auto Weapon")]
public class AutoWeapon : ScriptableObject
{
    [SerializeField] private string _key;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _damageBody;
    [SerializeField] private int _damageHead;
    public string Name { get => _key; }
    public GameObject Prefab { get => _prefab; set => _prefab = value; }
    public int DamageBody { get => _damageBody; set => _damageBody = value; }
    public int DamageHead { get => _damageHead; set => _damageHead = value; }
}
