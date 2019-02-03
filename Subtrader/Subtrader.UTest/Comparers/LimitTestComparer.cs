using System.Collections;
using System.Collections.Generic;
using Subtrader.Models;

namespace Subtrader.UTest.Comparers
{
    public class LimitTestComparer : IComparer<Limit>, IComparer
    {
        private readonly OrderTestComparer _orderTestComparer;

        public LimitTestComparer() : this(null)
        { }

        public LimitTestComparer(OrderTestComparer orderTestComparer)
        {
            _orderTestComparer = orderTestComparer ?? new OrderTestComparer(this, new SubscriptionTestComparer());
        }

        public int Compare(Limit x, Limit y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var b = x.Price.CompareTo(y.Price);
            if (b != 0) return b;

            b = _orderTestComparer.Compare(x.AskHead, y.AskHead);
            if (b != 0) return b;

            b = _orderTestComparer.Compare(x.BidHead, y.BidHead);
            if (b != 0) return b;

            return 0;
        }

        public int Compare(object x, object y)
        {
            return Compare((Limit)x, (Limit)y);
        }
    }
}
