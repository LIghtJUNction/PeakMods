// Decompiled with JetBrains decompiler
// Type: Wall
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

#nullable disable
public class Wall : MonoBehaviour
{
  public Vector2Int gridSize;
  public float gridCellSize;
  public List<WallPiece> pieces = new List<WallPiece>();

  internal void WallInit() => this.pieces = new List<WallPiece>();

  internal void AddPiece(WallPiece piece) => this.pieces.Add(piece);

  private void OnDrawGizmos()
  {
    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
    for (int x = 0; x < this.gridSize.x; ++x)
    {
      for (int y = 0; y < this.gridSize.y; ++y)
        Gizmos.DrawWireCube(this.GetGridPos(x, y), new Vector3(this.gridCellSize, this.gridCellSize, 0.25f));
    }
  }

  internal Vector3 GetGridPos(int x, int y)
  {
    Vector2 vector2_1 = ((Vector2) this.gridSize - Vector2.one) * this.gridCellSize;
    Vector2 vector2_2 = (Vector2) this.transform.position - vector2_1 * 0.5f;
    Vector2 vector2_3 = (Vector2) this.transform.position + vector2_1 * 0.5f;
    float t1 = (float) x / ((float) this.gridSize.x - 1f);
    float t2 = (float) y / ((float) this.gridSize.y - 1f);
    return new Vector3(Mathf.Lerp(vector2_2.x, vector2_3.x, t1), Mathf.Lerp(vector2_2.y, vector2_3.y, t2), this.transform.position.z);
  }

  internal Vector3 SnapToPosition(Vector3 position)
  {
    Vector2 vector2_1 = ((Vector2) this.gridSize - Vector2.one) * this.gridCellSize;
    Vector2 vector2_2 = (Vector2) this.transform.position - vector2_1 * 0.5f;
    Vector2 vector2_3 = (Vector2) this.transform.position + vector2_1 * 0.5f;
    float num1 = Mathf.InverseLerp(vector2_2.x, vector2_3.x, position.x);
    double num2 = (double) Mathf.InverseLerp(vector2_2.y, vector2_3.y, position.y);
    int x = Mathf.RoundToInt(num1 * ((float) this.gridSize.x - 1f));
    double num3 = (double) this.gridSize.y - 1.0;
    int y = Mathf.RoundToInt((float) (num2 * num3));
    return this.GetGridPos(x, y);
  }

  internal bool PieceFits(WallPiece piece, int x, int y)
  {
    foreach (WallPiece piece1 in this.pieces)
    {
      if (this.CollisionCheck(piece, x, y, piece1))
        return false;
    }
    return true;
  }

  private bool CollisionCheck(WallPiece newPiece, int newPosX, int newPosY, WallPiece existing)
  {
    for (int index1 = 0; index1 < newPiece.dimention.x; ++index1)
    {
      for (int index2 = 0; index2 < newPiece.dimention.y; ++index2)
      {
        if (this.CollisionCheckSpot(new Vector2Int(newPosX + index1, newPosY + index2), existing))
          return true;
      }
    }
    return false;
  }

  private bool CollisionCheckSpot(Vector2Int checkPos, WallPiece existing)
  {
    for (int index1 = 0; index1 < existing.dimention.x; ++index1)
    {
      for (int index2 = 0; index2 < existing.dimention.y; ++index2)
      {
        if (new Vector2Int(existing.wallPosition.x + index1, existing.wallPosition.y + index2) == checkPos)
          return true;
      }
    }
    return false;
  }
}
