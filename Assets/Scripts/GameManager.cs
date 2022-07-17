using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility.Patterns;
using MEC;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] List<Scene> scenes = new List<Scene>();
    [SerializeField] int currentScene = 0;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < scenes.Count; i++)
        {
            if (scenes[i] == SceneManager.GetActiveScene())
                currentScene = i;
        }
    }

    public void LoadNextScene()
    {
        currentScene++;
        LoadScene(scenes[currentScene]);
    }

    public void ReloadCurrentScene()
    {
        LoadScene(scenes[currentScene]);
    }

    void LoadScene(Scene sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad.name);
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
