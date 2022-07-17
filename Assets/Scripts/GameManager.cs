using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;
using MEC;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] List<string> sceneNames = new List<string>();
    [SerializeField] int currentScene = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < sceneNames.Count; i++)
        {
            if (sceneNames[i] == SceneManager.GetActiveScene().name)
                currentScene = i;
        }
    }

    public void LoadNextScene()
    {
        currentScene++;
        LoadScene(sceneNames[currentScene]);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(sceneNames[currentScene]);
    }

    void LoadScene(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
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
