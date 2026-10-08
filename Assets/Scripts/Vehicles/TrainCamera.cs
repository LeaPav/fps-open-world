using UnityEngine;

public class TrainCamera : MonoBehaviour
{
    [SerializeField] private Transform[] _cameraPositionTransforms;
    [SerializeField] private float _timeTravelCamera = 5f;
    [SerializeField] private float _waitDuration = 3f;
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private float _lookFollowSpeed = 3;
    [SerializeField] private AnimationCurve _animationCurve;

    private float _travelProgress;

    private float _waitTimer;
    private bool _isWaiting;



    private int _fromIndex;
    private int _toIndex;

    private void Awake()
    {
        _fromIndex = 0;
        _toIndex = 1;
        enabled = false;
    }

    private void LateUpdate()
    {
        

        if(_isWaiting)
        {
            _waitTimer += Time.deltaTime;

            if (_waitTimer >= _waitDuration)
            {
                _fromIndex = _toIndex;
                _toIndex = (_toIndex + 1) % _cameraPositionTransforms.Length;

                _travelProgress = 0f;
                _waitTimer = 0f;
                _isWaiting = false;
            }
        }
        else
        {
            _travelProgress += Time.deltaTime / _timeTravelCamera;
            if (_travelProgress >= 1f)
            {
                _travelProgress = 1f;
                _isWaiting = true;
            }
        }

        float curveProgress = _animationCurve.Evaluate(_travelProgress);
        transform.position = Vector3.Lerp(_cameraPositionTransforms[_fromIndex].position,
            _cameraPositionTransforms[_toIndex].position, curveProgress);

        Quaternion targetRotation = Quaternion.LookRotation(_targetTransform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _lookFollowSpeed * Time.deltaTime);

    }

}
