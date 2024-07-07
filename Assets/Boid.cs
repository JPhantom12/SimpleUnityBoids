using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Boid : MonoBehaviour
{
    [SerializeField] private List<Boid> _allBoids = new List<Boid>();

    private List<Boid> _localFlockmates = new List<Boid>();

    private bool _running;

    private float _safeDistance, _avoidFactor, _matchingFactor, _centeringFactor, _visibleDistance, _maxSpeed, _edgeTurningFactor, _edgeDistanceX, _edgeDistanceY, _obstacleAvoidanceFactor, _minSpeed;

    public void SetUp(BoidsManager.BoidConfig config, List<Boid> allBoids)
    {
        _safeDistance = config.SafeDistance;
        _avoidFactor = config.AvoidFactor;
        _matchingFactor = config.MatchingFactor;
        _centeringFactor = config.CenteringFactor;
        _visibleDistance = config.VisibleDistance;
        _allBoids = allBoids.ToList();
        _maxSpeed = config.MaxSpeed;
        _edgeTurningFactor = config.EdgeTurningFactor;
        _edgeDistanceX = config.EdgeDistanceX;
        _edgeDistanceY = config.EdgeDistanceY;
        _obstacleAvoidanceFactor = config.ObstacleAvoidanceFactor;
        _minSpeed=config.MinSpeed;
        _running = true;
    }
    
    public void FixedUpdate()
    {
        if (!_running)
            return;
        
        Sense();
        Steer();
        Velocity = Vector3.ClampMagnitude(Velocity, _maxSpeed);
        if(Velocity.magnitude < _minSpeed)
        {
            if(Velocity.magnitude == 0)
                Velocity = Vector2.up * _minSpeed;
            else
                Velocity = Velocity.normalized * _minSpeed;
        }
        transform.position += Velocity;
    }

    private void Sense()
    {
        _localFlockmates.Clear();
        foreach (Boid boid in _allBoids)
        {
            if(Mathf.Abs((transform.position - boid.transform.position).magnitude)<=_visibleDistance)
                _localFlockmates.Add(boid);
        }
    }

    private void Steer()
    {
        Avoid();
        Align();
        Cohesion();
        AvoidEdge();
        AvoidObstacle();
    }

    private void AvoidObstacle()
    {
        if (!Input.GetMouseButton(0))
            return;

        Vector3 avoid = (transform.position - BoidsManager.Instance.MousePosition);

        if (avoid.magnitude > 150)
            return;
        
        float avoidanceForce = (1-Mathf.Clamp01(Mathf.Abs(avoid.magnitude) / 400)) * _obstacleAvoidanceFactor;
        
        avoid.Normalize();

        Velocity += avoid * avoidanceForce;
    }

    private void AvoidEdge()
    {
        Vector3 edgeTurningForce = Vector3.zero;
        if (transform.position.x > _edgeDistanceX)
        {
            edgeTurningForce.x = -(transform.position.x - _edgeDistanceX);
        }

        if (transform.position.x < -_edgeDistanceX)
        {
            edgeTurningForce.x = -(transform.position.x + _edgeDistanceX);
        }

        if (transform.position.y > _edgeDistanceY)
        {
            edgeTurningForce.y = -(transform.position.y - _edgeDistanceY);
        }

        if (transform.position.y < -_edgeDistanceY)
        {
            edgeTurningForce.y = -(transform.position.y + _edgeDistanceY);
        }

        Velocity += edgeTurningForce * _edgeTurningFactor;
    }

    public Vector3 Velocity;

    private void Avoid()
    {
        Vector3 awayForce = Vector3.zero;
        foreach (Boid boid in _localFlockmates)
        {
            if(Vector2.Distance(transform.position, boid.transform.position) > _safeDistance)
                continue;

            awayForce += (transform.position - boid.transform.position);
        }

        Velocity += awayForce * _avoidFactor;
    }

    private void Align()
    {
        Vector3 averageVelocity = Vector3.zero;

        foreach (Boid boid in _localFlockmates)
        {
            averageVelocity += boid.Velocity;
        }

        if(_localFlockmates.Count > 0)
            averageVelocity /= _localFlockmates.Count;

        Velocity += _matchingFactor * averageVelocity;
    }

    private void Cohesion()
    {
        Vector3 averagePosition = Vector3.zero;

        foreach (Boid boid in _localFlockmates)
        {
            averagePosition += boid.transform.position;
        }

        if (_localFlockmates.Count > 0)
            averagePosition /= _localFlockmates.Count;

        Velocity += (averagePosition - transform.position) * _centeringFactor;
    }
}
