using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using Wrench = RosMessageTypes.Geometry.WrenchMsg;

public class FlatFishROS : MonoBehaviour
{

  public string topicName = "cmd_force";

  public float FlatFishVolume = 0.35f; // m^3
  public float waterDensity = 1000.0f; // kg/m^3

  public GameObject FlatFishRobot;
  private Rigidbody FlatFishRB;

  private Vector3 inputForce;
  private Vector3 inputTorque;

  void Start()
  {
    ROSConnection.GetOrCreateInstance().Subscribe<Wrench>(topicName, ForceCallback);

    FlatFishRB = FlatFishRobot.GetComponent<Rigidbody>();
  }

  void FixedUpdate()
  {
    float buoyancy = 9.81f * FlatFishVolume * waterDensity;

    Vector3 buoyancyForce = new Vector3(0.0f, buoyancy, 0.0f);
    
    // Weight force is already applied by Rigidbody
    FlatFishRB.AddRelativeForce(buoyancyForce + inputForce);
    FlatFishRB.AddRelativeTorque(inputTorque);
  }

  void ForceCallback(Wrench forceMsg)
  {
    // Convert to Unity coordinate system
    inputForce = forceMsg.force.From<FLU>();
    inputTorque = forceMsg.torque.From<FLU>();
  }

}
