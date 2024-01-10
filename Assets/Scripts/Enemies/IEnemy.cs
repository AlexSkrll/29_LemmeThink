using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    
    void TakeDamage(int damage);
   // void Disable();
}

// public void Die()
//    {
//        if(spawner != null ) spawner.EnemyDestroyed();
//        GameManager.instance.IncrementKillCount();
//        Destroy(gameObject);
//     }