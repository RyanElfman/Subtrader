using System.Collections;
using System.Collections.Generic;
using Subtrader.Models;

namespace Subtrader.UTest.Comparers
{
    public class SubscriptionTestComparer : IComparer<Subscription>, IComparer
    {
        public int Compare(Subscription x, Subscription y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var t = x.Owned.CompareTo(y.Owned);
            if (t != 0) return t;

            t = x.UserId.CompareTo(y.UserId);
            if (t != 0) return t;

            return 0;
        }

        public int Compare(object x, object y)
        {
            return Compare((Subscription)x, (Subscription)y);
        }
    }
}
