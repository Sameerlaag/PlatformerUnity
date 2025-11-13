using UnityEngine;
using UnityEngine.Animations.Rigging;

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
    }

    public int InteractionLayer => _interactableMask;

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
    public Transform CurrentIkTransform { get; private set; }
    public Transform CurrentShoulderTransform { get; private set; }
    public EBodySide CurrentBodySide { get; private set; }

    public void SetCurrentSide(Vector3 positionToCheck)
    {
        Vector3 leftShoulder = _leftHandIkConstraint.data.root.transform.position;
        Vector3 rightShoulder = _rightHandIkConstraint.data.root.transform.position;
        bool isLeftCloser = Vector3.Distance(positionToCheck, leftShoulder) <
                            Vector3.Distance(positionToCheck, rightShoulder);
        if (isLeftCloser)
        {
            CurrentBodySide = EBodySide.LEFT;
            CurrentIkConstraint = _leftFootIkConstraint;
            CurrentMultiRotationConstraint = _leftFootMultiRotationConstraint;
        }
        else
        {
            CurrentBodySide = EBodySide.RIGHT;
            CurrentIkConstraint = _rightFootIkConstraint;
            CurrentMultiRotationConstraint = _rightFootMultiRotationConstraint;
        }

        CurrentIkTransform = CurrentIkConstraint.data.root.transform;
        CurrentShoulderTransform = CurrentIkConstraint.data.target.transform;
        Debug.Log(CurrentBodySide);
    }
}