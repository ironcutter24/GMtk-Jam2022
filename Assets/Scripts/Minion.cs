using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Minion : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject diceBulletPrefab;

    [SerializeField]
    private int minionType = 6;
    public int Type { get => minionType; }

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

        if (collision.gameObject.layer == 9)  // "FriendlyBullet" layer
        {
            var diceColl = collision.gameObject.GetComponentInParent<DiceCollection>();
            if (diceColl.PopDices(new List<DiceData> { new DiceData(Type, 0) }))
            {
                Destroy(gameObject);
            }
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
