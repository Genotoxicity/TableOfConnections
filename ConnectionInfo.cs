using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    struct ConnectionInfo
    {
        public string conductorName;
        public string signal;
        public string assignmentFrom;
        public string locationFrom;
        public string deviceFrom;
        public string pinFrom;
        public string assignmentTo;
        public string locationTo;
        public string deviceTo;
        public string pinTo;
        public string type;

        public ConnectionInfo(Core core, NormalDevice device, DevicePin devicePin, string conductorName, string type)
        {
            this.conductorName = conductorName;
            signal = String.Intern(core.SignalName);
            int startPinId = core.StartPinId;
            int endPinId = core.EndPinId;
            devicePin.Id = startPinId;
            devicePin.Id = startPinId;
            assignmentFrom = String.Intern(device.Assignment);  // интернируем строку, поскольку 1) часто повторяется 2) нужно будет много сравнивать
            locationFrom = device.Location;
            deviceFrom = device.Name;
            pinFrom = String.Intern(devicePin.Name);
            device.Id = endPinId;
            devicePin.Id = endPinId;
            assignmentTo = String.Intern(device.Assignment);
            locationTo = device.Location;
            deviceTo = device.Name;
            pinTo = String.Intern(devicePin.Name);
            this.type = String.Intern(type);
        }

        public ConnectionInfo(int startPinId, int endPinId, string signal, NormalDevice device, DevicePin devicePin, Core core, WireCore wire) 
        {
            devicePin.Id = startPinId;
            pinFrom = String.Intern(devicePin.Name);
            this.signal = signal;
            List<int> startPinCoreIds = devicePin.CoreIds;
            device.Id = startPinId;
            deviceFrom = device.Name;
            assignmentFrom = String.Intern(device.Assignment);
            locationFrom = device.Location;
            devicePin.Id = endPinId;
            pinTo = String.Intern(devicePin.Name);
            List<int> endPinCoreIds = devicePin.CoreIds;
            device.Id = endPinId;
            deviceTo = device.Name;
            assignmentTo = String.Intern(device.Assignment);
            locationTo = device.Location;
            conductorName = String.Empty;
            type = String.Empty;
            if (startPinCoreIds.Count > 0 && endPinCoreIds.Count > 0)
            {
                List<int> intersection = startPinCoreIds.Intersect<int>(endPinCoreIds).ToList<int>();
                if (intersection.Count == 1)
                {
                    core.Id = intersection[0];
                    device.Id = intersection[0];
                    if (device.IsCable())
                    {
                        conductorName = device.Name;
                        type = String.Intern(device.ComponentName);
                    }
                    if (device.IsWireGroup())
                    {
                        wire.Id = intersection[0];
                        conductorName = wire.Name;
                        type = String.Intern(wire.WireType);
                    }
                }
            }
        }
    }
}
