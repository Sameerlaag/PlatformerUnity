using UnityEngine;

[System.Serializable]
public class WallRun : MonoBehaviour
{
    [Header("Detection Settings")]
    public float wallCheckDistance = 0.7f;
    public float wallCheckHeight = 1.0f;
    public LayerMask wallMask;

    [Header("Activation Requirements")]
    [Tooltip("Minimum speed required to start wall running")]
    public float minSpeedToStart = 3f;

    [Header("Movement Settings")]
    public float wallRunSpeed = 8f;
    public float wallRunGravity = 0.5f;
    public float wallRunDuration = 3f;

    [Header("Jump Settings")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 6f;

    [Header("Debug Visualization")]
    public bool showDebugRays = true;
    public bool showGizmos = true;

    // State
    private Vector3 wallNormal;
    private Vector3 runDirection;
    private bool isWallRight;
    private bool isWallRunning;
    private float wallRunTimer;

    // Debug data
    private bool lastRightHit;
    private bool lastLeftHit;
    private Vector3 lastHitPoint;
    private Vector3 lastHitNormal;
    private float currentSpeed;
    private string debugMessage = "";

    // Cached components
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Public getters
    public bool IsRunning => isWallRunning;
    public bool IsWallRight => isWallRight;
    public Vector3 RunDirection => runDirection;
    public Vector3 WallNormal => wallNormal;

    /// <summary>
    /// Checks if a wall is present and if conditions are met to wall run
    /// </summary>
    public bool CanStartWallRun(Transform playerTransform)
    {
        // Must have sufficient forward momentum
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        currentSpeed = horizontalVelocity.magnitude;

        if (currentSpeed < minSpeedToStart)
        {
            debugMessage = $"Speed too low: {currentSpeed:F2} < {minSpeedToStart}";
            return false;
        }

        // Check for wall
        if (!DetectWall(playerTransform))
        {
            debugMessage = "No wall detected";
            return false;
        }

        // Must be approaching wall at sufficient angle (not head-on or parallel)
        Vector3 playerForward = playerTransform.forward;
        playerForward.y = 0;
        playerForward.Normalize();

        return true;
    }

    /// <summary>
    /// Detects wall on left or right side
    /// </summary>
    private bool DetectWall(Transform playerTransform)
    {
        Vector3 origin = playerTransform.position + Vector3.up * wallCheckHeight;
        lastRightHit = false;
        lastLeftHit = false;

        // Check right side
        if (Physics.Raycast(origin, playerTransform.right, out RaycastHit rightHit, wallCheckDistance, wallMask))
        {
            lastRightHit = true;
            lastHitPoint = rightHit.point;
            lastHitNormal = rightHit.normal;

            // Ensure it's actually a vertical wall (normal points mostly horizontal)
            if (Mathf.Abs(rightHit.normal.y) < 0.2f)
            {
                debugMessage = $"Right surface too angled (Y: {rightHit.normal.y:F2})";
                return false;
            }

            wallNormal = rightHit.normal;
            runDirection = Vector3.Cross(Vector3.up, wallNormal).normalized;
            isWallRight = true;
            return true;
        }

        // Check left side
        if (Physics.Raycast(origin, -playerTransform.right, out RaycastHit leftHit, wallCheckDistance, wallMask))
        {
            lastLeftHit = true;
            lastHitPoint = leftHit.point;
            lastHitNormal = leftHit.normal;

            // Ensure it's actually a vertical wall
            if (Mathf.Abs(leftHit.normal.y) > 0.3f)
            {
                debugMessage = $"Left surface too angled (Y: {leftHit.normal.y:F2})";
                return false;
            }

            wallNormal = leftHit.normal;
            runDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
            isWallRight = false;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if wall is still present (for continuing wall run)
    /// </summary>
    public bool CheckForWall(Transform playerTransform)
    {
        return DetectWall(playerTransform);
    }

    public void StartWallRun()
    {
        Debug.Log("start");
        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        Debug.Log($"<color=green>WALL RUN STARTED</color> on {(isWallRight ? "RIGHT" : "LEFT")} wall | Speed: {currentSpeed:F2}");
    }

    public void StopWallRun()
    {
        if (!isWallRunning) return; // Already stopped, don't log again

        isWallRunning = false;
        Debug.Log("<color=red>WALL RUN STOPPED</color>");
    }

    public bool UpdateTimer()
    {
        wallRunTimer -= Time.deltaTime;
        return wallRunTimer > 0f; // Just return status, don't stop here
    }

    // GIZMOS - Always visible in Scene view
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Transform t = transform;
        Vector3 origin = t.position + Vector3.up * wallCheckHeight;

        // Draw detection rays
        Gizmos.color = lastRightHit ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + t.right * wallCheckDistance);

        Gizmos.color = lastLeftHit ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + -t.right * wallCheckDistance);

        // Draw hit point and normal
        if (lastRightHit || lastLeftHit)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lastHitPoint, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lastHitPoint, lastHitPoint + lastHitNormal * 0.5f);
        }

        // Draw wall run state
        if (isWallRunning)
        {
            // Wall normal
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(t.position, t.position + wallNormal * 2f);

            // Run direction
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(t.position, t.position + runDirection * 2f);

            // Speed indicator
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(t.position + Vector3.up * 2f, 0.2f);
        }
    }

    // GUI - Always visible in Game view
    private void OnGUI()
    {
        if (!showDebugRays) return;

        GUIStyle style = new GUIStyle();
        style.fontSize = 14;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperLeft;

        string debugText = "=== WALL RUN DEBUG ===\n";
        debugText += $"Speed: {currentSpeed:F2} (min: {minSpeedToStart})\n";
        debugText += $"Right Hit: {lastRightHit}\n";
        debugText += $"Left Hit: {lastLeftHit}\n";
        debugText += $"Wall Running: {isWallRunning}\n";
        debugText += $"Timer: {wallRunTimer:F2}s\n";
        debugText += $"\n{debugMessage}";

        // Black background
        GUI.Box(new Rect(10, 10, 350, 180), "");
        GUI.Label(new Rect(15, 15, 340, 170), debugText, style);

        // Color indicator
        Color indicatorColor = isWallRunning ? Color.green :
                               (lastRightHit || lastLeftHit) ? Color.yellow : Color.red;
        GUI.backgroundColor = indicatorColor;
        GUI.Box(new Rect(370, 10, 20, 20), "");
    }
}