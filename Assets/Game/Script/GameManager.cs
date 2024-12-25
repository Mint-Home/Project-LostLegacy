using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Character playerCharacter;
    public GameUIManager UI_Manager;

    bool gameIsOver;
    private void Awake()
    {
        playerCharacter = GameObject.FindWithTag("Player").GetComponent<Character>();
    }

    public void GameOver()
    {
        UI_Manager.ShowGameOverUI();
    }

    public void GameIsFinished()
    {
        UI_Manager.ShowGameIsFinishedUI();
    }

    void Update()
    {
        if (gameIsOver)
        {
            return;
        }
        if (playerCharacter.currentState == Character.CharacterState.Dead)
        {
            gameIsOver = true;
            GameOver();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UI_Manager.TogglePauseUI();
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReturnToTheMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
