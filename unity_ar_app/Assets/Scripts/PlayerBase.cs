using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public static PlayerBase Instance { get; private set; }

    [Header("Fixed World Position")]
    public Vector3 fixedPosition;

    void Awake()
    {
        Instance = this;

        // Force world position
        transform.position = fixedPosition;
    }
}