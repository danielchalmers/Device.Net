﻿using System;

#nullable enable

namespace Usb.Net.Android
{
    public interface IUsbPermissionBroadcastReceiver
    {
        bool? IsPermissionGranted { get; }
        event EventHandler Received;
        void Register();
    }
}