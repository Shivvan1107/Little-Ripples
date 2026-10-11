
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIButtonClickSound : MonoBehaviour
{
    [Header("Click Sound")]
    [SerializeField] private AudioClip clickSound;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.65f;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button cancelRestartButton;
    [SerializeField] private Button confirmRestartButton;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.ignoreListenerPause = true;

        RegisterButtons();
    }

    private void RegisterButtons()
    {
        RegisterButton(continueButton);
        RegisterButton(restartButton);
        RegisterButton(cancelRestartButton);
        RegisterButton(confirmRestartButton);
    }

    private void RegisterButton(Button button)
    {
        if (button != null)
            button.onClick.AddListener(PlayClick);
    }

    public void PlayClick()
    {
        if (clickSound == null)
            return;

        // Creates a short-lived 2D sound so it survives
        // the scene reload when Confirm Restart is clicked.
        GameObject soundObject = new GameObject("UI Click Audio");

        DontDestroyOnLoad(soundObject);

        AudioSource source = soundObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.volume = volume;
        source.ignoreListenerPause = true;
        source.clip = clickSound;

        source.Play();

        Destroy(soundObject, clickSound.length + 0.2f);
    }

    private void OnDestroy()
    {
        UnregisterButton(continueButton);
        UnregisterButton(restartButton);
        UnregisterButton(cancelRestartButton);
        UnregisterButton(confirmRestartButton);
    }

    private void UnregisterButton(Button button)
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClick);
    }
}
