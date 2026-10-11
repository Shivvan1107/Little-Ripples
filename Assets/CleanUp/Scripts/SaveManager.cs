
using System;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    private const string SaveKey = "CleanUp_Save_V1";

    [Serializable]
    public class SaveData
    {
        public int coins = 0;
        public int collectedTrash = 0;

        public bool hasSavedPosition = false;
        public Vector3 boatPosition;
        public Quaternion boatRotation = Quaternion.identity;

        public bool lastSavedAtDeath = false;

        public List<string> restoredStructures =
            new List<string>();
    }

    [Header("References")]
    public Transform boat;
    public BoatHealth boatHealth;

    [Header("Auto Save")]
    public float autoSaveInterval = 5f;

    private SaveData data;
    private float saveTimer;
    private bool positionRestored;
    private bool deathEventSubscribed;

    public int SavedCoins => data.coins;
    public int SavedTrashCount => data.collectedTrash;

    public static bool HasSavedGame()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        LoadData();
    }

    private void Start()
    {
        if (boat == null)
        {
            BoatMovement movement =
                FindObjectOfType<BoatMovement>();

            if (movement != null)
                boat = movement.transform;
        }

        if (boatHealth == null && boat != null)
            boatHealth = boat.GetComponent<BoatHealth>();

        if (boat != null && data.hasSavedPosition)
        {
            Rigidbody rb = boat.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.position = data.boatPosition;
                rb.rotation = data.boatRotation;

                // Prevent unwanted movement after loading.
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            boat.SetPositionAndRotation(
                data.boatPosition,
                data.boatRotation
            );

            Physics.SyncTransforms();
        }

        positionRestored = true;

        if (boatHealth != null)
        {
            boatHealth.OnBoatDestroyed += OnBoatDestroyed;
            deathEventSubscribed = true;
        }
    }

    private void Update()
    {
        if (!positionRestored)
            return;

        if (boatHealth != null && boatHealth.IsDestroyed)
            return;

        saveTimer += Time.deltaTime;

        if (saveTimer >= Mathf.Max(0.5f, autoSaveInterval))
        {
            SaveBoatPosition();
            SaveToDisk();
            saveTimer = 0f;
        }
    }

    public void SaveCoins(int amount)
    {
        data.coins = Mathf.Max(0, amount);
        SaveToDisk();
    }

    public void SaveTrashCount(int count)
    {
        data.collectedTrash = Mathf.Max(0, count);
        SaveToDisk();
    }

    public bool IsStructureRestored(string structureID)
    {
        if (string.IsNullOrEmpty(structureID))
            return false;

        return data.restoredStructures.Contains(structureID);
    }

    public void SaveRestoredStructure(string structureID)
    {
        if (string.IsNullOrEmpty(structureID))
            return;

        if (!data.restoredStructures.Contains(structureID))
        {
            data.restoredStructures.Add(structureID);
            SaveToDisk();
        }
    }

    public void SaveBoatPosition()
    {
        if (boat == null)
            return;

        if (boatHealth != null && boatHealth.IsDestroyed)
            return;

        data.boatPosition = boat.position;
        data.boatRotation = boat.rotation;
        data.hasSavedPosition = true;
        data.lastSavedAtDeath = false;
    }

    private void OnBoatDestroyed()
    {
        if (boat != null)
        {
            data.boatPosition = boat.position;
            data.boatRotation = boat.rotation;
            data.hasSavedPosition = true;
            data.lastSavedAtDeath = true;
        }

        SaveToDisk();
    }

    public void SaveToDisk()
    {
        PlayerPrefs.SetString(
            SaveKey,
            JsonUtility.ToJson(data)
        );

        PlayerPrefs.Save();
    }

    private void LoadData()
    {
        data = new SaveData();

        if (!PlayerPrefs.HasKey(SaveKey))
            return;

        try
        {
            SaveData loaded = JsonUtility.FromJson<SaveData>(
                PlayerPrefs.GetString(SaveKey)
            );

            if (loaded != null)
                data = loaded;

            if (data.restoredStructures == null)
                data.restoredStructures =
                    new List<string>();
        }
        catch (Exception ex)
        {
            Debug.LogError(
                "Failed to load save: " + ex.Message
            );
        }
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();

        data = new SaveData();

        // A scene reload must follow to reset
        // objects already restored in memory.
        positionRestored = false;

        Debug.Log("Save deleted. Restarting level.");
    }

    [ContextMenu("Delete Saved Progress")]
    public void DeleteSavedProgress()
    {
        ResetProgress();
    }

    private void OnApplicationPause(bool paused)
    {
        if (!paused || !positionRestored)
            return;

        SaveBoatPosition();
        SaveToDisk();
    }

    private void OnApplicationQuit()
    {
        if (!positionRestored)
            return;

        SaveBoatPosition();
        SaveToDisk();
    }

    private void OnDestroy()
    {
        if (deathEventSubscribed && boatHealth != null)
            boatHealth.OnBoatDestroyed -= OnBoatDestroyed;

        if (Instance == this)
            Instance = null;
    }
}
