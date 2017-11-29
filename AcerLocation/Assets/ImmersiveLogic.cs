// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Sharing.Tests
{
    public class ImmersiveLogic : BaseLogic, ISpeechHandler
    {
        public ImmersiveLogic() : base(
            CustomMessages.TestMessageID.RemoteHeadsetPosition,
            CustomMessages.TestMessageID.RemoteHoloLensPosition)
        {

        }
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            // We wait for the speech keyword "mark" to say that the physical headset has
            // been lined up with the digital one displayed by HoloLens.
            if (!base.IsReadyToSendReceive &&
                (string.Compare(eventData.RecognizedText, "mark", true) == 0))
            {
                Debug.Log("Got speech keyword");

                base.CreateAnchoredParent(
                    Camera.main.transform.position, 
                    Camera.main.transform.rotation, 
                    null);

                base.CreateGlasses(
                    Vector3.zero, base.AnchoredParent.transform);

                base.SwitchToSendingAndReceiving();
            }
        }
    }
}