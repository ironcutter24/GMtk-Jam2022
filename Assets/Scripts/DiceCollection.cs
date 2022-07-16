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

    public bool PopDices(List<DiceData> targetDices)
    {
        Debug.Log("Dices data requested!");

        foreach (var targetDice in targetDices)
        {
            foreach (var sampleDice in diceDatas)
            {
                if(sampleDice.type == targetDice.type)
                    return true;
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

[System.Serializable]
public class DiceData
{
    public int type;
    public int value;

    public DiceData(int type, int value)
    {
        this.type = type;
        this.value = value;
    }
}
