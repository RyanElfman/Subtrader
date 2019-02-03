using System;
using System.Diagnostics;

namespace Subtrader.Models
{
    [DebuggerDisplay("Price = {Price}, AskHead = {AskHead}, BidHead = {BidHead}")]
    [Serializable]
    public class Limit
    {
        public long Price { get; set; }
        public Order AskHead { get; set; }
        public Order BidHead { get; set; }
    }
}
