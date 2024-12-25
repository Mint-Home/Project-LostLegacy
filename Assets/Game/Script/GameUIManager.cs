using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    public GameManager GM;

    public TMPro.TextMeshProUGUI coinText;
    public Slider healthSlider;

    public GameObject UI_Pause;
    public GameObject UI_GameOver;
    public GameObject UI_GameIsFinished;
    private enum UI_State
    {
        gamePlay, pause, gameOver, gameIsFinished
    }

    UI_State currentState;

    private void Awake()
    {
        SwitchUIState(UI_State.gamePlay);
    }
    // Update is called once per frame
    void Update()
    {
        if (GM != null && GM.playerCharacter != null)
        {
            healthSlider.value = GM.playerCharacter.GetComponent<Health>().currentHealthPercentage;
            coinText.text = GM.playerCharacter.coin.ToString();
        }
    }

    private void SwitchUIState(UI_State state)  //why private here
    {
        UI_Pause.SetActive(false);
        UI_GameOver.SetActive(false);
        UI_GameIsFinished.SetActive(false);

        Time.timeScale = 1;

        switch(state)
        {
            case UI_State.gamePlay:
                break;
            case UI_State.pause:
                Time.timeScale = 0;
                UI_Pause.SetActive(true);
                break;
            case UI_State.gameOver:
                UI_GameOver.SetActive(true); 
                break;
            case UI_State.gameIsFinished:
                UI_GameIsFinished.SetActive(true);
                break;
        }

        currentState = state;
    }

    public void TogglePauseUI()
    {
        if(currentState == UI_State.gamePlay)
        {
            SwitchUIState(UI_State.pause);
        }
        else if (currentState == UI_State.pause)
        {
            SwitchUIState(UI_State.gamePlay);
        }
    }

    public void Buttom_Restart()
    {
        GM.RestartScene();
    }

    public void Buttom_MainMenu()
    {
        GM.ReturnToTheMainMenu();
    }

    public void ShowGameOverUI()
    {
        SwitchUIState(UI_State.gameOver);
    }
    public void ShowGameIsFinishedUI()
    {
        SwitchUIState(UI_State.gameIsFinished);
    }
}
