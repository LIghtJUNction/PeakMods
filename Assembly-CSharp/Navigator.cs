// Decompiled with JetBrains decompiler
// Type: Navigator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.AI;

#nullable disable
public class Navigator : MonoBehaviour
{
  [HideInInspector]
  public NavMeshAgent agent;
  private Bot bot;
  private Vector3 lastReadTargetPosition;

  private void Awake()
  {
    this.agent = this.GetComponent<NavMeshAgent>();
    this.agent.updatePosition = false;
    this.agent.updateRotation = false;
    this.bot = this.GetComponentInParent<Bot>();
  }

  private void Start()
  {
  }

  public bool TryGetPointOnNavMeshCloseTo(Vector3 position, out NavMeshHit hit)
  {
    return NavMesh.SamplePosition(position, out hit, 2f, 1 << NavMesh.GetAreaFromName("Walkable"));
  }

  private void Update()
  {
    this.agent.nextPosition = this.bot.Center;
    this.bot.navigationDirection_read = this.agent.desiredVelocity.normalized;
    if (this.agent.isOnNavMesh)
      this.bot.remainingNavDistance = this.agent.remainingDistance;
    if (this.lastReadTargetPosition == this.bot.targetPos_Set || !this.agent.isOnNavMesh)
      return;
    this.lastReadTargetPosition = this.bot.targetPos_Set;
    this.agent.SetDestination(this.lastReadTargetPosition);
  }

  public void SetAgentVelocity(Vector3 velocity) => this.agent.velocity = velocity;
}
