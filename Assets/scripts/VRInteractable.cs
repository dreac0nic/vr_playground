using UnityEngine;
using System.Collections;

public class VRInteractable : MonoBehaviour
{
  public bool InteractionEnabled = true;
  
  public virtual bool IsInteractable(VRInteractor interactor) {
    return InteractionEnabled;
  }

  public virtual void Interact(VRInteractor interactor) {
    Debug.Log(this.gameObject.name + " has been interacted with by " + interactor.gameObject.name);
  }
}
