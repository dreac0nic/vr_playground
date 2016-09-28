using UnityEngine;
using System.Collections;

public class VRInteractable : MonoBehaviour
{
  public bool InteractionEnabled = true;
  public bool PickupEnabled = true;

  protected VRInteractor m_Holder = null;
  
  public bool IsInteractable { get { return InteractionEnabled; } }
  public bool IsPickupable { get { return PickupEnabled; } }

  // TODO: Broadcast to object
  //       m_HeldInteractable.BroadcastMessage("Release", this, SendMessageOptions.DontRequireReceiver);
  public virtual void Interact(VRInteractor interactor) {
    if(IsInteractable) {
      Debug.Log(this.gameObject.name + " has been interacted with by " + interactor.gameObject.name);

      // TODO: Actually receive button presses.
    }
  }

  public virtual void Pickup(VRInteractor interactor) {
    if(IsInteractable && IsPickupable) {
      Debug.Log(this.gameObject.name + " has been picked up by " + interactor.gameObject.name);

      m_Holder = interactor;

      this.gameObject.BroadcastMessage("OnPickup", interactor, SendMessageOptions.DontRequireReceiver);
    }
  }

  public virtual void Release(VRInteractor interactor) {
    if(m_Holder == interactor) {
      Debug.Log(this.gameObject.name + " has been released by " + interactor.gameObject.name);
      
      m_Holder = null;

      this.gameObject.BroadcastMessage("OnRelease", interactor, SendMessageOptions.DontRequireReceiver);
    }
  }
}
