using UnityEngine;
using System.Collections;

public class VRInteractable : MonoBehaviour
{
  public bool InteractionEnabled = true;
  public bool PickupEnabled = true;

  protected VRInteractor m_Holder = null;
  
  public bool IsInteractable { get { return InteractionEnabled; } }
  public bool IsPickupable { get { return PickupEnabled; } }

  public virtual void Interact(VRInteractor interactor) {
    if(IsInteractable) {
      Debug.Log(this.gameObject.name + " has been interacted with by " + interactor.gameObject.name);
      
      if(IsPickupable) {
	m_Holder = interactor;
      }
    }
  }

  public virtual void Release(VRInteractor interactor) {
    Debug.Log(this.gameObject.name + " has been released by " + interactor.gameObject.name);

    if(m_Holder == interactor) {
      m_Holder = null;
    }
  }
}
