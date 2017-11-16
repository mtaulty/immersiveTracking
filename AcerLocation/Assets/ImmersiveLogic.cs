// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using HoloToolkit.Unity.InputModule;
using UnityEngine;

namespace HoloToolkit.Sharing.Tests
{
    public class ImmersiveLogic : MonoBehaviour, ISpeechHandler
    {
        public void OnSpeechKeywordRecognized(SpeechEventData eventData)
        {
            // We wait for the speech keyword "mark" to say that the physical headset has
            // been lined up with the digital one displayed by HoloLens.
            if (!this.transmitting &&
                (string.Compare(eventData.RecognizedText, "mark", true) == 0))
            {
                // Make a new parent which is at the same place as the camera so that we
                // "store" that position which is common across coordinate systems
                this.newParent = new GameObject();
                this.newParent.transform.position = Camera.main.transform.position;
                this.newParent.transform.rotation = Camera.main.transform.rotation;
                this.newParent.transform.forward = Camera.main.transform.forward;

                this.transmitting = true;
            }
        }
        private void Update()
        {
            if (this.transmitting)
            {
                CustomMessages.Instance.SendRemoteHeadsetPosition(
                    this.newParent.transform.InverseTransformPoint(Camera.main.transform.localPosition),
                    this.newParent.transform.InverseTransformDirection(Camera.main.transform.forward));
            }
        }
        bool transmitting = false;
        GameObject newParent;
    }
}