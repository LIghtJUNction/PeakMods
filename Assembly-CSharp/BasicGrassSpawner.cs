// Decompiled with JetBrains decompiler
// Type: BasicGrassSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Zorro.Core;

#nullable disable
[BurstCompile]
public class BasicGrassSpawner : MonoBehaviour
{
  public Transform placableParent;
  public Material grassMaterial;
  public ComputeShader grassComputeShader;

  public void Generate()
  {
    SpawnRoutine();

    void SpawnRoutine()
    {
      ((IEnumerable<GrassChunk>) this.GetComponentsInChildren<GrassChunk>()).ForEach<GrassChunk>((Action<GrassChunk>) (chunk => UnityEngine.Object.DestroyImmediate((UnityEngine.Object) chunk.gameObject)));
      NativeList<GrassPoint> nativeList = new NativeList<GrassPoint>(100000, (AllocatorManager.AllocatorHandle) Allocator.Persistent);
      int num1 = Mathf.RoundToInt(this.transform.localScale.x);
      int num2 = Mathf.RoundToInt(this.transform.localScale.y);
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int index1 = 0; index1 < num1; ++index1)
      {
        float t1 = (float) index1 / (float) num1;
        if (stopwatch.ElapsedMilliseconds > 10L)
        {
          stopwatch.Reset();
          stopwatch.Start();
        }
        for (int index2 = 0; index2 < num2; ++index2)
        {
          float t2 = (float) index2 / (float) num2;
          Optionable<RaycastHit> optionable = Raycast(this.transform.TransformPoint(new Vector3(Mathf.Lerp(-0.5f, 0.5f, t1), Mathf.Lerp(-0.5f, 0.5f, t2), 0.0f)), this.transform.forward, 500f);
          float srcStart = 0.2f;
          if (optionable.IsSome)
          {
            float perlin1 = GetPerlin(optionable.Value.point);
            RaycastHit raycastHit = optionable.Value;
            GrassPoint grassPoint;
            if ((double) perlin1 > (double) srcStart)
            {
              float num3 = math.remap(srcStart, 1f, 0.0f, 1f, perlin1);
              ref NativeList<GrassPoint> local1 = ref nativeList;
              grassPoint = new GrassPoint();
              grassPoint.worldPos = (float3) raycastHit.point;
              grassPoint.normal = (float3) raycastHit.normal;
              grassPoint.scale = num3;
              ref GrassPoint local2 = ref grassPoint;
              local1.Add(in local2);
            }
            int num4 = UnityEngine.Random.Range(10, 20);
            for (int index3 = 0; index3 < num4; ++index3)
            {
              Vector3 vector3 = UnityEngine.Random.insideUnitSphere * 2f;
              optionable = Raycast(raycastHit.point + raycastHit.normal * 4f + vector3, -raycastHit.normal, 500f);
              if (optionable.IsSome)
              {
                float perlin2 = GetPerlin(optionable.Value.point);
                if ((double) perlin2 > (double) srcStart)
                {
                  float num5 = math.remap(srcStart, 1f, 0.0f, 1f, perlin2);
                  ref NativeList<GrassPoint> local3 = ref nativeList;
                  grassPoint = new GrassPoint();
                  grassPoint.worldPos = (float3) optionable.Value.point;
                  grassPoint.normal = (float3) optionable.Value.normal;
                  grassPoint.scale = num5;
                  ref GrassPoint local4 = ref grassPoint;
                  local3.Add(in local4);
                }
              }
            }
          }
        }
      }
      this.grassMaterial.EnableKeyword("PROCEDURAL_INSTANCING_ON");
      stopwatch.Reset();
      stopwatch.Start();
      Dictionary<int3, List<GrassPoint>> dictionary = new Dictionary<int3, List<GrassPoint>>();
      int num6 = 0;
      foreach (GrassPoint grassPoint in nativeList)
      {
        if (stopwatch.ElapsedMilliseconds > 10L)
        {
          double num7 = (double) num6 / (double) nativeList.Length;
          stopwatch.Reset();
          stopwatch.Start();
        }
        int3 chunkFromPosition = GrassChunking.GetChunkFromPosition(grassPoint.worldPos);
        if (!dictionary.ContainsKey(chunkFromPosition))
          dictionary.Add(chunkFromPosition, new List<GrassPoint>());
        dictionary[chunkFromPosition].Add(grassPoint);
        ++num6;
      }
      stopwatch.Reset();
      stopwatch.Start();
      int count = dictionary.Count;
      int num8 = 0;
      foreach (KeyValuePair<int3, List<GrassPoint>> keyValuePair in dictionary)
      {
        int3 key = keyValuePair.Key;
        if (stopwatch.ElapsedMilliseconds > 10L)
        {
          double num9 = (double) num8 / (double) count;
          stopwatch.Reset();
          stopwatch.Start();
        }
        UnityEngine.Debug.Log((object) $"Chunk: {key}");
        GameObject gameObject = new GameObject($"Chunk {key}");
        gameObject.transform.position = (Vector3) ((float3) key * GrassChunking.CHUNK_SIZE);
        gameObject.AddComponent<GrassChunk>().SetData(keyValuePair.Value);
        gameObject.transform.SetParent(this.transform);
        GrassRenderer grassRenderer = gameObject.AddComponent<GrassRenderer>();
        grassRenderer.grassComputeShader = this.grassComputeShader;
        grassRenderer.m_grassRenderMaterial = this.grassMaterial;
        grassRenderer.CurrentChunk = key;
        ++num8;
      }
      nativeList.Dispose();
    }

    static float GetPerlin(Vector3 worldPos)
    {
      return Mathf.Clamp01(Mathf.PerlinNoise((float) ((double) worldPos.x * 0.10000000149011612 + (double) worldPos.y * 0.019999999552965164), worldPos.z * 0.1f));
    }

    Optionable<RaycastHit> Raycast(Vector3 pos, Vector3 direction, float distance)
    {
      RaycastHit hitInfo;
      return Physics.Raycast(new Ray(pos, direction), out hitInfo, distance) && (double) Vector3.Angle(hitInfo.normal, Vector3.up) < 45.0 && hitInfo.transform.IsGrandChildOf(this.placableParent) && (hitInfo.collider.gameObject.layer == 20 || hitInfo.collider.tag.Contains("Stone")) ? Optionable<RaycastHit>.Some(hitInfo) : Optionable<RaycastHit>.None;
    }
  }
}
