using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class HoloLensLogic : BaseLogic, IInputClickHandler
{
    public HoloLensLogic() : base(
        CustomMessages.TestMessageID.RemoteHoloLensPosition,
        CustomMessages.TestMessageID.RemoteHeadsetPosition)
    {
    }
    void Start()
    {
        base.CreateGlasses(
            new Vector3(0, 0, ACTIVATE_DISTANCE),
            Camera.main.transform);

        // Register ourselves as a click handler - we don't have a collider on
        // the glasses.
        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (!base.IsReadyToSendReceive)
        {
            // Create a new game object at this position where the glasses are
            base.CreateAnchoredParent(
                base.GlassesObject.transform.position,
                base.GlassesObject.transform.rotation,
                null);

            // Reparent the glasses off that object so that future movements can
            // be *relative* to that object.
            base.GlassesObject.transform.SetParent(base.AnchoredParent.transform, true);

            // We're now ready to handle remote position messages which will be
            // relative to a coordinate system matched up with the one that
            // our newParent object has.
            base.SwitchToSendingAndReceiving();
        }
    }
    static readonly float ACTIVATE_DISTANCE = 1.5f;
}