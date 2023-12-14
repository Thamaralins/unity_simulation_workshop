using System;
using UnityEngine;
using Unity.Robotics.ROSTCPConnector;
using Unity.Robotics.ROSTCPConnector.ROSGeometry;
using RosMessageTypes.Geometry;
using LaserScanMsg = RosMessageTypes.Sensor.LaserScanMsg;

public class LidarROS : MonoBehaviour
{
  ROSConnection ros;

  public string topicName = "scan";
  public string frameId = "lidar";

  public float LidarFOV = 270.0f;
  public int LidarNumOfPoints = 1024;
  public float LidarFrequency = 30.0f;
  public float TraceMaxDistance = 10.0f;
  public float MaxNoiseValue = 0.01f;
  public bool VisualizeLaserLines = false;

  private float Step;
  private float[] Scans;

  private LaserScanMsg LaserScan;

  private float elapsedTime;

  void Start()
  {
    ros = ROSConnection.GetOrCreateInstance();
    ros.RegisterPublisher<LaserScanMsg>(topicName);

    Step = LidarFOV / (LidarNumOfPoints - 1);
    
    LaserScan = new LaserScanMsg();
    LaserScan.header.frame_id = frameId;
    LaserScan.angle_min = -LidarFOV / 2.0f * Mathf.Deg2Rad;
    LaserScan.angle_max = LidarFOV / 2.0f * Mathf.Deg2Rad;
    LaserScan.angle_increment = Step * Mathf.Deg2Rad;
    LaserScan.range_min = 0.0f;
    LaserScan.range_max = TraceMaxDistance;

    Scans = new float[LidarNumOfPoints];
  }

  void Update()
  {
    elapsedTime += Time.deltaTime;

    if (elapsedTime > 1.0f / LidarFrequency)
    {
      publishLaserScan();
      elapsedTime -= 1.0f / LidarFrequency;
    }
  }

  void publishLaserScan()
  {
    LaserScan.ranges = new float[LidarNumOfPoints];

    Vector3 Start = transform.position;

    for (int i = 0; i < LidarNumOfPoints; i++)
    {
      float Angle = -(LidarFOV / 2) + Step * i;

      Vector3 Direction = Quaternion.AngleAxis(Angle, transform.up) * transform.forward;

      bool bHit = Physics.Raycast(Start, Direction, out RaycastHit hit, TraceMaxDistance);

      if (bHit)
      {
        Scans[i] = hit.distance;
        Scans[i] += UnityEngine.Random.Range(-MaxNoiseValue, MaxNoiseValue); // Noise

        if (VisualizeLaserLines)
          Debug.DrawLine(transform.position, hit.point, Color.red, 1.0f / LidarFrequency, true);

      }
      else
      {
        Scans[i] = Mathf.Infinity;

        if (VisualizeLaserLines)
          Debug.DrawLine(transform.position, transform.position + Direction*TraceMaxDistance, Color.white, 1.0f / LidarFrequency, true);
      }

    }

    LaserScan.ranges = Scans;
    ros.Publish(topicName, LaserScan);
  
  }
  
}
