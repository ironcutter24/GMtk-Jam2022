using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using System.Linq;

public class Player : MonoBehaviour
{
    CustomInput.InputManager inputManager;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    [Range(2f, 12f)]
    float speed = 6f;

    [SerializeField] GameObject diceCollectionPrefab;

    [SerializeField]
    Transform trailTarget;


    [SerializeField]
    Transform bulletSpawnPoint;


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


    private void Start()
    {
        inputManager = CustomInput.InputManager.Instance;
    }

    bool shieldState = false;

    private void Update()
    {
        if (inputManager.Shield && !shieldState)
            Timing.RunCoroutine(_Shield().CancelWith(gameObject));

        if (inputManager.Shoot)
            Shoot();
    }

    IEnumerator<float> _Shield()
    {
        shieldState = true;

        foreach (var parry in parryDatas)
        {
            parry.gfx.transform.localScale = Vector3.one * parry.radius;
            parry.gfx.SetActive(true);
        }

        Utility.Timer shieldTimer = new Utility.Timer();
        shieldTimer.Set(parryDuration);
        while (!shieldTimer.IsExpired)
        {
            HashSet<Collider2D> hits = new HashSet<Collider2D>();

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
            }
            yield return Timing.WaitForOneFrame;
        }

        foreach (var parry in parryDatas)
            parry.gfx.SetActive(false);

        yield return Timing.WaitForSeconds(parryCooldown);

        shieldState = false;
    }

    void Shoot()
    {
        if (queuedDices.Count <= 0) return;

        var bullet = Instantiate(diceCollectionPrefab, bulletSpawnPoint.transform.position, Quaternion.identity).GetComponent<DiceCollection>();
        for (int i = 0; i < queuedDices.Count; i++)
        {
            var dice = queuedDices[i].GetComponent<DiceBullet>();
            bullet.AddDice(new DiceData(dice.Type, dice.Value));
            Destroy(queuedDices[i]);
        }
        queuedDices.Clear();
    }

    List<GameObject> queuedDices = new List<GameObject>();
    void Enqueue(GameObject go)
    {
        //var val = go.GetComponent<DiceBullet>().Value;
        //diceCollection.AddDice(new DiceData(6, val, go));
        //Destroy(go);

        var newDice = go.GetComponentInParent<DiceBullet>();

        if (queuedDices.Count == 0)
        {
            newDice.SetQueueState(trailTarget);
        }
        else
        {
            newDice.SetQueueState(queuedDices[queuedDices.Count - 1].GetComponent<DiceBullet>().QueueAnchorPoint);
        }

        queuedDices.Add(newDice.gameObject);
    }

    public void PopLastElement()
    {
        if (queuedDices.Count <= 0)
        {
            return;
        }
        else
        {
            var dice = queuedDices[0];
            if (queuedDices.Count == 1)
            {
                Destroy(dice.gameObject);
            }
            else
            {
                queuedDices[1].GetComponent<DiceBullet>().SetQueueState(trailTarget);
                Destroy(dice.gameObject);
            }
            queuedDices.RemoveAt(0);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + inputManager.Directional * speed * Time.deltaTime);
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
