﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSPE3Lib;

namespace KSP.E3.TableOfConnections
{
    class ConnectionInfoComparer : IComparer<ConnectionInfo>
    {
        private static NaturalSortingStringComparer comparer;

        static ConnectionInfoComparer()
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