
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PondMainMenu : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject restartConfirmationPanel;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button cancelRestartButton;
    [SerializeField] private Button confirmRestartButton;

    [Header("Boat")]
    [SerializeField] private BoatMovement boatMovement;

    [Header("Save Settings")]
    [SerializeField] private string saveKey = "CleanUp_Save_V1";

    // Allows Restart to launch directly into gameplay
    // after the scene has reloaded.
    private static bool startImmediatelyAfterRestart = false;

    private bool menuOpen = false;

    private void Awake()
    {
        // Connect all four buttons automatically.
        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(OpenRestartConfirmation);

        if (cancelRestartButton != null)
            cancelRestartButton.onClick.AddListener(CancelRestart);

        if (confirmRestartButton != null)
            confirmRestartButton.onClick.AddListener(ConfirmRestart);
    }

    private void Start()
    {
        if (boatMovement == null)
            boatMovement = FindObjectOfType<BoatMovement>();

        if (startImmediatelyAfterRestart)
        {
            startImmediatelyAfterRestart = false;
            StartGameplay();
            return;
        }

        OpenMainMenu();
    }

    private void OpenMainMenu()
    {
        menuOpen = true;

        Time.timeScale = 0f;

        if (boatMovement != null)
            boatMovement.enabled = false;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);

        RefreshContinueButton();
    }

    private void RefreshContinueButton()
    {
        if (continueButton == null)
            return;

        // Continue is only available when save data exists.
        bool hasSave = PlayerPrefs.HasKey(saveKey);

        continueButton.interactable = hasSave;
    }

    public void ContinueGame()
    {
        if (!PlayerPrefs.HasKey(saveKey))
            return;

        StartGameplay();
    }

    private void StartGameplay()
    {
        menuOpen = false;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);

        Time.timeScale = 1f;

        if (boatMovement != null)
            boatMovement.enabled = true;
    }

    public void OpenRestartConfirmation()
    {
        if (!menuOpen)
            return;

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);

        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(true);

        Time.timeScale = 0f;
    }

    public void CancelRestart()
    {
        if (restartConfirmationPanel != null)
            restartConfirmationPanel.SetActive(false);

        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        RefreshContinueButton();
    }

    public void ConfirmRestart()
    {
        // Reset progress using the existing save system.
        if (SaveManager.Instance != null)
        {
            SaveManager.Instance.ResetProgress();

            // Prevent the outgoing scene from continuing
            // to autosave its old state during reloading.
            SaveManager.Instance.enabled = false;
        }
        else
        {
            PlayerPrefs.DeleteKey(saveKey);
            PlayerPrefs.Save();
        }

        // The next scene load starts gameplay immediately.
        startImmediatelyAfterRestart = true;

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
            restartButton.onClick.RemoveListener(OpenRestartConfirmation);

        if (cancelRestartButton != null)
            cancelRestartButton.onClick.RemoveListener(CancelRestart);

        if (confirmRestartButton != null)
            confirmRestartButton.onClick.RemoveListener(ConfirmRestart);
    }
}
