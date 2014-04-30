using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    struct PinConnection
    {
        public int startPinId;

        public int endPinId;

        public PinConnection(int startPinId, int endPinId)
        {
            this.startPinId = startPinId;
            this.endPinId = endPinId;
        }

        public static bool operator ==(PinConnection c1, PinConnection c2)
        {
            if (c1.startPinId == c2.startPinId && c1.endPinId == c2.endPinId)
                return true;
            return false;
        }

        public static bool operator !=(PinConnection c1, PinConnection c2)
        {
            return !(c1 == c2);
        }

        public override bool Equals(Object o)
        {
            if (o is PinConnection)
            {
                PinConnection c =(PinConnection)o;
                if (startPinId == c.startPinId && endPinId == c.endPinId)
                    return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            int hashCode = 5;
            hashCode = unchecked(hashCode * 14 + startPinId);
            hashCode = unchecked(hashCode * 14 + endPinId);
            return hashCode;
        }

    }
}
