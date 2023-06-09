using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : Bullet
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("EnemyRock") ||
            other.CompareTag("EnemyPaper") || other.CompareTag("EnemyScissors"))
        {
            Destroy(gameObject);
            Instantiate(hitVFX, new Vector3(transform.position.x, transform.position.y, transform.position.z+1), transform.rotation);
        }
    }

    private void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        if (transform.position.z > startingBulletPosition.z + maxRange ||
            transform.position.z < startingBulletPosition.z - maxRange)
        {
            Destroy(gameObject);
        }
    }
}
