using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;
using MEC;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] float diceRollCooldown = 1f;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Timing.RunCoroutine(_RollCountdown().CancelWith(gameObject));
    }

    IEnumerator<float> _RollCountdown()
    {
        while (Game.IsIn(GameState.Game))
        {
            yield return Timing.WaitForSeconds(diceRollCooldown);
            DiceBullet.RollAllDices();
        }
    }
}

public enum GameState { Menu, Game }
public static class Game
{
    private static GameState state = GameState.Game;
    public static GameState State { get => state; }

    public static bool IsIn(GameState testState)
    {
        return State == testState;
    }
}
