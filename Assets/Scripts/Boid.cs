﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Boid : MonoBehaviour
{
    public Vector3 baseRotation;

    [Range(0, 10)]
    public float maxSpeed = 1f;

    [Range(.01f, .5f)]
    public float maxForce = .03f;

    [Range(1, 10)]
    public float neighborhoodRadius = 3f;

    [Range(0, 3)]
    public float separationAmount = 1f;

    [Range(0, 3)]
    public float cohesionAmount = 1f;

    [Range(0, 3)]
    public float alignmentAmount = 1f;

    public Vector2 acceleration;
    public Vector2 velocity;

    public Slider MaxSpeedSlider;
    public Slider MaxForceSlider;
    public Slider NeighborhoodRadiusSlider;
    public Slider SeparationSlider;
    public Slider CohesionSlider;
    public Slider AlignmentSlider;

    private Vector2 Position
    {
        get
        {
            return gameObject.transform.position;
        }
        set
        {
            gameObject.transform.position = value;
        }
    }

    private void Start()
    {
        MaxSpeedSlider = (Slider)GameObject.Find("MaxSpeedSlider").GetComponent<Slider>();
        MaxForceSlider = (Slider)GameObject.Find("MaxForceSlider").GetComponent<Slider>();
        NeighborhoodRadiusSlider = (Slider)GameObject.Find("NeighborhoodRadiusSlider").GetComponent<Slider>();
        SeparationSlider = (Slider)GameObject.Find("SeparationSlider").GetComponent<Slider>();
        CohesionSlider = (Slider)GameObject.Find("CohesionSlider").GetComponent<Slider>();
        AlignmentSlider = (Slider)GameObject.Find("AlignmentSlider").GetComponent<Slider>();
        float angle = Random.Range(0, 2 * Mathf.PI);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
        velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }

    private void Update()
    {
        UpdateSliders();
        var boidColliders = Physics2D.OverlapCircleAll(Position, neighborhoodRadius);
        var boids = boidColliders.Select(o => o.GetComponent<Boid>()).ToList();
        boids.Remove(this);

        Flock(boids);
        UpdateVelocity();
        UpdatePosition();
        UpdateRotation();
        WrapAround();
    }

    private void UpdateSliders()
    {
        maxSpeed = MaxSpeedSlider.value;
        maxForce = MaxForceSlider.value;
        neighborhoodRadius = NeighborhoodRadiusSlider.value;
        separationAmount = SeparationSlider.value;
        cohesionAmount = CohesionSlider.value;
        alignmentAmount = AlignmentSlider.value;
    }

    private void Flock(IEnumerable<Boid> boids)
    {
        var alignment = Alignment(boids);
        var separation = Separation(boids);
        var cohesion = Cohesion(boids);

        acceleration = alignmentAmount * alignment + cohesionAmount * cohesion + separationAmount * separation;
    }

    public void UpdateVelocity()
    {
        velocity += acceleration;
        velocity = LimitMagnitude(velocity, maxSpeed);
    }

    private void UpdatePosition()
    {
        Position += velocity * Time.deltaTime;
    }

    private void UpdateRotation()
    {
        var angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle) + baseRotation);
    }

    private Vector2 Alignment(IEnumerable<Boid> boids)
    {
        var velocity = Vector2.zero;
        if (!boids.Any()) return velocity;

        foreach (var boid in boids)
        {
            velocity += boid.velocity;
        }
        velocity /= boids.Count();

        var steer = Steer(velocity.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Cohesion(IEnumerable<Boid> boids)
    {
        if (!boids.Any()) return Vector2.zero;

        var sumPositions = Vector2.zero;
        foreach (var boid in boids)
        {
            sumPositions += boid.Position;
        }
        var average = sumPositions / boids.Count();
        var direction = average - Position;

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Separation(IEnumerable<Boid> boids)
    {
        var direction = Vector2.zero;
        boids = boids.Where(o => DistanceTo(o) <= neighborhoodRadius / 2);
        if (!boids.Any()) return direction;

        foreach (var boid in boids)
        {
            var difference = Position - boid.Position;
            direction += difference.normalized / difference.magnitude;
        }
        direction /= boids.Count();

        var steer = Steer(direction.normalized * maxSpeed);
        return steer;
    }

    private Vector2 Steer(Vector2 desired)
    {
        var steer = desired - velocity;
        steer = LimitMagnitude(steer, maxForce);

        return steer;
    }

    private float DistanceTo(Boid boid)
    {
        return Vector3.Distance(boid.transform.position, Position);
    }

    private Vector2 LimitMagnitude(Vector2 baseVector, float maxMagnitude)
    {
        if (baseVector.sqrMagnitude > maxMagnitude * maxMagnitude)
        {
            baseVector = baseVector.normalized * maxMagnitude;
        }
        return baseVector;
    }

    private void WrapAround()
    {
        if (Position.x < -14) Position = new Vector2(14, Position.y);
        if (Position.y < -8) Position = new Vector2(Position.x, 8);
        if (Position.x > 14) Position = new Vector2(-14, Position.y);
        if (Position.y > 8) Position = new Vector2(Position.x, -8);
    }


}