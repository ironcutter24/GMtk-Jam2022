using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Minion : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject diceBulletPrefab;

    [SerializeField]
    private DiceType minionType = DiceType.None;
    public DiceType Type { get => minionType; }

    [SerializeField]
    private float shootCooldown = 2f;

    [SerializeField]
    SpriteRenderer gfx, aura;

    [SerializeField]
    Collider2D shapeCollider;

    CoroutineHandle shootCoroutine;
    private void Start()
    {
        shootCoroutine = Timing.RunCoroutine(_SpawnBullet().CancelWith(gameObject));
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
            if (diceColl.PopDices(new DiceData(Type, 0)))
            {
                TurnOff();
            }
        }
    }

    IEnumerator<float> _SpawnBullet()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return Timing.WaitForSeconds(shootCooldown);
            LaunchDice();
        }
    }

    void LaunchDice()
    {
        Instantiate(diceBulletPrefab, spawnPoint.position, Quaternion.identity);
    }

    void TurnOff()
    {
        Timing.KillCoroutines(shootCoroutine);
        shapeCollider.enabled = false;

        LeanTween.value(gameObject, 1f, .2f, .2f).setOnUpdate((float val) => {
            Debug.Log("tweened val:" + val);
            gfx.material.SetFloat("_Alpha", val);
        });

        LeanTween.value(gameObject, -.1f, 1f, .3f).setOnUpdate((float val) => {
            Debug.Log("tweened val:" + val);
            aura.material.SetFloat("_FadeAmount", val);
        });

        Timing.RunCoroutine(_ReviveCooldown().CancelWith(gameObject));
    }

    void TurnOn()
    {
        LeanTween.value(gameObject, .2f, 1f, .2f).setOnUpdate((float val) => {
            Debug.Log("tweened val:" + val);
            gfx.material.SetFloat("_Alpha", val);
        });

        LeanTween.value(gameObject, 1f, -.1f, .4f).setOnUpdate((float val) => {
            Debug.Log("tweened val:" + val);
            aura.material.SetFloat("_FadeAmount", val);
        }).setOnComplete(() => shapeCollider.enabled = true);

        shootCoroutine = Timing.RunCoroutine(_SpawnBullet().CancelWith(gameObject));
    }

    IEnumerator<float> _ReviveCooldown()
    {
        yield return Timing.WaitForSeconds(10f);
        TurnOn();
    }
}
