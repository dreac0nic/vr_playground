using UnityEngine;
using System.Collections;

[RequireComponent(typeof(VRInteractable))]
public class LeverInteractable : MonoBehaviour
{
  public Transform LeverPivot;

  private VRInteractable m_Interactable;

  public void Awake() {
    m_Interactable = GetComponent<VRInteractable>();
  }

  public void Update() {
    if(m_Interactable.Holder) {
      LeverPivot.LookAt(m_Interactable.Holder);
    }
  }
}
