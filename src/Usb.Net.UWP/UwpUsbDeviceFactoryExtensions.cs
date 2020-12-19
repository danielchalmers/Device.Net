﻿using Device.Net;
using Device.Net.UWP;
using Device.Net.Windows;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Usb.Net.UWP
{
    public static class UwpUsbDeviceFactoryExtensions
    {

        public static IDeviceFactory CreateUwpUsbDeviceFactory(
            this FilterDeviceDefinition filterDeviceDefinitions,
            ILoggerFactory loggerFactory = null,
            GetConnectedDeviceDefinitionsAsync getConnectedDeviceDefinitionsAsync = null,
            GetUsbInterfaceManager getUsbInterfaceManager = null,
            ushort? readBufferSize = null,
            ushort? writeBufferSize = null
            )
        {
            return CreateUwpUsbDeviceFactory(
                new List<FilterDeviceDefinition> { filterDeviceDefinitions },
                loggerFactory,
                getConnectedDeviceDefinitionsAsync,
                getUsbInterfaceManager,
                readBufferSize,
                writeBufferSize);
        }

        public static IDeviceFactory CreateUwpUsbDeviceFactory(
        this IEnumerable<FilterDeviceDefinition> filterDeviceDefinitions,
        ILoggerFactory loggerFactory,
        GetConnectedDeviceDefinitionsAsync getConnectedDeviceDefinitionsAsync = null,
        GetUsbInterfaceManager getUsbInterfaceManager = null,
        ushort? readBufferSize = null,
        ushort? writeBufferSize = null
        )
        {
            var firstDevice = filterDeviceDefinitions.First();

            if (getConnectedDeviceDefinitionsAsync == null)
            {
                var uwpHidDeviceEnumerator = new UwpDeviceEnumerator(
                    AqsHelpers.GetAqs(filterDeviceDefinitions, DeviceType.Usb),
                    DeviceType.Usb,
                    (d, cancellationToken) => Task.FromResult(new ConnectionInfo { CanConnect = true }),
                    loggerFactory);

                getConnectedDeviceDefinitionsAsync = uwpHidDeviceEnumerator.GetConnectedDeviceDefinitionsAsync;
            }

            getUsbInterfaceManager ??= (deviceId, cancellationToken) =>
                Task.FromResult<IUsbInterfaceManager>(
                    new UWPUsbInterfaceManager(
                        //TODO: no idea if this is OK...
                        new ConnectedDeviceDefinition(deviceId, DeviceType.Usb),
                        loggerFactory,
                        readBufferSize,
                        writeBufferSize));

            return UsbDeviceFactoryExtensions.CreateUsbDeviceFactory(getConnectedDeviceDefinitionsAsync, getUsbInterfaceManager, loggerFactory);
        }
    }
}
