
using UnityEngine;
using UnityEngine.UI;

public class PondPauseMenu : MonoBehaviour
{
    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenuPanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button closeGameButton;

    [Header("Gameplay")]
    [SerializeField] private BoatMovement boatMovement;

    [Header("Other Menus")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject restartConfirmationPanel;

    [Header("Settings")]
    [SerializeField] private KeyCode pauseKey = KeyCode.Escape;

    private bool isPaused;
    private bool hasStartedGameplay;

    private void Awake()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (closeGameButton != null)
            closeGameButton.onClick.AddListener(CloseGame);
    }

    private void Start()
    {
        if (boatMovement == null)
            boatMovement = FindObjectOfType<BoatMovement>();

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);
    }

    private void Update()
    {
        if (pauseMenuPanel == null)
            return;

        // Ignore ESC while the main menu or
        // restart confirmation is visible.
        if (IsMainMenuVisible())
        {
            hasStartedGameplay = false;
            return;
        }

        // Gameplay has begun after Continue or Restart.
        if (!hasStartedGameplay && Time.timeScale > 0f)
            hasStartedGameplay = true;

        if (!hasStartedGameplay)
            return;

        if (Input.GetKeyDown(pauseKey))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    private bool IsMainMenuVisible()
    {
        bool mainVisible =
            mainMenuPanel != null &&
            mainMenuPanel.activeInHierarchy;

        bool confirmationVisible =
            restartConfirmationPanel != null &&
            restartConfirmationPanel.activeInHierarchy;

        return mainVisible || confirmationVisible;
    }

    public void PauseGame()
    {
        if (isPaused || IsMainMenuVisible())
            return;

        isPaused = true;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(true);

        if (boatMovement != null)
            boatMovement.enabled = false;

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        if (boatMovement != null)
            boatMovement.enabled = true;
    }

    public void CloseGame()
    {
        // Exit pause state before closing.
        isPaused = false;
        hasStartedGameplay = false;

        if (pauseMenuPanel != null)
            pauseMenuPanel.SetActive(false);

        Time.timeScale = 1f;

        // Preserve current progress.
        if (SaveManager.Instance != null)
        {
            // SaveManager is responsible for
            // saving on pause or application exit.
            // Do not reset progress here.
        }

#if UNITY_WEBGL && !UNITY_EDITOR
        // Browsers generally cannot be closed by the game.
        // Return to the existing main menu.
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);

        if (boatMovement != null)
            boatMovement.enabled = false;

        Time.timeScale = 0f;
#else
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
#endif
    }

    private void OnDestroy()
    {
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(ResumeGame);

        if (closeGameButton != null)
            closeGameButton.onClick.RemoveListener(CloseGame);
    }
}
