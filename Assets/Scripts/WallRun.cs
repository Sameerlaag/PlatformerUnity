using UnityEngine;

public class WallRun : MonoBehaviour
{
    [Header("Wall Run Settings")]
    public float wallRunSpeed = 8f;
    public float wallRunDuration = 1.2f; // time instead of distance
    public float wallCheckDistance = 0.6f;
    public float minJumpHeight = 1.5f;
    public LayerMask wallMask;

    [Header("Jump Settings")]
    public float wallJumpUpForce = 7f;
    public float wallJumpSideForce = 6f;

    private Rigidbody rb;
    private bool isWallRunning;
    private float wallRunTimer;
    private Vector3 wallNormal; // which way the wall is facing
    private Vector3 runDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public bool CanStartWallRun(Transform playerTransform)
    {
        if (Physics.Raycast(playerTransform.position, playerTransform.right, out RaycastHit rightHit, wallCheckDistance, wallMask))
        {
            wallNormal = rightHit.normal;
            runDirection = Vector3.Cross(wallNormal, Vector3.up); // direction along wall
            return true;
        }
        if (Physics.Raycast(playerTransform.position, -playerTransform.right, out RaycastHit leftHit, wallCheckDistance, wallMask))
        {
            wallNormal = leftHit.normal;
            runDirection = Vector3.Cross(Vector3.up, wallNormal); // other side
            return true;
        }

        return false;
    }

    public void EnterWallRun()
    {
        Debug.Log("We wallrunning baby");

        isWallRunning = true;
        wallRunTimer = wallRunDuration;
        rb.useGravity = false;
    }

    public void UpdateWallRun()
    {
        if (!isWallRunning) return;

        wallRunTimer -= Time.deltaTime;
        if (wallRunTimer <= 0)
        {
            ExitWallRun();
            return;
        }

        rb.linearVelocity = runDirection.normalized * wallRunSpeed;
    }

    public void ExitWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
        Debug.Log("WallRun ended");

    }

    public void WallJump()
    {
        if (!isWallRunning) return;

        Vector3 jumpDir = Vector3.up * wallJumpUpForce + wallNormal * wallJumpSideForce;
        rb.linearVelocity = Vector3.zero;
        rb.AddForce(jumpDir, ForceMode.Impulse);

        ExitWallRun();
    }

    public bool IsWallRunning => isWallRunning;
}
