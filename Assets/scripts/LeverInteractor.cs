using UnityEngine;
using System.Collections;

public class LeverInteractor : MonoBehaviour
{
  public float InteractRadius = 0.125f;
  public Valve.VR.EVRButtonId InteractButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
  public LayerMask LayerFilter = -1;

  protected SteamVR_TrackedObject m_TrackedObject;

  protected SteamVR_Controller.Device m_Controller { get { return (m_TrackedObject ? SteamVR_Controller.Input((int)m_TrackedObject.index) : null); } }

  protected void Awake() {
    m_TrackedObject = GetComponent<SteamVR_TrackedObject>();
  }
  
  protected void Update() {
    if(m_Controller != null && m_Controller.GetPressDown(InteractButton)) {
      Collider[] colliders = Physics.OverlapSphere(this.transform.position, InteractRadius, LayerFilter, QueryTriggerInteraction.Collide);

      foreach(Collider found_object in colliders) {
	SimpleLever lever = found_object.GetComponentInParent<SimpleLever>();

	if(lever) {
	  lever.Impulse();
	}
      }
    }
  }
}
