using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MEC;

public class DiceBullet : MonoBehaviour
{
    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    [Range(2f, 12f)]
    float bulletSpeed = 6f;

    [SerializeField]
    Collider2D shapeCollider;

    [SerializeField]
    TextMeshProUGUI textMesh;

    float trailSpeed = 16f;

    public enum State { Enemy, Queued, Friendly }
    private State currentState = State.Enemy;
    public State CurrentState { get => currentState; }

    [SerializeField]
    DiceType diceType = DiceType.None;
    public DiceType Type { get => diceType; }

    private int value = 1;
    public int Value { get => value; }

    public Transform QueueTarget { get; private set; }

    public static HashSet<DiceBullet> Instances = new HashSet<DiceBullet>();

    [SerializeField]
    Transform queueAnchorPoint;
    public Transform QueueAnchorPoint { get => queueAnchorPoint; }

    private void Awake()
    {
        Instances.Add(this);
        Timing.RunCoroutine(_CheckDistance().CancelWith(gameObject));
    }

    private void OnDestroy()
    {
        Instances.Remove(this);
    }

    private void FixedUpdate()
    {
        if (currentState == State.Enemy)
        {
            rb.MovePosition(rb.position - (Vector2)transform.right * bulletSpeed * Time.deltaTime);
        }
        else
        if (currentState == State.Queued)
        {
            transform.position = Vector2.Lerp(transform.position, QueueTarget.position, trailSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)  // "Player" layer
        {
            Debug.Log("Player hit by: " + gameObject.name);
            collision.gameObject.GetComponentInParent<Player>().PopLastElement();
            Destroy(gameObject);
        }
    }

    IEnumerator<float> _CheckDistance()
    {
        while (currentState == State.Enemy)
        {
            yield return Timing.WaitForSeconds(1f);

            if (Mathf.Abs(transform.position.x) > 20f)
                Destroy(gameObject);
        }
    }

    public void SetQueueState(Transform target)
    {
        shapeCollider.enabled = false;
        QueueTarget = target;
        currentState = State.Queued;
    }

    public static void RollAllDices()
    {
        foreach (var obj in Instances)
        {
            if (obj.currentState == State.Enemy)
                obj.RollDice();
        }
    }

    void RollDice()
    {
        int rangeMax = DiceData.GetSidesOf(Type);

        value = Random.Range(1, rangeMax + 1);
        textMesh.text = value.ToString();
    }
}
