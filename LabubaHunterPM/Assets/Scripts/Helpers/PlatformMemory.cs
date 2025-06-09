using UnityEngine;

public class PlatformMemory : MonoBehaviour
{
    private static PlatformMemory _instance;
    private bool _isPlatformKnown = false;

    public static PlatformMemory Instance { get => _instance; set => _instance = value; }
    public bool IsPlatformKnown { get => _isPlatformKnown; set => _isPlatformKnown = value; }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
    }


}
