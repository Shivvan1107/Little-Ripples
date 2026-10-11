
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("References")]
    public BoatHealth boatHealth;
    public BoatTrashCollector collector;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text finalCoinsText;
    public Button restartButton;

    private void Awake()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    private void OnEnable()
    {
        if (boatHealth != null)
            boatHealth.OnBoatDestroyed += ShowGameOver;
    }

    private void OnDisable()
    {
        if (boatHealth != null)
            boatHealth.OnBoatDestroyed -= ShowGameOver;
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (collector != null && finalCoinsText != null)
        {
            finalCoinsText.text =
                "Coins Collected: " + collector.coins;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveListener(RestartGame);
    }
}
