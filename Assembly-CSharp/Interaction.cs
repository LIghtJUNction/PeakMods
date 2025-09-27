// Decompiled with JetBrains decompiler
// Type: Interaction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: F3E770A0-BBB5-4DDE-8A67-A45EBB5236BA
// Assembly location: G:\LIghtJUNction\steam\SteamLibrary\steamapps\common\PEAK\PEAK_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
[DefaultExecutionOrder(600)]
public class Interaction : MonoBehaviour
{
  public float distance = 2f;
  public float area = 0.5f;
  public float maxCharacterInteractAngle = 90f;
  public static Interaction instance;
  public IInteractible currentHovered;
  public IInteractibleConstant currentHeldInteractible;
  public float currentConstantInteractableTime;
  private float _cihf;
  public RaycastHit[] sphereCastResults = new RaycastHit[100];
  internal IInteractible bestInteractable;
  [SerializeField]
  internal CharacterInteractible bestCharacter;
  [HideInInspector]
  public bool readyToInteract = true;
  [HideInInspector]
  public bool readyToReleaseInteract = true;
  [SerializeField]
  private string bestInteractableName;

  public float currentInteractableHeldTime
  {
    get => this._cihf;
    set => this._cihf = value;
  }

  public float constantInteractableProgress
  {
    get => this.currentInteractableHeldTime / this.currentConstantInteractableTime;
  }

  private void Awake() => Interaction.instance = this;

  private bool canInteract
  {
    get
    {
      return !Character.localCharacter.data.passedOut && !Character.localCharacter.data.fullyPassedOut && Character.localCharacter.CanDoInput() && !(bool) (Object) Character.localCharacter.data.currentStickyItem;
    }
  }

  private void LateUpdate()
  {
    this.currentHovered = (IInteractible) null;
    if (!(bool) (Object) Character.localCharacter)
      return;
    if (!this.canInteract)
    {
      this.bestInteractable = (IInteractible) null;
      this.bestCharacter = (CharacterInteractible) null;
    }
    else
    {
      this.DoInteractableRaycasts(out this.bestInteractable);
      this.bestCharacter = this.bestInteractable as CharacterInteractible;
      this.DoInteraction(this.bestInteractable);
    }
    this.bestInteractableName = this.bestInteractable == null ? "null" : this.bestInteractable.GetTransform().gameObject.name;
    this.currentHovered = this.bestInteractable;
  }

  public bool hasValidTargetCharacter => (Object) this.bestCharacter != (Object) null;

  private void DoInteraction(IInteractible interactable)
  {
    if (Character.localCharacter.input.interactWasReleased && interactable != null && this.currentHeldInteractible == interactable && this.readyToReleaseInteract)
    {
      if (interactable is IInteractibleConstant interactibleConstant)
        interactibleConstant.ReleaseInteract(Character.localCharacter);
      this.readyToReleaseInteract = false;
    }
    if (!Character.localCharacter.input.interactIsPressed)
    {
      this.readyToInteract = true;
      this.CancelHeldInteract();
    }
    else
    {
      if (this.readyToInteract && interactable != null)
      {
        this.readyToReleaseInteract = true;
        if (interactable is IInteractibleConstant interactibleConstant && interactibleConstant.IsConstantlyInteractable(Character.localCharacter))
        {
          this.currentHeldInteractible = interactibleConstant;
          this.currentConstantInteractableTime = interactibleConstant.GetInteractTime(Character.localCharacter);
        }
        interactable.Interact(Character.localCharacter);
        this.readyToInteract = false;
        return;
      }
      if (Character.localCharacter.input.interactIsPressed && this.currentHeldInteractible != null)
      {
        if (interactable != this.currentHeldInteractible)
        {
          this.currentHeldInteractible = (IInteractibleConstant) null;
        }
        else
        {
          this.currentInteractableHeldTime += Time.deltaTime;
          if ((double) this.currentInteractableHeldTime >= (double) this.currentConstantInteractableTime)
          {
            this.currentHeldInteractible.Interact_CastFinished(Character.localCharacter);
            this.readyToReleaseInteract = false;
            if (!this.currentHeldInteractible.holdOnFinish)
              this.CancelHeldInteract();
          }
        }
      }
    }
    if (this.currentHeldInteractible != null)
      return;
    this.CancelHeldInteract();
  }

  private void DoInteractableRaycasts(out IInteractible interactableResult)
  {
    if ((Object) Character.localCharacter.data.carriedPlayer != (Object) null && Character.localCharacter.refs.items.currentSelectedSlot.IsSome && Character.localCharacter.refs.items.currentSelectedSlot.Value == (byte) 3)
    {
      Debug.Log((object) "HEUH");
      interactableResult = (IInteractible) Character.localCharacter.data.carriedPlayer.refs.interactible;
    }
    else
    {
      float num1 = Vector3.Angle(Vector3.down, MainCamera.instance.transform.forward);
      if ((double) num1 <= 10.0)
      {
        foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
        {
          if ((Object) stickyItemComponent.stuckToCharacter == (Object) Character.localCharacter && (double) stickyItemComponent.item.Center().y <= (double) Character.localCharacter.Center.y)
          {
            interactableResult = (IInteractible) stickyItemComponent.item;
            return;
          }
        }
      }
      else if ((double) num1 >= 170.0)
      {
        foreach (StickyItemComponent stickyItemComponent in StickyItemComponent.ALL_STUCK_ITEMS)
        {
          if ((Object) stickyItemComponent.stuckToCharacter == (Object) Character.localCharacter && (double) stickyItemComponent.item.Center().y >= (double) Character.localCharacter.Center.y)
          {
            interactableResult = (IInteractible) stickyItemComponent.item;
            return;
          }
        }
      }
      Ray ray = new Ray(MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
      RaycastHit[] raycastHitArray = HelperFunctions.LineCheckAll(ray.origin, ray.origin + ray.direction * this.distance, HelperFunctions.LayerType.AllPhysical, triggerInteraction: QueryTriggerInteraction.Collide);
      IInteractible interactible = (IInteractible) null;
      RaycastHit raycastHit1 = new RaycastHit();
      raycastHit1.distance = float.MaxValue;
      foreach (RaycastHit raycastHit2 in raycastHitArray)
      {
        if ((double) raycastHit2.distance < (double) raycastHit1.distance && !Character.localCharacter.refs.ragdoll.colliderList.Contains(raycastHit2.collider))
        {
          Item componentInParent = raycastHit2.transform.GetComponentInParent<Item>();
          if (!(bool) (Object) componentInParent || !((Object) componentInParent == (Object) Character.localCharacter.data.currentItem))
            raycastHit1 = raycastHit2;
        }
      }
      if ((Object) raycastHit1.collider != (Object) null)
      {
        IInteractible componentInParent = raycastHit1.collider.GetComponentInParent<IInteractible>();
        if (componentInParent != null && componentInParent.IsInteractible(Character.localCharacter))
          interactible = componentInParent;
      }
      bool flag = interactible == null;
      if (flag)
      {
        float num2 = float.MaxValue;
        this.sphereCastResults = new RaycastHit[100];
        int num3 = Physics.SphereCastNonAlloc(MainCamera.instance.transform.position + MainCamera.instance.transform.forward * (this.area / 2f), this.area, MainCamera.instance.transform.forward, this.sphereCastResults, Mathf.Min(raycastHit1.distance, this.distance), (int) HelperFunctions.GetMask(HelperFunctions.LayerType.AllPhysical), QueryTriggerInteraction.Collide);
        for (int index = 0; index < num3 && index < this.sphereCastResults.Length; ++index)
        {
          RaycastHit sphereCastResult = this.sphereCastResults[index];
          Item componentInParent1 = sphereCastResult.transform.GetComponentInParent<Item>();
          if (!(bool) (Object) componentInParent1 || !((Object) componentInParent1 == (Object) Character.localCharacter.data.currentItem))
          {
            float num4 = Vector3.Angle(sphereCastResult.point - MainCamera.instance.transform.position, MainCamera.instance.transform.forward);
            if (flag && (double) num4 < (double) num2)
            {
              IInteractible componentInParent2 = sphereCastResult.collider.GetComponentInParent<IInteractible>();
              if (componentInParent2 != null && componentInParent2.IsInteractible(Character.localCharacter))
              {
                Item componentInParent3 = sphereCastResult.transform.GetComponentInParent<Item>();
                if (!(bool) (Object) componentInParent3 || !((Object) componentInParent3 == (Object) Character.localCharacter.data.currentItem))
                {
                  RaycastHit raycastHit3 = HelperFunctions.LineCheck(ray.origin, sphereCastResult.point, HelperFunctions.LayerType.TerrainMap, triggerInteraction: QueryTriggerInteraction.Collide);
                  if ((Object) raycastHit3.collider != (Object) null && raycastHit3.collider.GetComponentInParent<IInteractible>() != componentInParent2)
                  {
                    Debug.DrawLine(ray.origin, sphereCastResult.point, Color.red);
                  }
                  else
                  {
                    Debug.DrawLine(ray.origin, sphereCastResult.point, Color.green);
                    num2 = num4;
                    interactible = componentInParent2;
                  }
                }
              }
            }
          }
        }
      }
      interactableResult = interactible;
    }
  }

  private void CancelHeldInteract()
  {
    if (this.currentHeldInteractible != null)
      this.currentHeldInteractible.CancelCast(Character.localCharacter);
    this.currentInteractableHeldTime = 0.0f;
    this.currentHeldInteractible = (IInteractibleConstant) null;
  }
}
