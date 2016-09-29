using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VRInteractable))]
public class ButtonInteractable : MonoBeviour
{
  public bool ToggleSwitch = false;

  protected bool m_State = false;

  private VRInteractable m_Interactable;

  public void Awake() {
    m_Interactable = GetComponent<VRInteractable>();
  }

  public void Update() {
    if(!ToggleSwitch && m_State) {
      if(m_Interactable.Holder) {
	// TODO: Check if Holder interact 
      }
    }
  }
  
  public void OnInteract(VRInteractor interactor) {
    if(ToggleState) {
      m_State = !m_State;
    } else {
      m_State = true;
    }
  }
}
