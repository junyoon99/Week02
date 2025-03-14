using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    protected float fireRate;
    public float fireCoolTime = 0;
    protected GameObject bullet;

    protected void Fire(GameObject bullet)
    {
        GameObject spawnedBullet = Instantiate(bullet, transform.position, transform.rotation);
        spawnedBullet.GetComponent<HandGunBullet>().fireFromVector = transform.position;
        spawnedBullet.GetComponent<HandGunBullet>().fireFromObject = transform.parent.parent.gameObject;
        fireCoolTime = 0;
    }
}
