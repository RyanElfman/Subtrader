using System.Diagnostics;

namespace Subtrader.Models
{
    [DebuggerDisplay("Id = {Id}, Shares = {Shares}, Next = {Next}, Prev = {Prev}, ParentLimit = {ParentLimit}, Subscription = {Subscription}")]
    public class Order
    {
        public long Id { get; set; }
        public int Shares { get; set; }
        public Order Next { get; set; }
        public Order Prev { get; set; }
        public Limit ParentLimit { get; set; }
        public Subscription Subscription { get; set; }
    }
}
