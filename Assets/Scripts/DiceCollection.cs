using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCollection : MonoBehaviour
{
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float speed = 5f;

    List<DiceData> diceDatas = new List<DiceData>();

    public void AddDice(DiceData newDice)
    {
        diceDatas.Add(newDice);
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 6)  // "Enemy" layer
        {
            var obj = collision.gameObject.GetComponent<Minion>();

            
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
