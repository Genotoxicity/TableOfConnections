using System.Collections.Generic;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    class ConnectionInfoComparer : IComparer<ConnectionInfo>
    {
        private NaturalSortingStringComparer comparer;

        public ConnectionInfoComparer()
        {
            comparer = new NaturalSortingStringComparer();
        }

        public int Compare(ConnectionInfo a, ConnectionInfo b)
        {
            int result = comparer.Compare(a.signal, b.signal);
            if (result != 0)
                return result;
            result = comparer.Compare(a.deviceFrom, b.deviceFrom);
            if (result != 0)
                return result;
            result = comparer.Compare(a.pinFrom, b.pinFrom);
            return result;
        }
    }
}
