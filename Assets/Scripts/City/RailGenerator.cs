using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class RailGenerator : MonoBehaviour
{

    [SerializeField] private SplineContainer _splineContainer;
    [SerializeField] private GameObject _railPrefab;
    [SerializeField] private float _railLength = 5f;

    void Start()
    {
        if(_splineContainer == null || _railPrefab == null)
        {
            Debug.LogError("RailGenerator : spline or prefab not found");
            return;
        }
        float trackLength = _splineContainer.CalculateLength();
        int railCount = Mathf.FloorToInt(trackLength / _railLength);

        for(int i = 0; i < railCount; i++)
        {
            float distance = i * _railLength;
            float t = distance / trackLength;

            _splineContainer.Evaluate(t, out float3 position, out float3 tangent, out  float3 upVector);

            Quaternion rotation = Quaternion.LookRotation(tangent, upVector);
            Instantiate(_railPrefab, position, rotation, transform);
        }
    }
}
