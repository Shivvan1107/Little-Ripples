
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RandomScatterTool : EditorWindow
{
    private enum ToolMode
    {
        BatchScatter,
        Paint,
        Erase
    }

    private ToolMode toolMode = ToolMode.Paint;

    // ---------- PREFABS ----------
    private readonly List<GameObject> prefabs =
        new List<GameObject>();

    // ---------- SCATTER SETTINGS ----------
    private BoxCollider scatterArea;
    private Transform outputParent;

    private int scatterCount = 100;
    private int randomSeed = 12345;
    private int maxAttempts = 30;
    private float minSpacing = 1f;

    // ---------- BRUSH SETTINGS ----------
    private float brushRadius = 5f;
    private int brushDensity = 8;
    private float strokeStep = 0.5f;
    private bool brushEnabled = false;

    // ---------- TRANSFORM SETTINGS ----------
    private float minScale = 0.8f;
    private float maxScale = 1.2f;
    private bool randomYaw = true;
    private bool alignToSurface = false;
    private float surfaceOffset = 0f;

    // ---------- RAYCAST SETTINGS ----------
    private LayerMask surfaceMask = ~0;
    private float raycastHeight = 100f;
    private float maxSlope = 90f;

    // ---------- INTERNAL STATE ----------
    private System.Random random;
    private Transform generatedRoot;
    private Transform currentStrokeRoot;

    private bool isPainting;
    private bool hasLastPaintPoint;
    private Vector3 lastPaintPoint;
    private float distanceSinceStamp;

    private int currentUndoGroup = -1;
    private Vector2 scrollPosition;

    private readonly List<Vector3> placedPositions =
        new List<Vector3>();

    private readonly List<Vector3> previewPoints =
        new List<Vector3>();

    [MenuItem("Tools/Environment/Random Scatter Tool")]
    public static void OpenWindow()
    {
        GetWindow<RandomScatterTool>(
            "Random Scatter"
        );
    }

    private void OnEnable()
    {
        if (prefabs.Count == 0)
            prefabs.Add(null);

        random = new System.Random(randomSeed);

        SceneView.duringSceneGui += OnSceneGUI;
        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnDisable()
    {
        EndStroke();

        SceneView.duringSceneGui -= OnSceneGUI;
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        RebuildPlacedPositions();
        Repaint();
        SceneView.RepaintAll();
    }

    // ========================================
    // EDITOR WINDOW
    // ========================================

    private void OnGUI()
    {
        scrollPosition =
            EditorGUILayout.BeginScrollView(
                scrollPosition
            );

        EditorGUILayout.LabelField(
            "RANDOM SCATTER TOOL V2",
            EditorStyles.boldLabel
        );

        EditorGUILayout.Space(8);

        ToolMode previousMode = toolMode;

        toolMode = (ToolMode)GUILayout.Toolbar(
            (int)toolMode,
            new string[]
            {
                "Batch Scatter",
                "Paint Brush",
                "Erase Brush"
            }
        );

        if (previousMode != toolMode)
        {
            EndStroke();
            SceneView.RepaintAll();
        }

        EditorGUILayout.Space(12);

        DrawPrefabSection();
        DrawAreaSection();

        if (toolMode == ToolMode.BatchScatter)
            DrawBatchSection();
        else
            DrawBrushSection();

        DrawTransformSection();
        DrawSurfaceSection();
        DrawActions();

        EditorGUILayout.EndScrollView();
    }

    private void DrawPrefabSection()
    {
        EditorGUILayout.LabelField(
            "PREFAB LIBRARY",
            EditorStyles.boldLabel
        );

        for (int i = 0; i < prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            prefabs[i] = (GameObject)
                EditorGUILayout.ObjectField(
                    "Prefab " + (i + 1),
                    prefabs[i],
                    typeof(GameObject),
                    false
                );

            if (GUILayout.Button(
                "X",
                GUILayout.Width(25)))
            {
                prefabs.RemoveAt(i);
                GUIUtility.ExitGUI();
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("+ Add Prefab"))
        {
            prefabs.Add(null);
        }

        EditorGUILayout.Space(12);
    }

    private void DrawAreaSection()
    {
        EditorGUILayout.LabelField(
            "SCENE SETUP",
            EditorStyles.boldLabel
        );

        scatterArea = (BoxCollider)
            EditorGUILayout.ObjectField(
                "Scatter Area",
                scatterArea,
                typeof(BoxCollider),
                true
            );

        outputParent = (Transform)
            EditorGUILayout.ObjectField(
                "Output Parent",
                outputParent,
                typeof(Transform),
                true
            );

        EditorGUILayout.Space(12);
    }

    private void DrawBatchSection()
    {
        EditorGUILayout.LabelField(
            "BATCH SCATTER",
            EditorStyles.boldLabel
        );

        scatterCount = Mathf.Max(
            1,
            EditorGUILayout.IntField(
                "Object Count",
                scatterCount
            )
        );

        maxAttempts = Mathf.Clamp(
            EditorGUILayout.IntField(
                "Attempts Per Object",
                maxAttempts
            ),
            1,
            1000
        );

        randomSeed = EditorGUILayout.IntField(
            "Random Seed",
            randomSeed
        );

        minSpacing = Mathf.Max(
            0f,
            EditorGUILayout.FloatField(
                "Minimum Spacing",
                minSpacing
            )
        );

        EditorGUILayout.Space(12);
    }

    private void DrawBrushSection()
    {
        EditorGUILayout.LabelField(
            "SCENE BRUSH",
            EditorStyles.boldLabel
        );

        brushEnabled = EditorGUILayout.Toggle(
            "Enable Scene Brush",
            brushEnabled
        );

        brushRadius = EditorGUILayout.Slider(
            "Brush Radius",
            brushRadius,
            0.25f,
            50f
        );

        brushDensity = EditorGUILayout.IntSlider(
            "Objects Per Stamp",
            brushDensity,
            1,
            50
        );

        strokeStep = EditorGUILayout.Slider(
            "Stroke Step",
            strokeStep,
            0.1f,
            2f
        );

        minSpacing = Mathf.Max(
            0f,
            EditorGUILayout.FloatField(
                "Minimum Spacing",
                minSpacing
            )
        );

        EditorGUILayout.HelpBox(
            "Enable the brush, then left-click and " +
            "drag in Scene view. Hold Alt to " +
            "navigate normally. Ctrl+Z undoes " +
            "the entire stroke.",
            MessageType.Info
        );

        EditorGUILayout.Space(12);
    }

    private void DrawTransformSection()
    {
        EditorGUILayout.LabelField(
            "RANDOM TRANSFORMS",
            EditorStyles.boldLabel
        );

        minScale = EditorGUILayout.FloatField(
            "Minimum Scale",
            minScale
        );

        maxScale = EditorGUILayout.FloatField(
            "Maximum Scale",
            maxScale
        );

        randomYaw = EditorGUILayout.Toggle(
            "Random Y Rotation",
            randomYaw
        );

        alignToSurface = EditorGUILayout.Toggle(
            "Align To Surface",
            alignToSurface
        );

        surfaceOffset = EditorGUILayout.FloatField(
            "Surface Offset",
            surfaceOffset
        );

        EditorGUILayout.Space(12);
    }

    private void DrawSurfaceSection()
    {
        EditorGUILayout.LabelField(
            "SURFACE FILTERING",
            EditorStyles.boldLabel
        );

        // Serialized LayerMask field lets Unity display
        // the standard layer-selection dropdown.
        SerializedObject serializedWindow =
            new SerializedObject(this);

        SerializedProperty maskProperty =
            serializedWindow.FindProperty("surfaceMask");

        if (maskProperty != null)
        {
            EditorGUILayout.PropertyField(
                maskProperty,
                new GUIContent("Surface Layers")
            );

            serializedWindow.ApplyModifiedProperties();
        }

        raycastHeight = Mathf.Max(
            0.1f,
            EditorGUILayout.FloatField(
                "Raycast Height",
                raycastHeight
            )
        );

        maxSlope = EditorGUILayout.Slider(
            "Maximum Slope",
            maxSlope,
            0f,
            90f
        );

        EditorGUILayout.Space(15);
    }

    private void DrawActions()
    {
        if (toolMode == ToolMode.BatchScatter)
        {
            if (GUILayout.Button(
                "SCATTER OBJECTS",
                GUILayout.Height(35)))
            {
                BatchScatter();
            }
        }
        else
        {
            if (GUILayout.Button("Focus Scene View"))
            {
                SceneView scene = SceneView.lastActiveSceneView;

                if (scene != null)
                    scene.Focus();
            }
        }

        if (GUILayout.Button(
            "Clear Generated Objects",
            GUILayout.Height(27)))
        {
            ClearGenerated();
        }
    }

    // ========================================
    // SCENE VIEW BRUSH
    // ========================================

    private void OnSceneGUI(SceneView sceneView)
    {
        if (!brushEnabled ||
            toolMode == ToolMode.BatchScatter)
            return;

        Event e = Event.current;

        if (e == null)
            return;

        // Allow normal Unity camera navigation.
        if (e.alt)
        {
            if (isPainting)
                EndStroke();

            return;
        }

        if (e.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(
                GUIUtility.GetControlID(
                    FocusType.Passive
                )
            );
        }

        bool hasSurface = GetMouseSurface(
            e.mousePosition,
            out RaycastHit hit
        );

        if (hasSurface)
        {
            DrawBrushPreview(hit.point, hit.normal);
        }

        // Repaint brush preview as the mouse moves.
        if (e.type == EventType.MouseMove)
        {
            sceneView.Repaint();
        }

        if (e.button == 0 &&
            e.type == EventType.MouseDown &&
            !e.control &&
            !e.command)
        {
            if (!hasSurface)
                return;

            BeginStroke();

            isPainting = true;
            hasLastPaintPoint = true;
            lastPaintPoint = hit.point;
            distanceSinceStamp = 0f;

            Stamp(hit.point);

            e.Use();
        }

        if (e.button == 0 &&
            e.type == EventType.MouseDrag &&
            isPainting)
        {
            if (hasSurface)
            {
                PaintAlongStroke(hit.point);
            }
            else
            {
                // Avoid drawing a long line when the
                // cursor leaves the paintable surface.
                hasLastPaintPoint = false;
                distanceSinceStamp = 0f;
            }

            e.Use();
        }

        if (e.button == 0 &&
            e.type == EventType.MouseUp &&
            isPainting)
        {
            EndStroke();
            e.Use();
        }
    }

    private void DrawBrushPreview(
        Vector3 point,
        Vector3 normal)
    {
        Color color = toolMode == ToolMode.Erase
            ? new Color(1f, 0.3f, 0.25f, 1f)
            : new Color(0.25f, 1f, 0.45f, 1f);

        Handles.color = color;

        Handles.DrawWireDisc(
            point,
            Vector3.up,
            brushRadius
        );

        Handles.color = new Color(
            color.r,
            color.g,
            color.b,
            0.13f
        );

        Handles.DrawSolidDisc(
            point,
            Vector3.up,
            brushRadius
        );

        Handles.color = Color.white;

        Handles.Label(
            point + Vector3.up * 0.25f,
            toolMode == ToolMode.Erase
                ? "ERASE"
                : "PAINT"
        );
    }

    private void PaintAlongStroke(Vector3 point)
    {
        if (!hasLastPaintPoint)
        {
            lastPaintPoint = point;
            hasLastPaintPoint = true;
            distanceSinceStamp = 0f;
            Stamp(point);
            return;
        }

        float distance = Vector3.Distance(
            lastPaintPoint,
            point
        );

        float interval = Mathf.Max(
            0.01f,
            brushRadius * strokeStep
        );

        Vector3 start = lastPaintPoint;

        // Safety limit against huge scene jumps.
        int stampLimit = 50;
        int stamps = 0;

        while (distanceSinceStamp + distance >= interval &&
               distance > 0.0001f &&
               stamps < stampLimit)
        {
            float needed =
                interval - distanceSinceStamp;

            float t = Mathf.Clamp01(
                needed / distance
            );

            Vector3 stampPoint = Vector3.Lerp(
                start,
                point,
                t
            );

            Stamp(stampPoint);

            start = stampPoint;
            distance = Vector3.Distance(
                start,
                point
            );

            distanceSinceStamp = 0f;
            stamps++;
        }

        distanceSinceStamp += distance;
        lastPaintPoint = point;
    }

    // ========================================
    // STROKE + UNDO
    // ========================================

    private void BeginStroke()
    {
        EndStroke();

        Undo.IncrementCurrentGroup();

        currentUndoGroup = Undo.GetCurrentGroup();

        Undo.SetCurrentGroupName(
            toolMode == ToolMode.Erase
                ? "Erase Scatter Stroke"
                : "Paint Scatter Stroke"
        );

        if (toolMode == ToolMode.Paint)
        {
            EnsureGeneratedRoot();

            GameObject strokeObject = new GameObject(
                "Brush Stroke"
            );

            Undo.RegisterCreatedObjectUndo(
                strokeObject,
                "Create Brush Stroke"
            );

            Undo.SetTransformParent(
                strokeObject.transform,
                generatedRoot,
                "Parent Brush Stroke"
            );

            currentStrokeRoot =
                strokeObject.transform;
        }

        random = new System.Random(
            unchecked(Environment.TickCount)
        );
    }

    private void EndStroke()
    {
        if (currentUndoGroup >= 0)
        {
            Undo.CollapseUndoOperations(
                currentUndoGroup
            );
        }

        currentUndoGroup = -1;
        currentStrokeRoot = null;
        isPainting = false;
        hasLastPaintPoint = false;
        distanceSinceStamp = 0f;

        SceneView.RepaintAll();
    }

    // ========================================
    // BRUSH STAMP
    // ========================================

    private void Stamp(Vector3 center)
    {
        if (toolMode == ToolMode.Erase)
        {
            EraseAt(center);
            return;
        }

        List<GameObject> validPrefabs =
            GetValidPrefabs();

        if (validPrefabs.Count == 0)
            return;

        for (int i = 0; i < brushDensity; i++)
        {
            float angle = RandomFloat(
                0f,
                Mathf.PI * 2f
            );

            // sqrt distributes points uniformly
            // across the circular brush area.
            float distance = Mathf.Sqrt(
                RandomFloat(0f, 1f)
            ) * brushRadius;

            Vector3 candidate = center +
                new Vector3(
                    Mathf.Cos(angle) * distance,
                    0f,
                    Mathf.Sin(angle) * distance
                );

            if (!GetSurfaceAtXZ(
                candidate.x,
                candidate.z,
                center.y,
                out RaycastHit hit))
                continue;

            if (!IsSpacingValid(hit.point))
                continue;

            GameObject prefab = validPrefabs[
                random.Next(validPrefabs.Count)
            ];

            CreateInstance(
                prefab,
                hit,
                currentStrokeRoot
            );
        }

        SceneView.RepaintAll();
    }

    // ========================================
    // BATCH SCATTER
    // ========================================

    private void BatchScatter()
    {
        if (scatterArea == null)
        {
            EditorUtility.DisplayDialog(
                "Missing Scatter Area",
                "Assign a Box Collider first.",
                "OK"
            );
            return;
        }

        List<GameObject> validPrefabs =
            GetValidPrefabs();

        if (validPrefabs.Count == 0)
        {
            EditorUtility.DisplayDialog(
                "Missing Prefabs",
                "Assign prefab assets first.",
                "OK"
            );
            return;
        }

        random = new System.Random(randomSeed);

        Undo.IncrementCurrentGroup();

        int undoGroup = Undo.GetCurrentGroup();

        Undo.SetCurrentGroupName(
            "Batch Scatter Objects"
        );

        EnsureGeneratedRoot();

        GameObject batchGroup = new GameObject(
            "Batch Scatter"
        );

        Undo.RegisterCreatedObjectUndo(
            batchGroup,
            "Create Batch Group"
        );

        Undo.SetTransformParent(
            batchGroup.transform,
            generatedRoot,
            "Parent Batch Group"
        );

        int spawned = 0;

        try
        {
            // Sample inside the actual local BoxCollider
            // volume, including rotated areas.
            for (int i = 0; i < scatterCount; i++)
            {
                for (int attempt = 0;
                     attempt < maxAttempts;
                     attempt++)
                {
                    Vector3 local = scatterArea.center +
                        new Vector3(
                            RandomFloat(-0.5f, 0.5f) *
                                scatterArea.size.x,
                            RandomFloat(-0.5f, 0.5f) *
                                scatterArea.size.y,
                            RandomFloat(-0.5f, 0.5f) *
                                scatterArea.size.z
                        );

                    Vector3 candidate =
                        scatterArea.transform.TransformPoint(
                            local
                        );

                    if (!GetSurfaceAtXZ(
                        candidate.x,
                        candidate.z,
                        candidate.y,
                        out RaycastHit hit))
                        continue;

                    Vector3 hitLocal =
                        scatterArea.transform.InverseTransformPoint(
                            hit.point
                        ) - scatterArea.center;

                    Vector3 halfSize =
                        scatterArea.size * 0.5f;

                    if (Mathf.Abs(hitLocal.x) > halfSize.x ||
                        Mathf.Abs(hitLocal.y) > halfSize.y ||
                        Mathf.Abs(hitLocal.z) > halfSize.z)
                        continue;

                    if (!IsSpacingValid(hit.point))
                        continue;

                    GameObject prefab = validPrefabs[
                        random.Next(validPrefabs.Count)
                    ];

                    CreateInstance(
                        prefab,
                        hit,
                        batchGroup.transform
                    );

                    spawned++;
                    break;
                }
            }
        }
        finally
        {
            Undo.CollapseUndoOperations(undoGroup);
        }

        Debug.Log(
            "Random Scatter: " + spawned +
            " / " + scatterCount +
            " objects placed."
        );

        SceneView.RepaintAll();
    }

    // ========================================
    // SURFACE DETECTION
    // ========================================

    private bool GetMouseSurface(
        Vector2 mousePosition,
        out RaycastHit result)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(
            mousePosition
        );

        return FindSurfaceHit(
            ray,
            100000f,
            out result
        );
    }

    private bool GetSurfaceAtXZ(
        float x,
        float z,
        float referenceY,
        out RaycastHit result)
    {
        float height = Mathf.Max(
            0.1f,
            raycastHeight
        );

        Ray ray = new Ray(
            new Vector3(
                x,
                referenceY + height,
                z
            ),
            Vector3.down
        );

        return FindSurfaceHit(
            ray,
            height * 2f,
            out result
        );
    }

    private bool FindSurfaceHit(
        Ray ray,
        float distance,
        out RaycastHit result)
    {
        result = default(RaycastHit);

        RaycastHit[] hits = Physics.RaycastAll(
            ray,
            distance,
            surfaceMask,
            QueryTriggerInteraction.Ignore
        );

        Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider == null)
                continue;

            // Ignore all objects made by this tool.
            if (generatedRoot != null &&
                hit.collider.transform.IsChildOf(
                    generatedRoot))
                continue;

            // Do not use the selection box as a surface.
            if (scatterArea != null &&
                hit.collider == scatterArea)
                continue;

            float slope = Vector3.Angle(
                hit.normal,
                Vector3.up
            );

            if (slope > maxSlope)
                continue;

            result = hit;
            return true;
        }

        return false;
    }

    // ========================================
    // PREFAB CREATION
    // ========================================

    private void CreateInstance(
        GameObject prefab,
        RaycastHit hit,
        Transform parent)
    {
        GameObject instance =
            PrefabUtility.InstantiatePrefab(
                prefab,
                hit.collider.gameObject.scene
            ) as GameObject;

        if (instance == null)
            return;

        Undo.RegisterCreatedObjectUndo(
            instance,
            "Scatter Prefab"
        );

        if (parent != null)
        {
            Undo.SetTransformParent(
                instance.transform,
                parent,
                "Parent Scatter Prefab"
            );
        }

        float yaw = randomYaw
            ? RandomFloat(0f, 360f)
            : 0f;

        Quaternion rotation =
            Quaternion.Euler(0f, yaw, 0f);

        if (alignToSurface)
        {
            rotation =
                Quaternion.FromToRotation(
                    Vector3.up,
                    hit.normal
                ) * rotation;
        }

        float scale = RandomFloat(
            Mathf.Max(
                0.001f,
                Mathf.Min(minScale, maxScale)
            ),
            Mathf.Max(
                0.001f,
                Mathf.Max(minScale, maxScale)
            )
        );

        instance.transform.SetPositionAndRotation(
            hit.point + hit.normal * surfaceOffset,
            rotation
        );

        instance.transform.localScale *= scale;

        // Record prefab instance transform changes.
        PrefabUtility.RecordPrefabInstancePropertyModifications(
            instance.transform
        );

        placedPositions.Add(hit.point);
    }

    private List<GameObject> GetValidPrefabs()
    {
        List<GameObject> valid =
            new List<GameObject>();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab != null &&
                PrefabUtility.IsPartOfPrefabAsset(
                    prefab))
            {
                valid.Add(prefab);
            }
        }

        return valid;
    }

    private bool IsSpacingValid(Vector3 position)
    {
        float sqrSpacing = minSpacing * minSpacing;

        foreach (Vector3 existing in placedPositions)
        {
            float dx = existing.x - position.x;
            float dz = existing.z - position.z;

            if (dx * dx + dz * dz < sqrSpacing)
                return false;
        }

        return true;
    }

    private float RandomFloat(float min, float max)
    {
        if (random == null)
            random = new System.Random(randomSeed);

        return min +
            (float)random.NextDouble() * (max - min);
    }

    // ========================================
    // ERASER
    // ========================================

    private void EraseAt(Vector3 center)
    {
        if (generatedRoot == null)
            return;

        List<Transform> objectsToErase =
            new List<Transform>();

        foreach (Transform group in generatedRoot)
        {
            foreach (Transform child in group)
            {
                Vector3 delta =
                    child.position - center;

                delta.y = 0f;

                if (delta.sqrMagnitude <=
                    brushRadius * brushRadius)
                {
                    objectsToErase.Add(child);
                }
            }
        }

        foreach (Transform item in objectsToErase)
        {
            Undo.DestroyObjectImmediate(
                item.gameObject
            );
        }

        RebuildPlacedPositions();
        SceneView.RepaintAll();
    }

    // ========================================
    // GENERATED OBJECT MANAGEMENT
    // ========================================

    private void EnsureGeneratedRoot()
    {
        if (generatedRoot != null)
            return;

        GameObject root = new GameObject(
            "Random Scatter Generated"
        );

        Undo.RegisterCreatedObjectUndo(
            root,
            "Create Scatter Root"
        );

        if (outputParent != null)
        {
            Undo.SetTransformParent(
                root.transform,
                outputParent,
                "Parent Scatter Root"
            );
        }

        generatedRoot = root.transform;
        RebuildPlacedPositions();
    }

    private void RebuildPlacedPositions()
    {
        placedPositions.Clear();

        if (generatedRoot == null)
            return;

        foreach (Transform group in generatedRoot)
        {
            foreach (Transform item in group)
            {
                placedPositions.Add(item.position);
            }
        }
    }

    private void ClearGenerated()
    {
        EndStroke();

        if (generatedRoot == null)
            return;

        Undo.DestroyObjectImmediate(
            generatedRoot.gameObject
        );

        generatedRoot = null;
        placedPositions.Clear();

        SceneView.RepaintAll();
    }
}
