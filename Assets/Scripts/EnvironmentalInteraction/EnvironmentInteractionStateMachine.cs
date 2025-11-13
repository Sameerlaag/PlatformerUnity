using EnvironmentalInteraction;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Assertions;

public class
    EnvironmentInteractionStateMachine : StateManager<
    EnvironmentInteractionStateMachine.EEnvironementInteractionState>
{
    public enum EEnvironementInteractionState
    {
        Search,
        Approach,
        Start,
        Run,
        Stop,
        Jump,
        Reset
    }

    [SerializeField] private TwoBoneIKConstraint leftFootIkConstraint;
    [SerializeField] private TwoBoneIKConstraint rightFootIkConstraint;
    [SerializeField] private MultiRotationConstraint leftFootMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint rightFootMultiRotationConstraint;
    [SerializeField] private TwoBoneIKConstraint leftHandIkConstraint;
    [SerializeField] private TwoBoneIKConstraint rightHandIkConstraint;
    [SerializeField] private MultiRotationConstraint leftHandMultiRotationConstraint;
    [SerializeField] private MultiRotationConstraint rightHandMultiRotationConstraint;
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private CapsuleCollider rootCollider;

    private EnvironmentInteractionContext _context;
    private int interactableMask;

    private void Awake()
    {
        _context = new EnvironmentInteractionContext(
            LayerMask.NameToLayer("Runwall"),
            leftFootIkConstraint,
            rightFootIkConstraint,
            leftFootMultiRotationConstraint,
            rightFootMultiRotationConstraint,
            leftHandIkConstraint,
            rightHandIkConstraint,
            leftHandMultiRotationConstraint,
            rightHandMultiRotationConstraint,
            rigidbody,
            rootCollider,
            transform.root
        );
        InitializeStates();
        ValidateConstraints();
        ConstructEnvironmentDetectionCollider();
    }

    private void ValidateConstraints()
    {
        Assert.IsNotNull(leftFootIkConstraint, "Left Foot IK constraint is not assigned.");
        Assert.IsNotNull(rightFootIkConstraint, "Right Foot IK constraint is not assigned.");
        Assert.IsNotNull(leftFootMultiRotationConstraint, "Left Foot rotation constraint is not assigned.");
        Assert.IsNotNull(rightFootMultiRotationConstraint, "Right Foot rotation constraint is not assigned.");
        Assert.IsNotNull(leftHandIkConstraint, "Left Hand IK constraint is not assigned.");
        Assert.IsNotNull(rightHandIkConstraint, "Right Hand IK constraint is not assigned.");
        Assert.IsNotNull(leftHandMultiRotationConstraint, "Left Hand rotation constraint is not assigned.");
        Assert.IsNotNull(rightHandMultiRotationConstraint, "Right Hand rotation constraint is not assigned.");
        Assert.IsNotNull(rigidbody, "Rigidbody is not assigned.");
        Assert.IsNotNull(rootCollider, "RootCollider is not assigned.");
    }

    private void ConstructEnvironmentDetectionCollider()
    {
        float wingspan = rootCollider.height;

        BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
        boxCollider.size = new Vector3(wingspan, wingspan, wingspan);
        boxCollider.center = new Vector3(rootCollider.center.x, rootCollider.center.y + (.1f * wingspan),
            rootCollider.center.z + (.35f * wingspan));
        boxCollider.isTrigger = true;
    }

    private void InitializeStates()
    {
        States.Add(EEnvironementInteractionState.Search,
            new SearchState(_context, EEnvironementInteractionState.Search));
        States.Add(EEnvironementInteractionState.Reset, new ResetState(_context, EEnvironementInteractionState.Reset));
        States.Add(EEnvironementInteractionState.Approach,
            new ApproachState(_context, EEnvironementInteractionState.Approach));
        States.Add(EEnvironementInteractionState.Start, new StartState(_context, EEnvironementInteractionState.Start));
        States.Add(EEnvironementInteractionState.Run, new RunState(_context, EEnvironementInteractionState.Run));
        States.Add(EEnvironementInteractionState.Stop, new StopState(_context, EEnvironementInteractionState.Stop));
        States.Add(EEnvironementInteractionState.Jump, new JumpState(_context, EEnvironementInteractionState.Jump));
        CurrentState = States[EEnvironementInteractionState.Search];
        Debug.Log(CurrentState);
    }
}