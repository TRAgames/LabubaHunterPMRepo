using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileOptimization : MonoBehaviour
{
    [SerializeField] private bool _needMobileOptimization = false;

    public bool NeedMobileOptimization { get => _needMobileOptimization; set => _needMobileOptimization = value; }
}
