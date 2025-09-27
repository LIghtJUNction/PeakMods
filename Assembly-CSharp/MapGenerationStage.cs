// Decompiled with JetBrains decompiler
// Type: MapGenerationStage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class MapGenerationStage : MonoBehaviour
{
  public BoxCollider spawnRange;
  public float nodeSpacing = 1f;
  [Range(0.0f, 1f)]
  public float defaultDensity;
  public Vector2 minMaxDensity = new Vector2(0.0f, 1f);
  public float randomizedNodeOffset;
  public bool useCurveX;
  public AnimationCurve curveX;
  public bool useCurveZ;
  public AnimationCurve curveZ;
  public List<MapGenerationStage.GenerationProximityPassData> proximityPassData;
  public bool useMinimumHeightLimit;
  public float minimumHeightLimit;
  public bool useMaximumHeightLimit;
  public float maximumHeightLimit;
  public MapGenerationStage.SpawnMode spawnMode;
  public GameObject objectPrefab;
  public SpawnList spawnList;
  public bool randomizeRotation = true;
  public bool randomizeRotationOnNormalPlane = true;
  public bool raycastDownward = true;
  public List<string> allowedTags;
  public Vector2 heightVariation;
  public Vector2 scaleVariation;
  public Color testGizmoColor = Color.red;
  public float testGizmoSize = 0.5f;
  public List<List<MapGenerationStage.GenerationNode>> nodeMap = new List<List<MapGenerationStage.GenerationNode>>();
  public List<GameObject> spawnedObjects;
  private RaycastHit hit;

  private bool singleObject => this.spawnMode == MapGenerationStage.SpawnMode.SingleObject;

  private void OnDrawGizmosSelected()
  {
    if (this.useMinimumHeightLimit)
    {
      Gizmos.color = new Color(1f, 0.21f, 0.0f, 0.49f);
      Gizmos.DrawCube(this.transform.position + new Vector3(0.0f, this.minimumHeightLimit, 0.0f), new Vector3(1000f, 0.01f, 1000f));
    }
    if (!this.useMaximumHeightLimit)
      return;
    Gizmos.color = new Color(0.0f, 1f, 0.96f, 0.49f);
    Gizmos.DrawCube(this.transform.position + new Vector3(0.0f, this.maximumHeightLimit, 0.0f), new Vector3(1000f, 0.01f, 1000f));
  }

  public void Generate(int seed = 0)
  {
    this.ClearSpawnedObjects();
    this.GenerateNodeMap();
    this.RunProximityPasses();
    this.SpawnObjectsFromNodeMap();
  }

  public void ClearSpawnedObjects()
  {
    for (int index = this.transform.childCount - 1; index >= 0; --index)
      UnityEngine.Object.DestroyImmediate((UnityEngine.Object) this.transform.GetChild(index).gameObject);
    this.spawnedObjects.Clear();
  }

  private void GenerateNodeMap()
  {
    if ((double) this.nodeSpacing == 0.0)
    {
      Debug.LogError((object) "NODE SPACING IS ZERO! THIS WOULD RESULT IN INFINITE SPAWNING!");
    }
    else
    {
      Vector2 vector2_1;
      ref Vector2 local1 = ref vector2_1;
      Bounds bounds = this.spawnRange.bounds;
      double x1 = (double) bounds.min.x;
      bounds = this.spawnRange.bounds;
      double z1 = (double) bounds.min.z;
      local1 = new Vector2((float) x1, (float) z1);
      Vector2 vector2_2;
      ref Vector2 local2 = ref vector2_2;
      bounds = this.spawnRange.bounds;
      double x2 = (double) bounds.max.x;
      bounds = this.spawnRange.bounds;
      double z2 = (double) bounds.max.z;
      local2 = new Vector2((float) x2, (float) z2);
      Vector2 vector2_3 = new Vector2(vector2_1.x, vector2_1.y);
      this.nodeMap.Clear();
      for (; (double) vector2_3.y <= (double) vector2_2.y; vector2_3.y += this.nodeSpacing)
      {
        List<MapGenerationStage.GenerationNode> generationNodeList = new List<MapGenerationStage.GenerationNode>();
        this.nodeMap.Add(generationNodeList);
        for (; (double) vector2_3.x <= (double) vector2_2.x; vector2_3.x += this.nodeSpacing)
        {
          Vector2 vector2_4 = new Vector2(vector2_3.x, vector2_3.y);
          if ((double) this.randomizedNodeOffset > 0.0)
            vector2_4 += new Vector2(UnityEngine.Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset), UnityEngine.Random.Range(-this.randomizedNodeOffset, this.randomizedNodeOffset));
          generationNodeList.Add(new MapGenerationStage.GenerationNode(new Vector2(vector2_4.x, vector2_4.y), this.defaultDensity));
        }
        vector2_3.x = vector2_1.x;
      }
    }
  }

  private void SpawnObjectsFromNodeMap()
  {
    this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.TrySpawnObject));
  }

  private void SpawnObject(Vector3 spot, Vector3 normal)
  {
    GameObject gameObject = !this.singleObject ? (this.singleObject || !(bool) (UnityEngine.Object) this.spawnList ? new GameObject() : UnityEngine.Object.Instantiate<GameObject>(this.spawnList.GetSingleSpawn())) : (!(bool) (UnityEngine.Object) this.objectPrefab ? new GameObject() : UnityEngine.Object.Instantiate<GameObject>(this.objectPrefab));
    if (this.randomizeRotation)
    {
      if (this.randomizeRotationOnNormalPlane)
        gameObject.transform.rotation = HelperFunctions.GetRandomRotationWithUp(normal);
      else
        gameObject.transform.eulerAngles = new Vector3(gameObject.transform.eulerAngles.x, (float) UnityEngine.Random.Range(0, 360), gameObject.transform.eulerAngles.z);
    }
    if (this.heightVariation != Vector2.zero)
      spot += Vector3.up * UnityEngine.Random.Range(this.heightVariation.x, this.heightVariation.y);
    if (this.scaleVariation != Vector2.zero)
    {
      float num = UnityEngine.Random.Range(this.scaleVariation.x, this.scaleVariation.y);
      gameObject.transform.localScale += new Vector3(num, num, num);
    }
    gameObject.transform.position = spot;
    gameObject.transform.SetParent(this.transform, true);
    LazyGizmo lazyGizmo = gameObject.AddComponent<LazyGizmo>();
    lazyGizmo.onSelected = false;
    lazyGizmo.color = this.testGizmoColor;
    lazyGizmo.radius = this.testGizmoSize;
    this.spawnedObjects.Add(gameObject);
  }

  private void TrySpawnObject(MapGenerationStage.GenerationNode node)
  {
    Vector3 vector3 = new Vector3(node.position.x, this.transform.position.y, node.position.y);
    Vector3 normal = Vector3.up;
    if ((this.raycastDownward || this.allowedTags.Count > 0) && Physics.Raycast(vector3 + Vector3.up * 50f, Vector3.down, out this.hit, 100f))
    {
      if (this.useMinimumHeightLimit && (double) this.hit.point.y < (double) this.transform.position.y + (double) this.minimumHeightLimit)
      {
        node.valid = false;
        return;
      }
      if (this.useMaximumHeightLimit && (double) this.hit.point.y > (double) this.transform.position.y + (double) this.maximumHeightLimit)
      {
        node.valid = false;
        return;
      }
      if (this.allowedTags.Count > 0 && !this.allowedTags.Contains(this.hit.collider.gameObject.tag))
      {
        node.valid = false;
        return;
      }
      if (this.raycastDownward)
      {
        vector3 = this.hit.point;
        normal = this.hit.normal;
        Debug.DrawLine(vector3, vector3 + normal * 10f, Color.red, 10f);
      }
    }
    if (!node.valid || (double) UnityEngine.Random.Range(0.0f, 1f) >= (double) node.probability)
      return;
    this.SpawnObject(vector3, normal);
  }

  private void RunProximityPasses()
  {
    this.RunActionOnAllNodes(new Action<MapGenerationStage.GenerationNode>(this.RunProximityPassesOnNode));
  }

  private void RunProximityPassesOnNode(MapGenerationStage.GenerationNode node)
  {
    this.RunPositionGradientPass(node);
    for (int index1 = 0; index1 < this.proximityPassData.Count; ++index1)
    {
      MapGenerationStage.GenerationProximityPassData proximityPassData = this.proximityPassData[index1];
      List<GameObject> spawnedObjects = proximityPassData.previousStage.spawnedObjects;
      for (int index2 = 0; index2 < spawnedObjects.Count; ++index2)
      {
        float num1 = Vector3.Distance((Vector3) node.position, (Vector3) Util.FlattenVector3(spawnedObjects[index2].transform.position));
        if ((double) num1 < (double) proximityPassData.hardAvoidanceRadius * (double) spawnedObjects[index2].transform.localScale.x)
          node.valid = false;
        else if ((double) num1 <= (double) proximityPassData.minMaxProximity.y)
        {
          float num2 = Util.RangeLerp(proximityPassData.correlation, 0.0f, proximityPassData.minMaxProximity.x, proximityPassData.minMaxProximity.y, num1);
          node.probability = Mathf.Clamp(node.probability + num2, this.minMaxDensity.x, this.minMaxDensity.y);
        }
      }
    }
  }

  private void RunPositionGradientPass(MapGenerationStage.GenerationNode node)
  {
    double num1 = (double) node.position.x - (double) this.spawnRange.bounds.min.x;
    Bounds bounds1 = this.spawnRange.bounds;
    double x1 = (double) bounds1.max.x;
    bounds1 = this.spawnRange.bounds;
    double x2 = (double) bounds1.min.x;
    double num2 = x1 - x2;
    float time1 = (float) (num1 / num2);
    double num3 = (double) node.position.y - (double) this.spawnRange.bounds.min.z;
    Bounds bounds2 = this.spawnRange.bounds;
    double z1 = (double) bounds2.max.z;
    bounds2 = this.spawnRange.bounds;
    double z2 = (double) bounds2.min.z;
    double num4 = z1 - z2;
    float time2 = (float) (num3 / num4);
    float num5 = 0.0f;
    float num6 = 0.0f;
    if (this.useCurveX)
      num5 = this.curveX.Evaluate(time1);
    if (this.useCurveZ)
      num6 = this.curveZ.Evaluate(time2);
    node.probability = Mathf.Clamp(node.probability + num5 + num6, this.minMaxDensity.x, this.minMaxDensity.y);
  }

  private void RunActionOnAllNodes(Action<MapGenerationStage.GenerationNode> Action)
  {
    for (int index1 = 0; index1 < this.nodeMap.Count; ++index1)
    {
      List<MapGenerationStage.GenerationNode> node = this.nodeMap[index1];
      for (int index2 = 0; index2 < node.Count; ++index2)
      {
        MapGenerationStage.GenerationNode generationNode = node[index2];
        Action(generationNode);
      }
    }
  }

  public enum SpawnMode
  {
    SingleObject,
    SpawnList,
  }

  public class GenerationNode
  {
    public Vector2 position;
    public float probability;
    public bool valid;

    public GenerationNode(Vector2 pos, float defaultProbability)
    {
      this.position = pos;
      this.probability = defaultProbability;
      this.valid = true;
    }
  }

  [Serializable]
  public class GenerationProximityPassData
  {
    public MapGenerationStage previousStage;
    public float hardAvoidanceRadius;
    public Vector2 minMaxProximity;
    public float correlation;
  }
}
