using UnityEngine;

public class lesmaenemyshoot : MonoBehaviour
{
    [SerializeField] Transform[] right_ShootPoints;
    [SerializeField] Transform[] left_ShootPoints;
    [SerializeField] PlatformEnemyDirectionSetter _direction;

    [SerializeField] string projectile_poolTag = "slime_projectile";

    public void Shoot()
    {
        if (_direction.XDirection == 1f)
        {
            foreach (Transform point in right_ShootPoints)
            {
                ObjectPoolManager.Instance.SpawnFromPool(projectile_poolTag, point.position, point.rotation);
            }
        }
        else
        {
             foreach (Transform point in left_ShootPoints)
            {
                ObjectPoolManager.Instance.SpawnFromPool(projectile_poolTag, point.position, point.rotation);
            }
        }
    }
}
