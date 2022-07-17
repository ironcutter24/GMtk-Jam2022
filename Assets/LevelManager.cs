using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    Material diceRollBarMat;

    [SerializeField]
    float diceRollCooldown = 1f;

    [SerializeField]
    GameObject prism;

    void Start()
    {
        Timing.RunCoroutine(_RollCountdown().CancelWith(gameObject));
    }

    IEnumerator<float> _RollCountdown()
    {
        while (gameObject.activeInHierarchy)
        {
            LeanTween.value(gameObject, 1f, -.1f, diceRollCooldown).setOnUpdate((float val) =>
            {
                diceRollBarMat.SetFloat("_FadeAmount", val);
            });

            yield return Timing.WaitForSeconds(diceRollCooldown);
            DiceBullet.RollAllDices();

            LeanTween.value(gameObject, 0f, 180f, .2f).setOnUpdate((float val) =>
            {
                prism.transform.localRotation = Quaternion.Euler(0f, val, 0f);
            });
        }
    }
}
