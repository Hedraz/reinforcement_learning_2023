using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class MyAgent : Agent
{
    public float speed = 7.0f;
    [SerializeField] private Transform targetTransform;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = Vector3.zero;

        float xPos = Random.Range(-22, 22);
        float zPos = Random.Range(-22, 22);

        targetTransform.localPosition = new Vector3(xPos, 0f, zPos);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        var actionTaken = actions.ContinuousActions;

        float actionSpeed = (actionTaken[0] + 1) / 2;
        float actionSteering = actionTaken[1];

        transform.Translate(actionSpeed * Vector3.forward * speed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(new Vector3(0, actionSteering * 180, 0));

        AddReward(-0.01f);
    }

public override void Heuristic(in ActionBuffers actionsOut) {
        ActionSegment<float> actions = actionsOut.ContinuousActions;

        actions[0] = -1;
        actions[1] = 0;

        if (Input.GetKey("w"))
            actions[0] = 1;
        
        if (Input.GetKey("d"))
            actions[1] = +0.5f;

        if (Input.GetKey("a"))
            actions[1] = -0.5f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Wall")
        {
            AddReward(-1);
            EndEpisode();
        }
        else if (collision.collider.tag == "Vent")
        {
            AddReward(1);
            EndEpisode();
        }
    }
}
