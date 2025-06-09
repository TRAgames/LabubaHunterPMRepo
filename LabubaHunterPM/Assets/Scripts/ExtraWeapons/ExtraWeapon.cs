using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Extra Weapon")]
public class ExtraWeapon : ScriptableObject
{
    [SerializeField] private string _key;
    public string Name { get => _key; }
    [SerializeField] GameObject _prefabPC;
    [SerializeField] GameObject _prefabMobile;
    public GameObject Prefab 
    {
        get { return PlayerPrefs.GetString("PLATFORM") == "PC" ? _prefabPC : _prefabMobile;  }
    }
}
