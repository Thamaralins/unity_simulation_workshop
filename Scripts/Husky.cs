using UnityEngine;

public class Husky : MonoBehaviour
{

  public float wheelRadius = 0.1651f;
  public float wheelSeparation = 0.512f;

  public float minVelocityLinearX = -1.0f;
  public float maxVelocityLinearX = 1.0f;
  public float minVelocityAngularZ = -3.14f;
  public float maxVelocityAngularZ = 3.14f;

  public GameObject huskyRobot;

  private ArticulationBody[] rightWheels;
  private ArticulationBody[] leftWheels;
  
  private float linVel;
  private float angVel;
  private float rightVelocity;
  private float leftVelocity;

  void Start()
  {
    rightWheels = new ArticulationBody[2];
    leftWheels = new ArticulationBody[2];

    rightWheels[0] = huskyRobot.transform.Find("world/base_link/front_right_wheel").GetComponent<ArticulationBody>();
    rightWheels[1] = huskyRobot.transform.Find("world/base_link/rear_right_wheel").GetComponent<ArticulationBody>();
    leftWheels[0] = huskyRobot.transform.Find("world/base_link/front_left_wheel").GetComponent<ArticulationBody>();
    leftWheels[1] = huskyRobot.transform.Find("world/base_link/rear_left_wheel").GetComponent<ArticulationBody>();

    ChangeDrivesTypeToVelocity();

  }

  private void FixedUpdate()
  {
    linVel = 0.0f;
    angVel = 0.0f;

    if (Input.GetKey(KeyCode.W))
      linVel = maxVelocityLinearX;
    else if (Input.GetKey(KeyCode.S))
      linVel = minVelocityLinearX;
    
    if (Input.GetKey(KeyCode.A))
      angVel = maxVelocityAngularZ;
    else if (Input.GetKey(KeyCode.D))
      angVel = minVelocityAngularZ;

    rightVelocity = (linVel + angVel * wheelSeparation / 2.0f) / wheelRadius;
    leftVelocity = (linVel - angVel * wheelSeparation / 2.0f) / wheelRadius;

    SetJointVelocities(rightVelocity, leftVelocity);
  }

  private void ChangeDrivesTypeToVelocity()
  {
    var frontRightDrive = rightWheels[0].xDrive;
    var rearRightDrive = rightWheels[1].xDrive;
    var frontLeftDrive = leftWheels[0].xDrive;
    var rearLeftDrive = leftWheels[1].xDrive;

    frontRightDrive.driveType = ArticulationDriveType.Velocity;
    rearRightDrive.driveType = ArticulationDriveType.Velocity;
    frontLeftDrive.driveType = ArticulationDriveType.Velocity;
    rearLeftDrive.driveType = ArticulationDriveType.Velocity;

    rightWheels[0].xDrive = frontRightDrive;
    rightWheels[1].xDrive = rearRightDrive;
    leftWheels[0].xDrive = frontLeftDrive;
    leftWheels[1].xDrive = rearLeftDrive;
  }

  private void SetJointVelocities(float rightVel, float leftVel)
  {
    var frontRightDrive = rightWheels[0].xDrive;
    var rearRightDrive = rightWheels[1].xDrive;
    var frontLeftDrive = leftWheels[0].xDrive;
    var rearLeftDrive = leftWheels[1].xDrive;

    frontRightDrive.targetVelocity = rightVel * Mathf.Rad2Deg;
    rearRightDrive.targetVelocity = rightVel * Mathf.Rad2Deg;
    frontLeftDrive.targetVelocity = leftVel * Mathf.Rad2Deg;
    rearLeftDrive.targetVelocity = leftVel * Mathf.Rad2Deg;

    rightWheels[0].xDrive = frontRightDrive;
    rightWheels[1].xDrive = rearRightDrive;
    leftWheels[0].xDrive = frontLeftDrive;
    leftWheels[1].xDrive = rearLeftDrive;
  }

}
