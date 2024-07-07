using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsManager : MonoBehaviour
{
    public static BoidsManager Instance;

    private void Awake() => Instance = this;
    
    [SerializeField] private List<Boid> _boids = new List<Boid>();

    [SerializeField] private int _numberOfBoids;

    [SerializeField] private GameObject _boidPrefab;

    [SerializeField] private float _xSize, _ySize;
    
    [System.Serializable]
    public struct BoidConfig
    {
        public float SafeDistance, AvoidFactor, MatchingFactor, CenteringFactor, VisibleDistance, MaxSpeed, EdgeTurningFactor, EdgeDistanceX, EdgeDistanceY, ObstacleAvoidanceFactor, MinSpeed;
    }


    public BoidConfig config;
    
    private void Start()
    {
        for (int i = 0; i < _numberOfBoids; i++)
        {
            _boids.Add(Instantiate(_boidPrefab, new Vector3(Random.Range(-_xSize, _xSize), Random.Range(-_ySize,_ySize), 0), Quaternion.identity).GetComponent<Boid>());
        }
        
        foreach (Boid boid in _boids)
        {
            boid.SetUp(config, _boids);
        }
    }

    public Vector3 MousePosition;

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            MousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MousePosition.z = 0;
        }
    }
}
