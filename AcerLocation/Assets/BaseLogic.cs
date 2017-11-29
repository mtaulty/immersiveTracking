using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Tests;

public class BaseLogic : MonoBehaviour
{
    public GameObject headsetPrefab;

    public BaseLogic(CustomMessages.TestMessageID sendMessageId, CustomMessages.TestMessageID receiveMessageId)
    {
        this.sendMessageId = sendMessageId;
        this.receiveMessageId = receiveMessageId;
    }
    protected void CreateGlasses(Vector3 position, Transform parent)
    {
        if (this.GlassesObject == null)
        {
            this.GlassesObject = (GameObject)GameObject.Instantiate(this.headsetPrefab);
            this.GlassesObject.transform.position = position;
            this.GlassesObject.transform.SetParent(parent, true);
        }
    }
    protected void CreateAnchoredParent(Vector3 position, Quaternion rotation, Transform parent)
    {
        if (this.AnchoredParent == null)
        {
            this.AnchoredParent = new GameObject();
            this.AnchoredParent.transform.position = position;
            this.AnchoredParent.transform.rotation = rotation;
            this.AnchoredParent.transform.SetParent(parent, true);
        }
    }
    protected void SendPositionRelativeToAnchorMessage(Vector3 position, Vector3 direction)
    {
        CustomMessages.Instance.SendRemotePositionDirection(
            (byte)this.sendMessageId,
            this.AnchoredParent.transform.InverseTransformPoint(position), 
            this.AnchoredParent.transform.InverseTransformDirection(direction));
    }
    protected GameObject AnchoredParent
    {
        get; set;
    }
    protected GameObject GlassesObject
    {
        get;set;
    }
    protected bool IsReadyToSendReceive
    {
        get
        {
            return (this.isReadyToSendReceive);
        }
    }
    protected void SwitchToSendingAndReceiving()
    {
        if (!this.isReadyToSendReceive)
        {
            this.isReadyToSendReceive = true;
            CustomMessages.Instance.MessageHandlers[this.receiveMessageId] = OnPositionReceived;
        }
    }
    void OnPositionReceived(NetworkInMessage msg)
    {
        long userID = msg.ReadInt64();
        Vector3 position = CustomMessages.Instance.ReadVector3(msg);
        Vector3 direction = CustomMessages.Instance.ReadVector3(msg);
        this.OnPositionMessage(position, direction);
    }
    protected virtual void OnPositionMessage(Vector3 position, Vector3 direction)
    {
        Debug.Log(
            string.Format("Received position {0},{1},{2}", position.x, position.y, position.z));

        this.GlassesObject.transform.localPosition = position;
        this.GlassesObject.transform.forward = direction;
    }
    void Update()
    {
        if (this.IsReadyToSendReceive)
        {
            this.SendPositionRelativeToAnchorMessage(
                Camera.main.transform.localPosition,
                Camera.main.transform.forward);
        }
    }
    CustomMessages.TestMessageID sendMessageId;
    CustomMessages.TestMessageID receiveMessageId;
    bool isReadyToSendReceive;
}
