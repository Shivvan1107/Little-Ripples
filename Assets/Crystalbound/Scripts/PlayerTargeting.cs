using UnityEngine;

public class PlayerTargeting : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private Camera mainCamera;

    private EnemyTarget currentTarget;

    private void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void Update()
    {
        HandleMouseInput();

        CheckCurrentTarget();
    }

    private void HandleMouseInput()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(
            Input.mousePosition
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            EnemyTarget enemy =
                hit.collider.GetComponentInParent<EnemyTarget>();

            if (enemy != null)
            {
                SelectTarget(enemy);
            }
        }
    }

    private void SelectTarget(EnemyTarget newTarget)
    {
        if (currentTarget != null)
        {
            currentTarget.SetSelected(false);
        }

        currentTarget = newTarget;

        if (currentTarget != null)
        {
            currentTarget.SetSelected(true);

            Debug.Log(
                "Selected Enemy: " +
                currentTarget.gameObject.name
            );
        }
    }

    private void CheckCurrentTarget()
    {
        if (currentTarget == null)
            return;

        EnemyHealth enemyHealth =
            currentTarget.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            enemyHealth =
                currentTarget.GetComponentInParent<EnemyHealth>();
        }

        if (enemyHealth == null)
        {
            ClearTarget();
            return;
        }

        if (enemyHealth.GetCurrentHealth() <= 0f)
        {
            ClearTarget();
        }
    }

    private void ClearTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.SetSelected(false);
        }

        currentTarget = null;
    }

    public EnemyTarget GetCurrentTarget()
    {
        return currentTarget;
    }
}