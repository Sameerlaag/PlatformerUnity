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


    public EnvironmentInteractionContext(
        TwoBoneIKConstraint leftFootIkConstraint, TwoBoneIKConstraint rightFootIkConstraint,
        MultiRotationConstraint leftFootMultiRotationConstraint,
        MultiRotationConstraint rightFootMultiRotationConstraint, TwoBoneIKConstraint leftHandIkConstraint,
        TwoBoneIKConstraint rightHandIkConstraint, MultiRotationConstraint leftHandMultiRotationConstraint,
        MultiRotationConstraint rightHandMultiRotationConstraint, Rigidbody rigidbody,
        CapsuleCollider rootCollider)
    {
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
    }

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
}