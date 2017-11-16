using HoloToolkit.Sharing;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class Logic : MonoBehaviour
{
  // Update is called once per frame
  void Update()
  {
    if (!this.sharingInitialised &&
      (SharingStage.Instance != null) &&
      !string.IsNullOrEmpty(SharingStage.Instance.RoomName) &&
      (MixedRealityCameraManager.Instance != null))
    {
      this.sharingInitialised = true;
      var childName = "HoloLens";

      if (MixedRealityCameraManager.Instance.CurrentDisplayType == MixedRealityCameraManager.DisplayType.Opaque)
      {
        childName = "Immersive";
      }
      var child = this.gameObject.transform.Find(childName);
      child.gameObject.SetActive(true);
    }
  }
  bool sharingInitialised;
}
