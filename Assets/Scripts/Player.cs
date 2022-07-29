using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Player : MonoBehaviour
{
    CustomInput.InputManager inputManager;

    bool shieldState = false;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    [Range(2f, 12f)]
    float speed = 6f;

    [SerializeField]
    SpriteRenderer gfx, aura;

    [SerializeField] GameObject diceCollectionPrefab;

    [SerializeField]
    Transform bulletSpawnPoint;

    [SerializeField]
    Transform trailTarget;

    [SerializeField]
    [Range(1, 12)]
    int maxTrailLenght = 3;


    [Header("Parry")]

    [SerializeField]
    [Range(.1f, 1f)]
    float parryDuration = .2f;

    [SerializeField]
    [Range(.1f, 1f)]
    float parryCooldown = .4f;

    [SerializeField]
    LayerMask parryMask;

    [SerializeField]
    List<ParryData> parryDatas = new List<ParryData>();


    [Header("Materials")]

    [SerializeField]
    Material parryUseMat;

    [SerializeField]
    Material parryRechargeMat;


    private void Start()
    {
        inputManager = CustomInput.InputManager.Instance;

        parryUseMat.SetFloat("_FadeAmount", 1f);
    }

    int horizontalTweenId;
    int verticalTweenId;
    Vector2 leanSpace = new Vector2(.12f, .1f);
    private void Update()
    {
        const float leanTime = .2f;

        if (inputManager.Horizontal.HasChanged)
        {
            LeanTween.cancel(horizontalTweenId);
            horizontalTweenId = LeanTween.value(gfx.gameObject, transform.localScale.y, inputManager.Vertical.GetValue(), leanTime).setOnUpdate((float val) =>
            {
                transform.localScale = new Vector3(1f - (Mathf.Abs(val) * leanSpace.x), 1f, 1f);
            }).id;
        }

        if (inputManager.Vertical.HasChanged)
        {
            LeanTween.cancel(verticalTweenId);
            verticalTweenId = LeanTween.value(gfx.gameObject, transform.localScale.y, inputManager.Vertical.GetValue(), leanTime).setOnUpdate((float val) =>
            {
                transform.localScale = new Vector3(1f, 1f - (Mathf.Abs(val) * leanSpace.y), 1f);
            }).id;
        }


        if (inputManager.Shield && !shieldState)
            Timing.RunCoroutine(_Shield().CancelWith(gameObject));

        if (inputManager.Shoot)
            Shoot();
    }

    bool hasController = true;
    private void FixedUpdate()
    {
        if (hasController)
        {
            rb.MovePosition(rb.position + inputManager.Directional * speed * Time.deltaTime);
        }
    }

    #region Abilities

    IEnumerator<float> _Shield()
    {
        const float parryReloadDuration = .1f;

        shieldState = true;

        AudioManager.Instance.PlayFX_parryUse();

        // Shield gfx tweening
        LeanTween.value(gameObject, 1f, -.1f, parryDuration * .5f).setOnUpdate((float val) =>
        {
            parryUseMat.SetFloat("_FadeAmount", val);
        });

        LeanTween.value(gameObject, -.1f, 1f, parryReloadDuration).setOnUpdate((float val) =>
        {
            parryRechargeMat.SetFloat("_FadeAmount", val);
        });

        HashSet<Collider2D> hits = new HashSet<Collider2D>();

        Utility.Timer shieldTimer = new Utility.Timer();
        shieldTimer.Set(parryDuration);
        while (!shieldTimer.IsExpired)
        {
            foreach (var parry in parryDatas)
            {
                var hit = Physics2D.OverlapCircleAll(parry.transform.position, parry.radius, parryMask);

                foreach (var obj in hit)
                    hits.Add(obj);
            }

            if (hits.Count > 0)
            {
                foreach (var obj in hits)
                {
                    if (obj != null)
                        Enqueue(obj.gameObject);
                }
                hits.Clear();
            }
            yield return Timing.WaitForOneFrame;
        }

        foreach (var parry in parryDatas)
            parry.gfx.SetActive(false);

        // Shield gfx tweening
        LeanTween.value(gameObject, -.1f, 1f, parryDuration * .5f).setOnUpdate((float val) =>
        {
            parryUseMat.SetFloat("_FadeAmount", val);
        });

        yield return Timing.WaitForSeconds(parryCooldown - parryReloadDuration);

        LeanTween.value(gameObject, 1f, -.1f, parryReloadDuration).setOnUpdate((float val) =>
        {
            parryRechargeMat.SetFloat("_FadeAmount", val);
        });

        yield return Timing.WaitForSeconds(parryReloadDuration);

        AudioManager.Instance.PlayFX_parryReady();

        shieldState = false;
    }

    void Shoot()
    {
        if (queuedDices.Count <= 0) return;

        AudioManager.Instance.PlayFX_shoot();

        var bullet = Instantiate(diceCollectionPrefab, bulletSpawnPoint.transform.position, Quaternion.identity).GetComponent<DiceCollection>();
        for (int i = 0; i < queuedDices.Count; i++)
        {
            var dice = queuedDices[i];
            bullet.AddDice(new DiceData(dice.Type, dice.Value));
            Destroy(queuedDices[i].gameObject);
        }
        queuedDices.Clear();
    }

    #endregion

    #region Trail

    List<DiceBullet> queuedDices = new List<DiceBullet>();
    void Enqueue(GameObject go)
    {
        //var val = go.GetComponent<DiceBullet>().Value;
        //diceCollection.AddDice(new DiceData(6, val, go));
        //Destroy(go);

        var newDice = go.GetComponentInParent<DiceBullet>();

        if (queuedDices.Count == maxTrailLenght)
        {
            PopLastElement();
        }

        if (queuedDices.Count == 0)
        {
            newDice.SetQueueState(trailTarget);
        }
        else
        {
            newDice.SetQueueState(queuedDices[queuedDices.Count - 1].QueueAnchorPoint);
        }

        queuedDices.Add(newDice);
    }

    public void Hit()
    {
        AudioManager.Instance.PlayFX_hitPlayer();
        PopLastElement();
    }

    void PopLastElement()
    {
        if (queuedDices.Count <= 0)
        {
            StartCoroutine(_Death());
            return;
        }
        else
        {
            var dice = queuedDices[0];
            if (queuedDices.Count > 1)
            {
                queuedDices[1].SetQueueState(trailTarget);
            }
            queuedDices.RemoveAt(0);
            Destroy(dice.gameObject);
        }
    }

    #endregion

    IEnumerator _Death()
    {
        hasController = false;

        AudioManager.Instance.PlayFX_koPlayer();
        GameManager.Instance.SetTimeScale(.1f);

        LeanTween.value(gfx.gameObject, 1f, 0f, .4f).setOnUpdate((float val) =>
        {
            gfx.material.SetFloat("_Alpha", val);

            parryRechargeMat.SetFloat("_Alpha", val);

            var colorApp = aura.color;
            colorApp.a = val;
            aura.color = colorApp;
        }).setIgnoreTimeScale(true);

        yield return new WaitForSecondsRealtime(1.2f);

        GameManager.Instance.ReloadCurrentScene();
        yield break;
    }

    private void OnDrawGizmos()
    {
        foreach (var parry in parryDatas)
            Gizmos.DrawWireSphere(parry.transform.position, parry.radius);
    }
}

[System.Serializable]
public class Parry
{
    public Transform pivot;
    public GameObject gfx;
    public LayerMask mask;

    [Range(.2f, 3f)]
    public float radius = 1f;
}
