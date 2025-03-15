using UnityEngine;

public class HandGun : WeaponBase
{
    private void OnEnable()
    {
        bullet = Resources.Load<GameObject>("Prefabs/Bullets/HandGunBullet");
        fireRate = 1f;
    }

    void Update()
    {
        if (fireCoolTime > fireRate)
        {
            Fire(bullet);
        }
    }
}
