
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenuPanel;
    public GameObject restartConfirmationPanel;

    public Button continueButton;
    public Button restartButton;
    public Button confirmRestartButton;
    public Button cancelRestartButton;

    public BoatMovement boatMovement;

    // Only persists between scene reloads.
    private static bool startFreshImmediately = false;

    private void Start()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(
                ShowRestartConfirmation
            );

        if (confirmRestartButton != null)
            confirmRestartButton.onClick.AddListener(
                RestartLevel
            );

        if (cancelRestartButton != null)
            cancelRestartButton.onClick.AddListener(
                CancelRestart
            );

        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);

        if (startFreshImmediately)
        {
            startFreshImmediately = false;
            BeginGameplay();
        }
        else
        {
            OpenMenu();
        }
    }

    private void OpenMenu()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (boatMovement != null)
            boatMovement.enabled = false;

        if (continueButton != null)
            continueButton.interactable =
                SaveManager.HasSavedGame();

        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ContinueGame()
    {
        BeginGameplay();
    }

    private void BeginGameplay()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);

        if (boatMovement != null)
            boatMovement.enabled = true;

        Time.timeScale = 1f;
    }

    public void ShowRestartConfirmation()
    {
        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(true);
    }

    public void CancelRestart()
    {
        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        if (SaveManager.Instance != null)
            SaveManager.Instance.ResetProgress();

        startFreshImmediately = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void OnDestroy()
    {
        if (continueButton != null)
            continueButton.onClick.RemoveListener(ContinueGame);

        if (restartButton != null)
            restartButton.onClick.RemoveListener(
                ShowRestartConfirmation
            );

        if (confirmRestartButton != null)
            confirmRestartButton.onClick.RemoveListener(
                RestartLevel
            );

        if (cancelRestartButton != null)
            cancelRestartButton.onClick.RemoveListener(
                CancelRestart
            );
    }
}
