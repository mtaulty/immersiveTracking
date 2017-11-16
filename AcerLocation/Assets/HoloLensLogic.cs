using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity.InputModule;
using UnityEngine;

public class HoloLensLogic : MonoBehaviour, IInputClickHandler
{
    enum State
    {
        WaitingForPosition,
        ListeningForRemoteTransforms
    }
    public bool useAxisMarker = false;
    public GameObject headsetPrefab;
    public GameObject axisMarkerPrefab;

    void Start()
    {
        this.currentState = State.WaitingForPosition;

        // Create our headset prefab (the pair of glasses) and put them around 1.5m
        // in front of the user.
        this.headsetInstance = (GameObject)GameObject.Instantiate(this.headsetPrefab);
        this.headsetInstance.transform.position = new Vector3(0, 0, ACTIVATE_DISTANCE);
        this.headsetInstance.transform.SetParent(Camera.main.transform, true);

        // Register ourselves as a click handler - we don't have a collider on
        // the glasses.
        InputManager.Instance.PushFallbackInputHandler(this.gameObject);
    }
    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (this.currentState == State.WaitingForPosition)
        {
            this.currentState = State.ListeningForRemoteTransforms;

            // Create a new game object at this position where the glasses are
            var newParent = new GameObject();
            newParent.transform.position = this.headsetInstance.transform.position;
            newParent.transform.rotation = this.headsetInstance.transform.rotation;

            // Reparent the glasses off that object so that future movements can
            // be *relative* to that object.
            this.headsetInstance.transform.SetParent(newParent.transform, true);

            // Create a marker for the axes as a debugging aid.
            if (this.useAxisMarker && (this.axisMarkerPrefab != null))
            {
                var axisMarker = (GameObject)GameObject.Instantiate(this.axisMarkerPrefab);
                axisMarker.transform.SetParent(newParent.transform, false);
            }
            // We're now ready to handle remote position messages which will be
            // relative to a coordinate system matched up with the one that
            // our newParent object has.
            CustomMessages.Instance.MessageHandlers[CustomMessages.TestMessageID.RemoteHeadsetPosition] =
                OnHeadTransformMessageReceived;
        }
    }
    void OnHeadTransformMessageReceived(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();
        Vector3 headPos = CustomMessages.Instance.ReadVector3(msg);
        Vector3 headForward = CustomMessages.Instance.ReadVector3(msg);

        if (this.currentState == State.ListeningForRemoteTransforms)
        {
            // Move the glasses relative to their parent (which is sync'd
            // with the remote coordinate system).
            this.headsetInstance.transform.localPosition = headPos;
            this.headsetInstance.transform.forward = headForward;
        }
    }
    State currentState;
    GameObject headsetInstance;
    static readonly float ACTIVATE_DISTANCE = 1.5f;
}