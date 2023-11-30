using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MyAgent : Agent
{
    public float speed = 5.0f;
    [SerializeField] private Transform targetTransform;
    public Transform[] spawnPoints;
    public Transform Vent, Impostor;
    private Quaternion originalRotation;
    public float rotationSpeed = 45.0f;

    public override void OnEpisodeBegin()
    {
        int agentIndex;
        int ventIndex;

        do
        {
            agentIndex = Random.Range(0, spawnPoints.Length);
            ventIndex = Random.Range(0, spawnPoints.Length);
        } while (agentIndex == ventIndex);

        Transform agentSpawnPoint = spawnPoints[agentIndex];
        Transform ventSpawnPoint = spawnPoints[ventIndex];

        Impostor.localPosition = agentSpawnPoint.localPosition;
        Vent.localPosition = ventSpawnPoint.localPosition;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition- targetTransform.localPosition);
        
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        originalRotation = transform.rotation;

        transform.localPosition += new Vector3(moveX, 0, moveZ) * speed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Slerp(transform.rotation, originalRotation, Time.deltaTime * rotationSpeed);
    }

public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            AddReward(-100);
            EndEpisode();
        }
        else if (collision.collider.tag == "Vent")
        {
            AddReward(+100);
            EndEpisode();
        }
    }
}
