using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject setupPanel;

    public GameObject[] pages;

    public void Start()
    {
        setupPanel.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }

    public void LoadMainMenu()
    {
        setupPanel.SetActive(false);
        mainMenuCanvas.SetActive(true);
    }
    
    public void LoadSetup()
    {
        mainMenuCanvas.SetActive(false);
        setupPanel.SetActive(true);
        ActivateTab(0);
    }

    public void ActivateTab(int tabNum)
    {
        foreach (var tab in pages)
        {
            tab.SetActive(false);
        }

        pages[tabNum].SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
