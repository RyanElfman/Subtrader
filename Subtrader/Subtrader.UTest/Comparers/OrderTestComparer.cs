using System;
using System.Collections;
using System.Collections.Generic;
using Subtrader.Models;

namespace Subtrader.UTest.Comparers
{
    public class OrderTestComparer : IComparer<Order>, IComparer
    {
        private readonly LimitTestComparer _limitTestComparer;
        private readonly SubscriptionTestComparer _subscriptionTestComparer;
        private readonly HashSet<Tuple<string, object>> _visited;

        public OrderTestComparer() : this(null, null)
        { }

        public OrderTestComparer(LimitTestComparer limitTestComparer,
            SubscriptionTestComparer subscriptionTestComparer)
        {
            _limitTestComparer = limitTestComparer ?? new LimitTestComparer(this);
            _subscriptionTestComparer = subscriptionTestComparer ?? new SubscriptionTestComparer();
            _visited = new HashSet<Tuple<string, object>>();
        }

        public int Compare(Order x, Order y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;

            var b = x.Id.CompareTo(y.Id);
            if (b != 0) return b;

            b = x.Shares.CompareTo(y.Shares);
            if (b != 0) return b;

            b = _subscriptionTestComparer.Compare(x.Subscription, y.Subscription);
            if (b != 0) return b;

            b = Compare(nameof(Order.Next), x.Next, y.Next);
            if (b != 0) return b;

            b = Compare(nameof(Order.Prev), x.Prev, y.Prev);
            if (b != 0) return b;

            b = Compare(nameof(Order.ParentLimit), x.ParentLimit, y.ParentLimit);
            if (b != 0) return b;

            return 0;
        }

        public int Compare(object x, object y)
        {
            return Compare((Order)x, (Order)y);
        }

        private int Compare<T>(string path, T x, T y)
        {
            var xp = Tuple.Create(path, (object)x);
            var yp = Tuple.Create(path, (object)y);
            if (!_visited.Contains(xp) || !_visited.Contains(yp))
            {
                if (x != null)
                    _visited.Add(xp);

                if (y != null)
                    _visited.Add(yp);

                int n = 0;
                if (typeof(T) == typeof(Order))
                {
                    n = Compare(x, y);
                }
                else if (typeof(T) == typeof(Limit))
                {
                    n = _limitTestComparer.Compare(x, y);
                }
                else
                {
                    throw new Exception("The type being compared is not supported.");
                }

                if (n != 0) return n;
            }

            return 0;
        }
    }
}
