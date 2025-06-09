using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeFar : MonoBehaviour
{

    public void SetFar(int value)
    {
        Camera.main.farClipPlane = value;

    }
}
