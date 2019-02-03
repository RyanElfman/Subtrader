using System.Diagnostics;

namespace Subtrader.Models
{
    [DebuggerDisplay("UserId = {UserId}, Owned = {Owned}")]
    public class Subscription
    {
        public long UserId { get; set; }
        public int Owned { get; set; }
    }
}
