using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BossDice : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMesh;

    [SerializeField]
    DiceType type = DiceType.None;
    public DiceType Type { get { return type; } }

    int value = 1;
    public int Value { get { return value; } }

    public DiceData DiceData { get => new DiceData(type, value); }

    private void Start()
    {
        value = Random.Range(1, DiceData.GetSidesOf(Type) + 1);
        textMesh.text = value.ToString();
    }
}
