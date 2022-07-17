using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class Boss : MonoBehaviour
{
    [SerializeField]
    List<DiceData> dices = new List<DiceData>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)  // "Player" layer
        {
            Debug.Log("Player hit by: " + gameObject.name);
            collision.gameObject.GetComponentInParent<Player>().PopLastElement();
        }

        if (collision.gameObject.layer == 9)  // "FriendlyBullet" layer
        {
            Debug.Log("Hit: " + gameObject.name);

            var diceColl = collision.gameObject.GetComponentInParent<DiceCollection>();

            int i = 0;
            int count = dices.Count;
            while (i < count)
            {
                if (diceColl.PopDices(dices[i]))
                {
                    dices.RemoveAt(i);
                    count--;
                }
                else
                {
                    i++;
                }
            }

            if(dices.Count <= 0)
            {
                Timing.RunCoroutine(_Death().CancelWith(gameObject));
            }
        }
    }

    IEnumerator<float> _Death()
    {
        AudioManager.Instance.PlayFX_koBoss();

        yield return Timing.WaitForSeconds(2f);

        GameManager.Instance.LoadNextScene();
        yield break;
    }
}
