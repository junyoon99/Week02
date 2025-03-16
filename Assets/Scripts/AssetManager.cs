using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static GameObject blood;
    private void Awake()
    {
        blood = Resources.Load<GameObject>("Prefabs/Particles/Blood");
    }
}
