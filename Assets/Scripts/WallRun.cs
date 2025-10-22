using UnityEngine;

[System.Serializable]
public class WallRun : MonoBehaviour
{
    [Header("Detection Settings")]
    public float wallCheckDistance = 0.7f;
    public float wallCheckHeight = 1.0f;
    public LayerMask wallMask;

    [Header("Activation Requirements")]
    [Tooltip("Minimum horizontal speed required to start wall running")]
    public float minSpeedToStart = 3f;

    [Header("Movement Settings")]
    public float wallRunSpeed = 8f;
    public float wallRunGravity = 0.5f;
    public float wallRunDuration = 1.5f;

    [Header("Jump Settings")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 6f;

    [Header("Debug Visualization")]
    public bool showDebugRays = true;
    public bool showGizmos = true;

    // --- State ---
    private Vector3 wallNormal;
    private Vector3 runDirection;
    private bool isWallRight;
    private bool isWallRunning;
    private float wallRunTimer;
    private float currentSpeed;

    // --- Debug Data ---
    private bool lastRightHit;
    private bool lastLeftHit;
    private Vector3 lastHitPoint;
    private Vector3 lastHitNormal;
    private string debugMessage = "";

    // --- Cached ---
    private Rigidbody rb;
    private PlayerManager player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = GetComponent<PlayerManager>();
    }

    public bool IsRunning => isWallRunning;
    public bool IsWallRight => isWallRight;
    public Vector3 RunDirection => runDirection;
    public Vector3 WallNormal => wallNormal;

    // ===========================================================
    // === ENTRY CONDITIONS ======================================
    // ===========================================================
    public bool CanStartWallRun(Transform playerTransform)
    {
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        currentSpeed = horizontalVelocity.magnitude;

        if (currentSpeed < minSpeedToStart)
        {
            debugMessage = $"Speed too low: {currentSpeed:F2} < {minSpeedToStart}";
            return false;
        }

        if (!DetectWall(playerTransform))
        {
            debugMessage = "No valid wall detected";
            return false;
        }

        // Check if approach angle is reasonable (not perpendicular to wall)
        Vector3 playerForward = playerTransform.forward;
        float dot = Vector3.Dot(playerForward, wallNormal);
        if (Mathf.Abs(dot) > 0.8f)
        {
            debugMessage = "Approach angle too steep";
            return false;
        }

        return true;
    }

    // ===========================================================
    // === WALL DETECTION ========================================
    // ===========================================================
    private bool DetectWall(Transform playerTransform)
    {
        Vector3 origin = playerTransform.position + Vector3.up * wallCheckHeight;
        lastRightHit = false;
        lastLeftHit = false;

        // --- Right side ---
        if (Physics.Raycast(origin, playerTransform.right, out RaycastHit rightHit, wallCheckDistance, wallMask))
        {
            lastRightHit = true;
            lastHitPoint = rightHit.point;
            lastHitNormal = rightHit.normal;

            if (Mathf.Abs(rightHit.normal.y) > 0.3f)
            {
                debugMessage = $"Right wall angled too much (Y: {rightHit.normal.y:F2})";
                return false;
            }

            wallNormal = rightHit.normal;
            runDirection = Vector3.Cross(Vector3.up, wallNormal).normalized;
            isWallRight = true;
            return true;
        }

        // --- Left side ---
        if (Physics.Raycast(origin, -playerTransform.right, out RaycastHit leftHit, wallCheckDistance, wallMask))
        {
            lastLeftHit = true;
            lastHitPoint = leftHit.point;
            lastHitNormal = leftHit.normal;

            if (Mathf.Abs(leftHit.normal.y) > 0.3f)
            {
                debugMessage = $"Left wall angled too much (Y: {leftHit.normal.y:F2})";
                return false;
            }

            wallNormal = leftHit.normal;
            runDirection = Vector3.Cross(wallNormal, Vector3.up).normalized;
            isWallRight = false;
            return true;
        }

        return false;
    }

    public bool CheckForWall(Transform playerTransform)
    {
        return DetectWall(playerTransform);
    }

    // ===========================================================
    // === STATE CONTROL =========================================
    // ===========================================================
    public void StartWallRun()
    {
        if (isWallRunning) return;

        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        player.Animator.applyRootMotion = true;

        Debug.Log($"<color=green>WALL RUN STARTED</color> ({(isWallRight ? "RIGHT" : "LEFT")})");
    }

    public void StopWallRun()
    {
        if (!isWallRunning) return;

        isWallRunning = false;
        player.Animator.applyRootMotion = false;

        Debug.Log("<color=red>WALL RUN STOPPED</color>");
    }

    public bool UpdateTimer()
    {
        wallRunTimer -= Time.deltaTime;
        return wallRunTimer > 0f;
    }

    // ===========================================================
    // === MOVEMENT EXECUTION ====================================
    // ===========================================================
    public void ApplyWallRunMovement(Rigidbody rb)
    {
        if (!isWallRunning) return;

        // Root motion active → skip manual velocity if animation drives motion
        if (player.Animator.applyRootMotion) return;

        // Keep player glued to wall while moving forward
        Vector3 velocity = runDirection * wallRunSpeed;
        velocity.y = -wallRunGravity;
        rb.linearVelocity = velocity;
    }

    // ===========================================================
    // === DEBUG VISUALIZATION ===================================
    // ===========================================================
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Transform t = transform;
        Vector3 origin = t.position + Vector3.up * wallCheckHeight;

        // Right ray
        Gizmos.color = lastRightHit ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + t.right * wallCheckDistance);

        // Left ray
        Gizmos.color = lastLeftHit ? Color.green : Color.red;
        Gizmos.DrawLine(origin, origin + -t.right * wallCheckDistance);

        if (lastRightHit || lastLeftHit)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lastHitPoint, 0.1f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(lastHitPoint, lastHitPoint + lastHitNormal * 0.5f);
        }

        if (isWallRunning)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(t.position, t.position + runDirection * 2f);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(t.position, t.position + wallNormal * 1.5f);
        }
    }

    private void OnGUI()
    {
        if (!showDebugRays) return;

        GUIStyle style = new GUIStyle
        {
            fontSize = 14,
            normal = { textColor = Color.white },
            alignment = TextAnchor.UpperLeft
        };

        string debugText =
            "=== WALL RUN DEBUG ===\n" +
            $"Speed: {currentSpeed:F2} (min {minSpeedToStart})\n" +
            $"Right Hit: {lastRightHit}\n" +
            $"Left Hit: {lastLeftHit}\n" +
            $"Wall Running: {isWallRunning}\n" +
            $"Timer: {wallRunTimer:F2}s\n" +
            $"{debugMessage}";

        GUI.Box(new Rect(10, 10, 350, 180), "");
        GUI.Label(new Rect(15, 15, 340, 170), debugText, style);
    }
}
