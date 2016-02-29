/*==============================================================================
Copyright (c) 2012-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// This MonoBehaviour manages the usage of a webcam for Play Mode in Windows or Mac.
    /// </summary>
    public class WebCamBehaviour : WebCamAbstractBehaviour
    {
        public void Awake()
        {
            ////making sure it's not trying to use the leap camera
            //WebCamDevice[] devices = WebCamTexture.devices;
            //string n = "";
            //foreach (WebCamDevice d in devices)
            //    if (d.name != "Leap Motion Controller")
            //        n = d.name;
            //DeviceName = n;
        }
        [ContextMenu("Log Device name")]
        public void LogDeviceName()
        {
            Debug.Log("Device name: " + DeviceName);
        }

        [ContextMenu("Log Webcam Devices")]
        public void LogWebcamDevices()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            foreach(WebCamDevice d in devices)
                Debug.Log("Device name: " + d.name);
        }
    }
}
