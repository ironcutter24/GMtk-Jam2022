using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Boss : MonoBehaviour
{
    private List<BossDice> dices = new List<BossDice>();

    [SerializeField] GameObject FourSidesPrefabs, SixSidesPrefab, EightSidesPrefab;

    [SerializeField] List<Transform> presetTwo = new List<Transform>();
    [SerializeField] List<Transform> presetThree = new List<Transform>();
    [SerializeField] List<Transform> presetFour = new List<Transform>();

    [SerializeField] Material bossMat;
    [SerializeField] GameObject circleBG;

    [Header("Custom Attributes")]
    [SerializeField, Range(1, 4)]
    int numberOfDices = 1;

    const float hitDuration = .4f;
    float deathDuration { get => hitDuration * 2f; }

    private void Start()
    {
        InitDices();
        bossMat.SetFloat("_FadeAmount", 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)  // "Player" layer
        {
            Debug.Log("Player hit by: " + gameObject.name);
            collision.gameObject.GetComponentInParent<Player>().Hit();
        }

        if (collision.gameObject.layer == 9)  // "FriendlyBullet" layer
        {
            Debug.Log("Boss hit");

            var diceDatas = collision.gameObject.GetComponentInParent<DiceCollection>().GetDiceList();

            foreach (var dice in diceDatas)
            {
                for (int i = 0; i < dices.Count; i++)
                {
                    if (DiceData.AreEquals(dice, dices[i].DiceData))
                    {
                        AudioManager.Instance.PlayFX_hitBoss();
                        HitAnimation();

                        var obj = dices[i];
                        dices.RemoveAt(i);
                        Destroy(obj.gameObject);
                        break;
                    }
                }
            }

            if (dices.Count <= 0)
            {
                Timing.RunCoroutine(_Death().CancelWith(gameObject));
            }
        }
    }

    IEnumerator<float> _Death()
    {
        AudioManager.Instance.PlayFX_koBoss();

        LeanTween.value(gameObject, 0f, 1f, deathDuration).setOnUpdate((float val) =>
        {
            bossMat.SetFloat("_FadeAmount", val * .9f);
        });

        LeanTween.value(gameObject, 0f, 1f, deathDuration * .8f).setOnUpdate((float val) =>
        {
            circleBG.transform.localScale = Vector3.one * (1 - val);
        }).setEaseInCubic();

        GameManager.Instance.SetTimeScale(.5f);

        yield return Timing.WaitForSeconds(deathDuration);

        GameManager.Instance.LoadNextScene();
        yield break;
    }

    void HitAnimation()
    {
        LeanTween.value(gameObject, 0f, .1f, hitDuration).setOnUpdate((float val) =>
        {
            bossMat.SetFloat("_FadeAmount", val);
        }).setOnComplete(() =>
        LeanTween.value(gameObject, .1f, 0f, hitDuration).setOnUpdate((float val) =>
        {
            bossMat.SetFloat("_FadeAmount", val);
        })
        );
    }

    void InitDices()
    {
        switch (numberOfDices)
        {
            case 1: CreateDices(new List<Transform> { transform }); break;
            case 2: CreateDices(presetTwo); break;
            case 3: CreateDices(presetThree); break;
            case 4: CreateDices(presetFour); break;
            default: throw new System.Exception("Incorrect boss dice number");
        }
    }

    void CreateDices(List<Transform> targets)
    {
        foreach (var t in targets)
        {
            var obj = Instantiate(GetRandomDicePrefab(), t);
            var script = obj.GetComponent<BossDice>();

            dices.Add(script);
        }
    }

    GameObject GetRandomDicePrefab()
    {
        switch (Random.Range(0, 3))
        {
            case 0: return FourSidesPrefabs;
            case 1: return SixSidesPrefab;
            case 2: return EightSidesPrefab;
            default: throw new System.Exception("Random dice generation error");
        }
    }
}
