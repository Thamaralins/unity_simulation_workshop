using System;
using UnityEngine;
using RosMessageTypes.Geometry;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using OdometryMsg = RosMessageTypes.Nav.OdometryMsg;
using ImuMsg = RosMessageTypes.Sensor.ImuMsg;

public class OdomImuROS : MonoBehaviour
{
  ROSConnection ros;

  public string odometryTopicName = "odom";
  public string imuTopicName = "imu";

  public GameObject robot;
  private ArticulationBody robotBody;

  private Vector3 position;
  private Quaternion orientation;
  private Vector3 linearVelocity;
  private Vector3 angularVelocity;
  private Vector3 linearAcceleration;
  private Vector3 lastLinearVelocity;

  private OdometryMsg Odometry;
  private ImuMsg Imu;


  void Start()
  {
    ros = ROSConnection.GetOrCreateInstance();
    ros.RegisterPublisher<OdometryMsg>(odometryTopicName);
    ros.RegisterPublisher<ImuMsg>(imuTopicName);

    Imu = new ImuMsg();
    Odometry = new OdometryMsg();

    Imu.header.frame_id = "imu";
    Odometry.header.frame_id = "odom";

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
    
    // Remember to convert to ROS coordinate system
    // Odometry message
    Odometry.pose.pose.position = position.To<FLU>();
    Odometry.pose.pose.orientation = orientation.To<FLU>();
    Odometry.twist.twist.linear = linearVelocity.To<FLU>();
    Odometry.twist.twist.angular = angularVelocity.To<FLU>();

    // Imu message
    Imu.orientation = orientation.To<FLU>();
    Imu.angular_velocity = angularVelocity.To<FLU>();
    Imu.linear_acceleration = linearAcceleration.To<FLU>();

    ros.Publish(odometryTopicName, Odometry);
    ros.Publish(imuTopicName, Imu);

  }

}
