using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    class ConnectionInfo
    {
        public string name;
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

        public ConnectionInfo(Project project, Core core, string name, string type)
        {
            Device deviceFrom = project.GetDeviceById(core.FirstEndPinId);
            Device deviceTo = project.GetDeviceById(core.SecondEndPinId);
            Pin pinFrom = project.GetPinById(core.FirstEndPinId);
            Pin pinTo = project.GetPinById(core.SecondEndPinId);
            this.name = name;
            signal = core.SignalName;
            assignmentFrom = String.Intern(deviceFrom.Assignment);  // интернируем строку, поскольку 1) часто повторяется 2) нужно будет много сравнивать
            locationFrom = deviceFrom.Location;
            this.deviceFrom = deviceFrom.Name;
            this.pinFrom = pinFrom.Name;
            assignmentTo = String.Intern(deviceTo.Assignment);
            locationTo = deviceTo.Location;
            this.deviceTo = deviceTo.Name;
            this.pinTo = pinTo.Name;
            this.type = String.Intern(type);
        }
    }
}
