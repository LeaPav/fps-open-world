using UnityEditor.AssetImporters;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class CityGenerator : MonoBehaviour
{

    [Header("Grid")]
    [SerializeField] private int _gridSize = 6;
    [SerializeField] private float _blockSize = 60f;
    [SerializeField] private float _roadWidth = 10f;
    [SerializeField] private int _slotsPerSide = 3;

    [Header("Prefabs")]
    [SerializeField] private GameObject _roadPrefab;
    [SerializeField] private GameObject _linePrefab;
    [SerializeField] private GameObject[] _housePrefabs;
    [SerializeField] private GameObject[] _towerPrefabs;

    [Header("Road lines")]
    [SerializeField] private float _dashSpacing = 6f;

    [Header("Districts")]
    [SerializeField] private float _noiseScale = 0.35f;
    [SerializeField] private float _towerThreshold = 0.5f;
    [SerializeField] private int _seed = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(_roadPrefab == null || _linePrefab == null || _housePrefabs.Length == 0 || _towerPrefabs.Length == 0)
        {
            Debug.LogError("a prefab is missing in the inspector");
            return;
        }

        Random.InitState(_seed);
        float noiseOffset = Random.Range(0f, 1000f);
        float pitch = _blockSize + _roadWidth;

        GenerateRoads(pitch);
        GenerateLines(pitch);
        GenerateBlocks(pitch, noiseOffset);
    }

    private void GenerateRoads(float pitch)
    {
        Vector3 origin = transform.position;
        float totalLength = _gridSize * pitch;

        float roadLength = totalLength + _roadWidth;
        float center = totalLength / 2f - _roadWidth / 2f;
        float roadHeight = _roadPrefab.transform.localScale.y;

        for(int i=0; i <=_gridSize; i++)
        {
            float line = i * pitch - _roadWidth / 2f;

            GameObject roadX = Instantiate(_roadPrefab, origin + new Vector3(center, 0f, line), Quaternion.identity, transform);
            roadX.transform.localScale = new Vector3(roadLength, roadHeight, _roadWidth);

            GameObject roadZ = Instantiate(_roadPrefab, origin + new Vector3(line, 0f, center), Quaternion.identity, transform);
            roadZ.transform.localScale = new Vector3(_roadWidth, roadHeight, roadLength);
        }
    }

    private void GenerateLines(float pitch)
    {
        Vector3 origin = transform.position;

        for(int i = 0; i <= _gridSize; i++)
        {
            float lineCoord = i * pitch - _roadWidth / 2f;

            for (int b = 0; b < _gridSize; b++)
            {
                for(float d = _dashSpacing / 2f; d < _blockSize; d+= _dashSpacing)
                {
                    float along = b * pitch + d;

                    Instantiate(_linePrefab, origin + new Vector3(along, 0.06f, lineCoord), Quaternion.identity, transform);
                    Instantiate(_linePrefab, origin + new Vector3(lineCoord, 0.06f, along), Quaternion.Euler(0f, 90f, 0f), transform);
                }
            }
        }
    }

    private void GenerateBlocks(float pitch, float noiseOffset)
    {
        Vector3 origin = transform.position;
        float slotSize = _blockSize / _slotsPerSide;

        for (int x = 0; x < _gridSize; x++)
        {
            for (int z = 0; z < _gridSize; z++)
            {
                float noise = Mathf.PerlinNoise(x * _noiseScale + noiseOffset, z * _noiseScale + noiseOffset);
                GameObject[] prefabs = noise > _towerThreshold ? _towerPrefabs : _housePrefabs;

                Vector3 blockOrigin = origin + new Vector3(x * pitch, 0f, z * pitch);

                for (int sx = 0; sx < _slotsPerSide;  sx++)
                {
                    for (int sz = 0; sz < _slotsPerSide; sz++)
                    {
                        Vector3 slotCenter = blockOrigin + new Vector3((sx + 0.5f) * slotSize, 0f, (sz+0.5f) * slotSize);
                        GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
                        Quaternion rotation = Quaternion.Euler(0f, Random.Range(0, 4) * 90f, 0f);

                        Instantiate(prefab, slotCenter, rotation, transform);
                    }
                }
            }
        }
    }
}
