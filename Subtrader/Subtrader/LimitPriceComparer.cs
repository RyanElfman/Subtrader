using System.Collections.Generic;
using Subtrader.Models;

namespace Subtrader
{
    public class LimitPriceComparer : IComparer<Limit>
    {
        public int Compare(Limit x, Limit y)
        {
            return x.Price.CompareTo(y.Price);
        }
    }
}
