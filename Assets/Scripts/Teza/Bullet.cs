using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector2 velocity = new Vector2(0f, 0f);
    public LayerMask enemyLayer;
    void Update()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = currentPosition + velocity * Time.deltaTime;

        RaycastHit2D hit = Physics2D.Raycast(currentPosition, velocity, velocity.magnitude * Time.deltaTime);

        if (hit.collider != null)
        {
            IEnemy enemyHealth = hit.collider.GetComponent<IEnemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(1);
                Destroy(gameObject);
            }
        }

        transform.position = newPosition;
    }

}
