// Decompiled with JetBrains decompiler
// Type: HelperFunctions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

#nullable disable
public class HelperFunctions : MonoBehaviour
{
  public static LayerMask AllPhysical = (LayerMask) LayerMask.GetMask("Terrain", "Map", "Default", "Character", "Rope");
  public static LayerMask AllPhysicalExceptCharacter = (LayerMask) LayerMask.GetMask("Terrain", "Map", "Default", "Rope");
  public static LayerMask AllPhysicalExceptDefault = (LayerMask) LayerMask.GetMask("Terrain", "Map", "Character", "Rope");
  public static LayerMask terrainMapMask = (LayerMask) LayerMask.GetMask("Terrain", "Map");
  public static LayerMask terrainMask = (LayerMask) LayerMask.GetMask("Terrain");
  public static LayerMask MapMask = (LayerMask) LayerMask.GetMask("Map");
  public static LayerMask DefaultMask = (LayerMask) LayerMask.GetMask("Default");
  public static LayerMask CharacterAndDefaultMask = (LayerMask) LayerMask.GetMask("Character", "Default");

  internal static Terrain GetTerrain(Vector3 center)
  {
    RaycastHit raycastHit = HelperFunctions.LineCheck(center + Vector3.up * 1000f, center - Vector3.up * 1000f, HelperFunctions.LayerType.Terrain);
    return (bool) (UnityEngine.Object) raycastHit.transform ? raycastHit.transform.GetComponent<Terrain>() : (Terrain) null;
  }

  public static LayerMask GetMask(HelperFunctions.LayerType layerType)
  {
    switch (layerType)
    {
      case HelperFunctions.LayerType.AllPhysical:
        return HelperFunctions.AllPhysical;
      case HelperFunctions.LayerType.TerrainMap:
        return HelperFunctions.terrainMapMask;
      case HelperFunctions.LayerType.Terrain:
        return HelperFunctions.terrainMask;
      case HelperFunctions.LayerType.Map:
        return HelperFunctions.MapMask;
      case HelperFunctions.LayerType.Default:
        return HelperFunctions.DefaultMask;
      case HelperFunctions.LayerType.AllPhysicalExceptCharacter:
        return HelperFunctions.AllPhysicalExceptCharacter;
      case HelperFunctions.LayerType.CharacterAndDefault:
        return HelperFunctions.CharacterAndDefaultMask;
      case HelperFunctions.LayerType.AllPhysicalExceptDefault:
        return HelperFunctions.AllPhysicalExceptDefault;
      default:
        return HelperFunctions.MapMask;
    }
  }

  public static Vector3 GetGroundPos(
    Vector3 from,
    HelperFunctions.LayerType layerType,
    float radius = 0.0f)
  {
    Vector3 groundPos = from;
    RaycastHit raycastHit = HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius);
    if ((bool) (UnityEngine.Object) raycastHit.transform)
      groundPos = raycastHit.point;
    return groundPos;
  }

  public static RaycastHit GetGroundPosRaycast(
    Vector3 from,
    HelperFunctions.LayerType layerType,
    float radius = 0.0f)
  {
    return HelperFunctions.LineCheck(from, from + Vector3.down * 10000f, layerType, radius);
  }

  internal static GameObject InstantiatePrefab(
    GameObject sourceObj,
    Vector3 pos,
    Quaternion rot,
    Transform parent)
  {
    GameObject gameObject = HelperFunctions.InstantiatePrefab(sourceObj, parent);
    gameObject.transform.position = pos;
    gameObject.transform.rotation = rot;
    return gameObject;
  }

  internal static GameObject InstantiatePrefab(GameObject sourceObj, Transform parent)
  {
    GameObject gameObject = (GameObject) null;
    if (!Application.isEditor)
      gameObject = UnityEngine.Object.Instantiate<GameObject>(sourceObj, parent);
    return gameObject;
  }

  public static RaycastHit GetGroundPosRaycast(
    Vector3 from,
    HelperFunctions.LayerType layerType,
    Vector3 gravityDir,
    float radius = 0.0f)
  {
    return HelperFunctions.LineCheck(from, from + gravityDir * 10000f, layerType, radius);
  }

  public static RaycastHit LineCheck(
    Vector3 from,
    Vector3 to,
    HelperFunctions.LayerType layerType,
    float radius = 0.0f,
    QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
  {
    RaycastHit hitInfo = new RaycastHit();
    Ray ray = new Ray(from, to - from);
    if ((double) radius == 0.0)
      Physics.Raycast(ray, out hitInfo, Vector3.Distance(from, to), (int) HelperFunctions.GetMask(layerType));
    else
      Physics.SphereCast(ray, radius, out hitInfo, Vector3.Distance(from, to), (int) HelperFunctions.GetMask(layerType));
    return hitInfo;
  }

  public static RaycastHit[] LineCheckAll(
    Vector3 from,
    Vector3 to,
    HelperFunctions.LayerType layerType,
    float radius = 0.0f,
    QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Ignore)
  {
    return (double) radius != 0.0 ? Physics.SphereCastAll(from, radius, to - from, Vector3.Distance(from, to), (int) HelperFunctions.GetMask(layerType), triggerInteraction) : Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), (int) HelperFunctions.GetMask(layerType), triggerInteraction);
  }

  public static RaycastHit LineCheckIgnoreItem(
    Vector3 from,
    Vector3 to,
    HelperFunctions.LayerType layerType,
    Item ignoreItem)
  {
    RaycastHit raycastHit1 = new RaycastHit();
    foreach (RaycastHit raycastHit2 in Physics.RaycastAll(from, to - from, Vector3.Distance(from, to), (int) HelperFunctions.GetMask(layerType)))
    {
      Item componentInParent = raycastHit2.collider.GetComponentInParent<Item>();
      if ((!(bool) (UnityEngine.Object) componentInParent || !((UnityEngine.Object) componentInParent == (UnityEngine.Object) ignoreItem)) && ((UnityEngine.Object) raycastHit1.collider == (UnityEngine.Object) null || (double) raycastHit1.distance > (double) raycastHit2.distance))
        raycastHit1 = raycastHit2;
    }
    return raycastHit1;
  }

  internal static ConfigurableJoint AttachPositionJoint(
    Rigidbody rig1,
    Rigidbody rig2,
    bool useCustomConnection = false,
    Vector3 customConnectionPoint = default (Vector3))
  {
    ConfigurableJoint configurableJoint = rig1.gameObject.AddComponent<ConfigurableJoint>();
    configurableJoint.xMotion = ConfigurableJointMotion.Locked;
    configurableJoint.yMotion = ConfigurableJointMotion.Locked;
    configurableJoint.zMotion = ConfigurableJointMotion.Locked;
    configurableJoint.projectionMode = JointProjectionMode.PositionAndRotation;
    configurableJoint.anchor = !useCustomConnection ? rig1.transform.InverseTransformPoint(rig2.position) : rig1.transform.InverseTransformPoint(customConnectionPoint);
    configurableJoint.enableCollision = false;
    configurableJoint.connectedBody = rig2;
    return configurableJoint;
  }

  internal static Joint AttachFixedJoint(Rigidbody rig1, Rigidbody rig2)
  {
    FixedJoint fixedJoint = rig1.gameObject.AddComponent<FixedJoint>();
    fixedJoint.enableCollision = false;
    fixedJoint.connectedBody = rig2;
    return (Joint) fixedJoint;
  }

  internal static Vector3 RandomOnFlatCircle()
  {
    Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
    return new Vector3(insideUnitCircle.x, 0.0f, insideUnitCircle.y);
  }

  internal static void DestroyAll(UnityEngine.Object[] objects)
  {
    for (int index = objects.Length - 1; index >= 0; --index)
      UnityEngine.Object.Destroy(objects[index]);
  }

  internal static Vector3 EulerToLook(Vector2 euler) => new Vector3(euler.y, -euler.x, 0.0f);

  internal static Vector3 LookToEuler(Vector2 lookRotationValues)
  {
    return new Vector3(-lookRotationValues.y, lookRotationValues.x, 0.0f);
  }

  internal static Vector3 LookToDirection(Vector3 look, Vector3 targetDir)
  {
    return HelperFunctions.EulerToDirection(HelperFunctions.LookToEuler((Vector2) look), targetDir);
  }

  internal static Vector3 EulerToDirection(Vector3 euler, Vector3 targetDir)
  {
    return Quaternion.Euler(euler) * targetDir;
  }

  internal static Vector3 DirectionToEuler(Vector3 dir)
  {
    return Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
  }

  internal static Vector3 DirectionToLook(Vector3 dir)
  {
    Vector3 euler = HelperFunctions.DirectionToEuler(dir);
    while ((double) euler.x > 180.0)
      euler.x -= 360f;
    return HelperFunctions.EulerToLook((Vector2) euler);
  }

  internal static Vector3 GroundDirection(Vector3 planeNormal, Vector3 sideDirection)
  {
    return -Vector3.Cross(sideDirection, planeNormal);
  }

  internal static Vector3 SeparateClamps(Vector3 rotationError, float clamp)
  {
    rotationError.x = Mathf.Clamp(rotationError.x, -clamp, clamp);
    rotationError.y = Mathf.Clamp(rotationError.y, -clamp, clamp);
    rotationError.z = Mathf.Clamp(rotationError.z, -clamp, clamp);
    return rotationError;
  }

  internal static float FlatDistance(Vector3 from, Vector3 to)
  {
    return Vector2.Distance(from.XZ(), to.XZ());
  }

  internal static void IgnoreConnect(Rigidbody rig1, Rigidbody rig2)
  {
    rig1.gameObject.AddComponent<ConfigurableJoint>().connectedBody = rig2;
  }

  internal static RaycastHit[] SortRaycastResults(RaycastHit[] hitsToSort)
  {
    ((IList<RaycastHit>) hitsToSort).Sort<RaycastHit>(new Comparison<RaycastHit>(HelperFunctions.RaycastHitComparer));
    return hitsToSort;
  }

  public static Vector3[] GetCircularDirections(int count)
  {
    Vector3[] circularDirections = new Vector3[count];
    float num = 360f / (float) count;
    for (int index = 0; index < count; ++index)
    {
      float f = (float) Math.PI / 180f * ((float) index * num);
      circularDirections[index] = new Vector3(Mathf.Cos(f), 0.0f, Mathf.Sin(f)).normalized;
    }
    return circularDirections;
  }

  private static int RaycastHitComparer(RaycastHit x, RaycastHit y)
  {
    return (double) x.distance < (double) y.distance ? -1 : 1;
  }

  internal static Quaternion GetRandomRotationWithUp(Vector3 normal)
  {
    Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere with
    {
      y = 0.0f
    };
    return Quaternion.LookRotation(Vector3.Cross(normal, Vector3.Cross(normal, onUnitSphere)), normal);
  }

  public static Bounds GetTotalBounds(GameObject gameObject)
  {
    return HelperFunctions.GetTotalBounds((IEnumerable<Renderer>) gameObject.GetComponentsInChildren<MeshRenderer>());
  }

  internal static Vector3 GetCenterOfMass(Transform transform)
  {
    Vector3 zero = Vector3.zero;
    float num = 0.0f;
    for (int index = 0; index < transform.childCount; ++index)
    {
      Collider component = transform.GetChild(index).GetComponent<Collider>();
      if ((bool) (UnityEngine.Object) component)
      {
        zero += component.transform.position;
        ++num;
      }
    }
    Vector3 position = zero / num;
    return transform.InverseTransformPoint(position);
  }

  public static Bounds GetTotalBounds(IEnumerable<Renderer> rends)
  {
    Bounds totalBounds = new Bounds();
    bool flag = true;
    foreach (Renderer rend in rends)
    {
      if (flag)
      {
        totalBounds = rend.bounds;
        flag = false;
      }
      else
        totalBounds.Encapsulate(rend.bounds);
    }
    return totalBounds;
  }

  public static List<Tout> GetComponentListFromComponentArray<Tin, Tout>(
    IEnumerable<Tin> inComponents)
    where Tin : Component
    where Tout : Component
  {
    List<Tout> fromComponentArray = new List<Tout>();
    foreach (Tin inComponent in inComponents)
    {
      Tout component = inComponent.GetComponent<Tout>();
      if ((bool) (UnityEngine.Object) component)
        fromComponentArray.Add(component);
    }
    return fromComponentArray;
  }

  internal static IEnumerable<T> SortBySiblingIndex<T>(IEnumerable<T> componentsToSort) where T : Component
  {
    List<T> objList = new List<T>();
    objList.AddRange(componentsToSort);
    objList.Sort((Comparison<T>) ((p1, p2) => p1.transform.GetSiblingIndex().CompareTo(p2.transform.GetSiblingIndex())));
    return (IEnumerable<T>) objList;
  }

  internal static float FlatAngle(Vector3 dir1, Vector3 dir2)
  {
    return Vector3.Angle(dir1.Flat(), dir2.Flat());
  }

  internal static void SetChildCollidersLayer(Transform root, int layerID)
  {
    foreach (Component componentsInChild in root.GetComponentsInChildren<Collider>())
      componentsInChild.gameObject.layer = layerID;
  }

  internal static void SetJointDrive(
    ConfigurableJoint joint,
    float spring,
    float damper,
    Rigidbody rig)
  {
    JointDrive angularXdrive = joint.angularXDrive with
    {
      positionSpring = spring * rig.mass,
      positionDamper = damper * rig.mass
    };
    joint.angularXDrive = angularXdrive;
    joint.angularYZDrive = angularXdrive;
  }

  internal static Transform FindChildRecursive(string targetName, Transform root)
  {
    if (root.gameObject.name.ToUpper() == targetName.ToUpper())
      return root;
    for (int index = 0; index < root.childCount; ++index)
    {
      Transform childRecursive = HelperFunctions.FindChildRecursive(targetName, root.GetChild(index));
      if (!((UnityEngine.Object) childRecursive == (UnityEngine.Object) null) && childRecursive.gameObject.name.ToUpper() == targetName.ToUpper())
        return childRecursive;
    }
    return (Transform) null;
  }

  internal static void PhysicsRotateTowards(Rigidbody rig, Vector3 from, Vector3 to, float force)
  {
    Vector3 vector3 = Vector3.Cross(from, to).normalized * Vector3.Angle(from, to);
    rig.AddTorque(vector3 * force, ForceMode.Acceleration);
  }

  internal static Vector3 MultiplyVectors(Vector3 v1, Vector3 v2)
  {
    v1.x *= v2.x;
    v1.y *= v2.y;
    v1.z *= v2.z;
    return v1;
  }

  public static Vector3 CubicBezier(
    Vector3 Start,
    Vector3 _P1,
    Vector3 _P2,
    Vector3 end,
    float _t)
  {
    return (1f - _t) * HelperFunctions.QuadraticBezier(Start, _P1, _P2, _t) + _t * HelperFunctions.QuadraticBezier(_P1, _P2, end, _t);
  }

  public static Vector3 QuadraticBezier(Vector3 start, Vector3 _P1, Vector3 end, float _t)
  {
    return (1f - _t) * HelperFunctions.LinearBezier(start, _P1, _t) + _t * HelperFunctions.LinearBezier(_P1, end, _t);
  }

  public static Vector3 LinearBezier(Vector3 start, Vector3 end, float _t)
  {
    return (1f - _t) * start + _t * end;
  }

  internal static Vector3 GetRandomPositionInBounds(Bounds bounds)
  {
    return new Vector3(Mathf.Lerp(bounds.min.x, bounds.max.x, UnityEngine.Random.value), Mathf.Lerp(bounds.min.y, bounds.max.y, UnityEngine.Random.value), Mathf.Lerp(bounds.min.z, bounds.max.z, UnityEngine.Random.value));
  }

  internal static GameObject SpawnPrefab(
    GameObject gameObject,
    Vector3 position,
    Quaternion rotation,
    Transform transform)
  {
    GameObject gameObject1 = (GameObject) null;
    if (!Application.isEditor)
      gameObject1 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
    gameObject1.transform.SetParent(transform);
    gameObject1.transform.rotation = rotation;
    gameObject1.transform.position = position;
    return gameObject1;
  }

  internal static Quaternion GetRotationWithUp(Vector3 forward, Vector3 up)
  {
    return Quaternion.LookRotation(Vector3.ProjectOnPlane(forward, up), up);
  }

  internal static float BoxDistance(Vector3 pos1, Vector3 pos2)
  {
    return Mathf.Max(Mathf.Max(Mathf.Max(0.0f, Mathf.Abs(pos1.x - pos2.x)), Mathf.Abs(pos1.y - pos2.y)), Mathf.Abs(pos1.z - pos2.z));
  }

  internal static bool CanSee(Transform looker, Vector3 pos, float maxAngle = 70f)
  {
    return (double) Vector3.Angle(looker.forward, pos - looker.position) <= (double) maxAngle && !(bool) (UnityEngine.Object) HelperFunctions.LineCheck(looker.transform.position, pos, HelperFunctions.LayerType.TerrainMap).transform;
  }

  internal static bool InBoxRange(Vector3 position1, Vector3 position2, int range)
  {
    return (double) Mathf.Abs(position1.x - position2.x) <= (double) range && (double) Mathf.Abs(position1.y - position2.y) <= (double) range && (double) Mathf.Abs(position1.z - position2.z) <= (double) range;
  }

  internal static UnityEngine.Random.State SetRandomSeedFromWorldPos(Vector3 position, int seed)
  {
    position.x = (float) Mathf.RoundToInt(position.x);
    position.y = (float) Mathf.RoundToInt(position.y);
    position.z = (float) Mathf.RoundToInt(position.z);
    UnityEngine.Random.State state = UnityEngine.Random.state;
    Debug.Log((object) "Set Seed");
    UnityEngine.Random.InitState(Mathf.RoundToInt((float) ((double) seed + (double) position.x + (double) position.y * 100.0 + (double) position.z * 10000.0)));
    return state;
  }

  public static List<Transform> FindAllChildrenWithTag(string targetTag, Transform target)
  {
    List<Transform> allChildrenWithTag = new List<Transform>();
    for (int index = 0; index < target.childCount; ++index)
    {
      Transform child = target.GetChild(index);
      if (child.name.Contains(targetTag))
        allChildrenWithTag.Add(child);
      allChildrenWithTag.AddRange((IEnumerable<Transform>) HelperFunctions.FindAllChildrenWithTag(targetTag, child));
    }
    return allChildrenWithTag;
  }

  internal static T[] GridToFlatArray<T>(T[,] grid)
  {
    T[] flatArray = new T[grid.GetLength(0) * grid.GetLength(1)];
    int length = grid.GetLength(0);
    for (int index1 = 0; index1 < length; ++index1)
    {
      for (int index2 = 0; index2 < length; ++index2)
      {
        int index3 = index1 * length + index2;
        flatArray[index3] = grid[index2, index1];
      }
    }
    return flatArray;
  }

  internal static NativeArray<float> FloatGridToNativeArray(float[,] floats)
  {
    NativeArray<float> nativeArray = new NativeArray<float>(floats.GetLength(0) * floats.GetLength(1), Allocator.TempJob);
    int length = floats.GetLength(0);
    for (int index1 = 0; index1 < length; ++index1)
    {
      for (int index2 = 0; index2 < length; ++index2)
      {
        int index3 = index1 * length + index2;
        nativeArray[index3] = floats[index1, index2];
      }
    }
    return nativeArray;
  }

  internal static float[,] NativeArrayToFloatGrid(NativeArray<float> array, int arrayLength)
  {
    float[,] floatGrid = new float[arrayLength, arrayLength];
    int length = array.Length;
    for (int index1 = 0; index1 < length; ++index1)
    {
      int index2 = Mathf.FloorToInt((float) (index1 / arrayLength));
      int index3 = index1 - index2 * arrayLength;
      floatGrid[index2, index3] = array[index1];
    }
    return floatGrid;
  }

  public static Vector2Int GetIndex_FlatToGrid(int flatIndex, int arrayLength)
  {
    int x = Mathf.FloorToInt((float) (flatIndex / arrayLength));
    int y = flatIndex - x * arrayLength;
    return new Vector2Int(x, y);
  }

  public static int GetIndex_GridToFlat(Vector2Int gridIndex, int arrayLength)
  {
    return gridIndex.x * arrayLength + gridIndex.y;
  }

  internal static List<Vector2Int> GetIndexesInBounds(
    int xRess,
    int yRess,
    Bounds selectionBounds,
    Bounds totalBounds)
  {
    int num1 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.min.x) * (float) xRess);
    int num2 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.x, totalBounds.max.x, selectionBounds.max.x) * (float) xRess);
    int num3 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.min.z) * (float) xRess);
    int num4 = Mathf.RoundToInt(Mathf.InverseLerp(totalBounds.min.z, totalBounds.max.z, selectionBounds.max.z) * (float) yRess);
    List<Vector2Int> indexesInBounds = new List<Vector2Int>();
    for (int x = num1; x < num2; ++x)
    {
      for (int y = num3; y < num4; ++y)
      {
        indexesInBounds.Add(new Vector2Int(x, y));
        HelperFunctions.IDToWorldPos(x, y, xRess, yRess, totalBounds);
      }
    }
    return indexesInBounds;
  }

  public static Vector3 IDToWorldPos(int x, int y, int xRess, int yRess, Bounds totalBounds)
  {
    float t1 = (float) x / ((float) xRess - 1f);
    float t2 = (float) y / ((float) yRess - 1f);
    return new Vector3(Mathf.Lerp(totalBounds.min.x, totalBounds.max.x, t1), 0.0f, Mathf.Lerp(totalBounds.min.z, totalBounds.max.z, t2));
  }

  internal static Vector3 GetRadomPointInBounds(Bounds b)
  {
    Vector3 min = b.min;
    Vector3 max = b.max;
    return new Vector3(Mathf.Lerp(min.x, max.x, UnityEngine.Random.value), Mathf.Lerp(min.y, max.y, UnityEngine.Random.value), Mathf.Lerp(min.z, max.z, UnityEngine.Random.value));
  }

  internal static Camera GetMainCamera()
  {
    if ((UnityEngine.Object) MainCamera.instance == (UnityEngine.Object) null)
    {
      MainCamera.instance = UnityEngine.Object.FindAnyObjectByType<MainCamera>();
      MainCamera.instance.cam = MainCamera.instance.GetComponent<Camera>();
    }
    return MainCamera.instance.cam;
  }

  internal static Color GetVertexColorAtPoint(
    Vector3[] verts,
    Color[] colors,
    Transform transform,
    Vector3 point)
  {
    if (colors.Length == 0)
      return Color.black;
    Color vertexColorAtPoint = Color.black;
    float num1 = 1E+07f;
    for (int index = 0; index < verts.Length; ++index)
    {
      Vector3 b = transform.TransformPoint(verts[index]);
      float num2 = Vector3.Distance(point, b);
      if ((double) num2 < (double) num1)
      {
        num1 = num2;
        vertexColorAtPoint = colors[index];
      }
    }
    return vertexColorAtPoint;
  }

  internal static float GetValue(Color color) => Mathf.Max(color.r, color.g, color.b);

  public static T RandomSelection<T>(List<T> list)
  {
    return list == null || list.Count == 0 ? default (T) : list[UnityEngine.Random.Range(0, list.Count)];
  }

  public static bool IsLayerInLayerMask(LayerMask layerMask, int layer)
  {
    return (layerMask.value & 1 << layer) != 0;
  }

  public static bool IsLayerInLayerMask(HelperFunctions.LayerType layerType, int layer)
  {
    return HelperFunctions.IsLayerInLayerMask(HelperFunctions.GetMask(layerType), layer);
  }

  public static Vector3 ZeroY(Vector3 original) => new Vector3(original.x, 0.0f, original.z);

  internal static bool AnyPlayerInZRange(float min, float max)
  {
    foreach (Character allCharacter in Character.AllCharacters)
    {
      if (!allCharacter.isBot && (double) allCharacter.Center.z >= (double) min && (double) allCharacter.Center.z <= (double) max)
        return true;
    }
    return false;
  }

  public enum LayerType
  {
    AllPhysical,
    TerrainMap,
    Terrain,
    Map,
    Default,
    AllPhysicalExceptCharacter,
    CharacterAndDefault,
    AllPhysicalExceptDefault,
  }
}
