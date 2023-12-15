using UnityEngine;

public class FlatFish : MonoBehaviour
{

  public float FlatFishVolume = 0.35f; // m^3
  public float waterDensity = 1000.0f; // kg/m^3

  public GameObject FlatFishRobot;
  private Rigidbody FlatFishRB;

  public Vector3 inputForce;
  public Vector3 inputTorque;

  void Start()
  {
    FlatFishRB = FlatFishRobot.GetComponent<Rigidbody>();
    
    inputForce = new Vector3(0.0f, 0.0f, 0.0f);
    inputTorque = new Vector3(0.0f, 0.0f, 0.0f);
  }

  void FixedUpdate()
  {
    float buoyancy = 9.81f * FlatFishVolume * waterDensity;

    Vector3 buoyancyForce = new Vector3(0.0f, buoyancy, 0.0f);
    
    if (Input.GetKeyDown(KeyCode.W))
      IncreaseForceX(5.0f);
    if (Input.GetKeyDown(KeyCode.S))
      IncreaseForceX(-5.0f);

    if (Input.GetKeyDown(KeyCode.A))
      IncreaseForceY(5.0f);
    if (Input.GetKeyDown(KeyCode.D))
      IncreaseForceY(-5.0f);

    if (Input.GetKeyDown(KeyCode.Q))
      IncreaseForceZ(5.0f);
    if (Input.GetKeyDown(KeyCode.E))
      IncreaseForceZ(-5.0f);

    if (Input.GetKeyDown(KeyCode.Z))
      IncreaseTorqueZ(5.0f);
    if (Input.GetKeyDown(KeyCode.X))
      IncreaseTorqueZ(-5.0f);
    
    // Weight force is already applied by Rigidbody
    FlatFishRB.AddRelativeForce(buoyancyForce + inputForce);
    FlatFishRB.AddRelativeTorque(inputTorque);
  }

  public void IncreaseForceX(float val)
  {
    inputForce += new Vector3(0.0f, 0.0f, val);
  }

  public void IncreaseForceY(float val)
  {
    inputForce += new Vector3(-val, 0.0f, 0.0f);
  }

  public void IncreaseForceZ(float val)
  {
    inputForce += new Vector3(0.0f, val, 0.0f);
  }

  public void IncreaseTorqueZ(float val)
  {
    inputTorque += new Vector3(0.0f, val, 0.0f);
  }

}
