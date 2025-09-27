// Decompiled with JetBrains decompiler
// Type: WallPieceSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
public class WallPieceSpawner : MonoBehaviour
{
  public WallPiece[] pieces;
  private Transform root;
  private Wall wall;

  private void Go()
  {
    this.wall = this.GetComponent<Wall>();
    this.wall.WallInit();
    this.root = this.transform.Find("Pieces");
    this.Clear();
    for (int index = 0; index < 50; ++index)
      this.DoSpawns();
  }

  private void DoSpawns()
  {
    for (int x = 0; x < this.wall.gridSize.x; ++x)
    {
      for (int y = 0; y < this.wall.gridSize.y; ++y)
      {
        WallPiece randomPiece = this.GetRandomPiece();
        if (this.wall.PieceFits(randomPiece, x, y))
          this.SpawnPiece(randomPiece, x, y);
      }
    }
  }

  private void SpawnPiece(WallPiece piece, int x, int y)
  {
    WallPiece component = HelperFunctions.SpawnPrefab(piece.gameObject, this.wall.GetGridPos(x, y), Quaternion.identity, this.root).GetComponent<WallPiece>();
    component.wallPosition = new Vector2Int(x, y);
    this.wall.AddPiece(component);
  }

  private WallPiece GetRandomPiece() => this.pieces[Random.Range(0, this.pieces.Length)];

  private void Clear()
  {
    this.root = this.transform.Find("Pieces");
    for (int index = this.root.childCount - 1; index >= 0; --index)
      Object.DestroyImmediate((Object) this.root.GetChild(index).gameObject);
  }
}
