using UnityEngine;

public class MathHelper : MonoBehaviour
{
    public bool CheckDiv(int number, int waveNumber)
    {
        return waveNumber % number == 0 ? true : false;
    }
}
