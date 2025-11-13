using System.Numerics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using Vector3 = UnityEngine.Vector3;

public class EnvironmentInteractionContext
{
    private TwoBoneIKConstraint _leftFootIkConstraint;
    private TwoBoneIKConstraint _rightFootIkConstraint;
    private MultiRotationConstraint _leftFootMultiRotationConstraint;
    private MultiRotationConstraint _rightFootMultiRotationConstraint;
    private TwoBoneIKConstraint _leftHandIkConstraint;
    private TwoBoneIKConstraint _rightHandIkConstraint;
    private MultiRotationConstraint _leftHandMultiRotationConstraint;
    private MultiRotationConstraint _rightHandMultiRotationConstraint;
    private Rigidbody _rigidbody;
    private CapsuleCollider _rootCollider;
    private LayerMask _interactableMask;
    private Transform _rootTransform;

    public enum EBodySide
    {
        LEFT,
        RIGHT
    }

    public EnvironmentInteractionContext(
        int interactableMask,
        TwoBoneIKConstraint leftFootIkConstraint, TwoBoneIKConstraint rightFootIkConstraint,
        MultiRotationConstraint leftFootMultiRotationConstraint,
        MultiRotationConstraint rightFootMultiRotationConstraint, TwoBoneIKConstraint leftHandIkConstraint,
        TwoBoneIKConstraint rightHandIkConstraint, MultiRotationConstraint leftHandMultiRotationConstraint,
        MultiRotationConstraint rightHandMultiRotationConstraint, Rigidbody rigidbody,
        CapsuleCollider rootCollider,
        Transform rootTransform)
    {
        _interactableMask = interactableMask;
        _leftFootIkConstraint = leftFootIkConstraint;
        _rightFootIkConstraint = rightFootIkConstraint;
        _leftFootMultiRotationConstraint = leftFootMultiRotationConstraint;
        _rightFootMultiRotationConstraint = rightFootMultiRotationConstraint;
        _leftHandIkConstraint = leftHandIkConstraint;
        _rightHandIkConstraint = rightHandIkConstraint;
        _leftHandMultiRotationConstraint = leftHandMultiRotationConstraint;
        _rightHandMultiRotationConstraint = rightHandMultiRotationConstraint;
        _rigidbody = rigidbody;
        _rootCollider = rootCollider;
        _rootTransform = rootTransform;
        
        CharacterShoulderHeight = leftHandIkConstraint.data.root.transform.position.y;
    }

    public int InteractionLayer => _interactableMask;
    public float CharacterShoulderHeight { get; set; }
    public TwoBoneIKConstraint LeftFootIkConstraint => _leftFootIkConstraint;
    public TwoBoneIKConstraint RightFootIkConstraint => _rightFootIkConstraint;
    public MultiRotationConstraint LeftFootMultiRotationConstraint => _leftFootMultiRotationConstraint;
    public MultiRotationConstraint RightFootMultiRotationConstraint => _rightFootMultiRotationConstraint;
    public TwoBoneIKConstraint LeftHandIkConstraint => _leftHandIkConstraint;
    public TwoBoneIKConstraint RightHandIkConstraint => _rightHandIkConstraint;
    public MultiRotationConstraint LeftHandMultiRotationConstraint => _leftHandMultiRotationConstraint;
    public MultiRotationConstraint RightHandMultiRotationConstraint => _rightHandMultiRotationConstraint;
    public Rigidbody Rigidbody => _rigidbody;
    public CapsuleCollider RootCollider => _rootCollider;
    public Transform RootTransform => _rootTransform;

    public TwoBoneIKConstraint CurrentIkConstraint { get; private set; }
    public MultiRotationConstraint CurrentMultiRotationConstraint { get; private set; }
    public Transform CurrentIkTargetTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }
    public Collider CurrentIntersectingCollider { get; set; }
    public Vector3 ClosestPointOnColliderFromShoulder { get; set; } = Vector3.positiveInfinity;

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftHandIkConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightHandIkConstraint.data.root.transform.position;
        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) <
                            Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            CurrentBodySide = EBodySide.LEFT;
            CurrentIkConstraint = _leftHandIkConstraint;
            CurrentMultiRotationConstraint = _leftHandMultiRotationConstraint;
        }
        else
        {
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIkConstraint = _rightHandIkConstraint;
            CurrentMultiRotationConstraint = _rightHandMultiRotationConstraint;
        }

        CurrentIkTargetTransform = CurrentIkConstraint.data.root.transform;
        CurrentShoulderTransform = CurrentIkConstraint.data.target.transform;
        Debug.Log(CurrentBodySide);
    }
}