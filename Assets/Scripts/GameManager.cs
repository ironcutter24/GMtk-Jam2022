using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;
using MEC;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] float diceRollCooldown = 1f;

    [SerializeField] List<Scene> scenes = new List<Scene>();
    [SerializeField] int currentScene = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Timing.RunCoroutine(_RollCountdown().CancelWith(gameObject));
    }

    public void PlayerWin()
    {
        currentScene++;
        SceneManager.LoadScene(scenes[currentScene].name);
    }

    public void PlayerLose()
    {
        SceneManager.LoadScene(scenes[currentScene].name);
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
