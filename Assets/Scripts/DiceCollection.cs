using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class DiceCollection : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed = 5f;

    List<DiceData> diceDatas = new List<DiceData>();

    private void Start()
    {
        Timing.RunCoroutine(_CheckDistance().CancelWith(gameObject));
    }

    public void AddDice(DiceData newDice)
    {
        diceDatas.Add(newDice);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.right * speed * Time.deltaTime);
    }

    public bool PopDices(DiceData targetDice)
    {
        Debug.Log("Dices data requested!");

        foreach (var sampleDice in diceDatas)
        {
            if (sampleDice.type == targetDice.type)
            {
                if (targetDice.value == 0) return true;
                if (sampleDice.value == targetDice.value) return true;
            }
        }
        return false;
    }

    IEnumerator<float> _CheckDistance()
    {
        while (gameObject.activeInHierarchy)
        {
            yield return Timing.WaitForSeconds(1f);

            if (Mathf.Abs(transform.position.x) > 20f)
                Destroy(gameObject);
        }
    }
}

public enum DiceType { None, Four, Six, Eight }

[System.Serializable]
public class DiceData
{
    public DiceType type;
    public int value;

    public DiceData(DiceType type, int value)
    {
        if (value < 0 || value > GetSidesOf(type)) throw new System.Exception("Dice value is out of range");

        this.type = type;
        this.value = value;
    }

    public static int GetSidesOf(DiceType dice)
    {
        switch (dice)
        {
            case DiceType.Four: return 4;
            case DiceType.Six: return 6;
            case DiceType.Eight: return 8;
            default: throw new System.Exception("Dice type not set");
        }
    }
}
