using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Minion : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject diceBulletPrefab;

    private void Start()
    {
        Timing.RunCoroutine(_SpawnBullet().CancelWith(gameObject));
    }

    IEnumerator<float> _SpawnBullet()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return Timing.WaitForSeconds(2f);
            LaunchDice();
        }
    }

    void LaunchDice()
    {
        Instantiate(diceBulletPrefab, spawnPoint);
    }
}
