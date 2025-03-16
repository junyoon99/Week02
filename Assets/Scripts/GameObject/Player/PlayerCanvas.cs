using UnityEngine;

public class PlayerCanvas : MonoBehaviour
{
    public Transform skillBar;

    private void Awake()
    {
        skillBar = transform.Find("Canvas/SkillBar");
    }
}
