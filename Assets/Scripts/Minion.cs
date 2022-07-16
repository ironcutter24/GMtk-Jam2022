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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)  // "Player" layer
        {
            Debug.Log("Player hit by: " + gameObject.name);
            collision.gameObject.GetComponentInParent<Player>().PopLastElement();
        }
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
        Instantiate(diceBulletPrefab, spawnPoint.position, Quaternion.identity);
    }
}
