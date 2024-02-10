using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource deathSFX;
    [SerializeField] private AudioSource enemyDeathSFX;
    [SerializeField] private AudioSource shootingSFX;
    [SerializeField] private AudioSource gunEmptySFX;
    [SerializeField] private AudioSource shieldSFX;
    [SerializeField] private AudioSource attackSFX;
    [SerializeField] private AudioSource dashSFX;
     [SerializeField] private AudioSource music;


    public void AttackSFX()
    {
        attackSFX.pitch = Random.Range(0.8f, 1.5f);
        attackSFX.Play();
    }
    
    public void ShieldSFX()
    {
        shieldSFX.Play();
    }
    public void ShootingSFX()
    {
        shootingSFX.Play();
    }
    public void GunEmptySFX()
    {
        gunEmptySFX.Play();
    }
    public void EnemyDeathSFX()
    {
        enemyDeathSFX.pitch = Random.Range(0.5f, 1.5f);
        enemyDeathSFX.Play();
    }
    public void DeathSFX()
    {
        deathSFX.Play();
    }
    public void DashSFX()
    {
        dashSFX.pitch = Random.Range(1f, 1.5f);
        dashSFX.Play();
    }
    public void MusicQuietSFX()
    {
        music.volume = 0.25f;
    }
    public void MusicNormalSFX()
    {
        music.volume = 0.8f;
    }

}
