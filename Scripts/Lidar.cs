using System;
using UnityEngine;

public class Lidar : MonoBehaviour
{
  public float LidarFOV = 270.0f;
  public int LidarNumOfPoints = 1024;
  public float LidarFrequency = 30.0f;
  public float TraceMaxDistance = 10.0f;
  public float MaxNoiseValue = 0.01f;
  public bool VisualizeLaserLines = false;

  private float Step;
  private Vector3[] PointCloud;
  private Vector3 infinityVector;

  private float elapsedTime;

  void Start()
  {
    Step = LidarFOV / (LidarNumOfPoints - 1);

    Vector3 infinityVector = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    PointCloud = new Vector3[LidarNumOfPoints];
  }

  void Update()
  {
    elapsedTime += Time.deltaTime;

    if (elapsedTime > 1.0f / LidarFrequency)
    {
      GetLaserScan();
      elapsedTime -= 1.0f / LidarFrequency;
    }
  }

  void GetLaserScan()
  {
    Vector3 Start = transform.position;

    for (int i = 0; i < LidarNumOfPoints; i++)
    {
      float Angle = -(LidarFOV / 2) + Step * i;

      Vector3 Direction = Quaternion.AngleAxis(Angle, transform.up) * transform.forward;

      bool bHit = Physics.Raycast(Start, Direction, out RaycastHit hit, TraceMaxDistance);

      if (bHit)
      {
        PointCloud[i] = hit.point;
        PointCloud[i] += Direction * UnityEngine.Random.Range(-MaxNoiseValue, MaxNoiseValue); // Noise

        if (VisualizeLaserLines)
          Debug.DrawLine(transform.position, hit.point, Color.red, 1.0f / LidarFrequency, true);

      }
      else
      {
        PointCloud[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

        if (VisualizeLaserLines)
          Debug.DrawLine(transform.position, transform.position + Direction*TraceMaxDistance, Color.white, 1.0f / LidarFrequency, true);
      }

    }

  }

  void OnDrawGizmos()
  {
    if (Application.isPlaying && PointCloud != null)
    {
      Gizmos.color = Color.red;
      foreach (Vector3 point in PointCloud)
        if (point != infinityVector)
          Gizmos.DrawSphere(point, 0.005f);
    }
  }
  
}
