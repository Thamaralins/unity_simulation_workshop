using System;
using UnityEngine;

public class OdomImu : MonoBehaviour
{
  public GameObject robot;
  private ArticulationBody robotBody;

  public Vector3 position;
  public Quaternion orientation;
  public Vector3 linearVelocity;
  public Vector3 angularVelocity;
  public Vector3 linearAcceleration;
  public Vector3 lastLinearVelocity;


  void Start()
  {
    robotBody = robot.transform.Find("world/base_link").GetComponent<ArticulationBody>();
  }

  void FixedUpdate()
  {
    position = robotBody.transform.position;
    orientation = robotBody.transform.rotation;

    linearVelocity = robotBody.velocity;
    angularVelocity = robotBody.angularVelocity;

    linearAcceleration = (linearVelocity - lastLinearVelocity) / Time.fixedDeltaTime;

    lastLinearVelocity = robotBody.velocity;
    
  }

}
