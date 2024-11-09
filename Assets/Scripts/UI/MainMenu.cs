using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private Button newGameButton;
    private Button continueButton;
    private Button quitButton;
    private void Awake()
    {
        newGameButton = transform.GetChild(1).GetComponent<Button>();
        continueButton = transform.GetChild(2).GetComponent<Button>();
        quitButton = transform.GetChild(3).GetComponent<Button>();

        quitButton.onClick.AddListener(QuiteGame);
        newGameButton.onClick.AddListener(NewGame);
        newGameButton.onClick.AddListener(ContinueGame);
    }

    private void QuiteGame()
    {
        Application.Quit();
    }

    private void NewGame()
    {
        PlayerPrefs.DeleteAll();
        SceneController.Instance.TransitionToFirstLevel();
    }

    private void ContinueGame()
    {
        SceneController.Instance.TransitionToLoadGame();
    }
}
