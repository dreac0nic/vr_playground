using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(VRInteractable))]
public class VRHoldable : MonoBehaviour
{
  private Rigidbody m_Rigidbody;
  private VRInteractable m_Interactable;

  public void Awake() {
    m_Rigidbody = GetComponent<Rigidbody>();
    m_Interactable = GetComponent<VRInteractable>();
  }

  public void Update() {
    if(m_Interactable.Holder) {
      this.transform.position = m_Interactable.Holder.transform.position;
      this.transform.rotation = m_Interactable.Holder.transform.rotation;
    }
  }

  public void OnPickup(VRInteractor new_interactor) {
    m_Rigidbody.isKinematic = true;
  }

  public void OnRelease(VRInteractor old_interactor) {
    m_Rigidbody.isKinematic = false;

    // Apply current force the interactor
    if(old_interactor.Controller != null) {
      m_Rigidbody.AddForce(old_interactor.Controller.velocity, ForceMode.VelocityChange);
      m_Rigidbody.AddTorque(old_interactor.Controller.angularVelocity, ForceMode.VelocityChange);
    }
  }
}
