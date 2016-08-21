using UnityEngine;
using System.Collections;

public class BallKiller : MonoBehaviour
{
  public float KillHeight = -1000.0f;
  
  protected void Update() {
    if(this.transform.position.y < KillHeight) {
      GameObject.Destroy(this.gameObject);
    }
  }
}
