using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateScale : MonoBehaviour
{
    public float Calculate(float distanceToPlayer)
    {
        if (distanceToPlayer <= 0) return 0.01f;
        return (distanceToPlayer / 10) * (Mathf.Log10(distanceToPlayer) + 1);
    }
}
