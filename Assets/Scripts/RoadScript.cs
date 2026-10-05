using UnityEngine;

public class RoadScript : MonoBehaviour
{

    [SerializeField] GameObject _firstLine;

    [SerializeField] int _numberOfLines = 100;

    [SerializeField] Vector3 _spawnDistance = new Vector3(0, 0, 10);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 lastPosition = _firstLine.transform.position;

        for (int i = 0;  i < _numberOfLines; i++)
        {
            Vector3 spawnPoint = lastPosition + _spawnDistance;

            GameObject gameObject = Instantiate(_firstLine, spawnPoint, Quaternion.identity,transform);

            lastPosition = gameObject.transform.position;
        }

    }

}
