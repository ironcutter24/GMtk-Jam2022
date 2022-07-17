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

    [SerializeField]
    int value = 1;
    public int Value { get { return value; } }

    public DiceData DiceData { get => new DiceData(type, value); }

    public void SetValue(int value)
    {
        this.value = value;
    }
}
