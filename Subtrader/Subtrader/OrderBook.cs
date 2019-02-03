using System.Collections.Generic;
using Kaos.Collections;
using Subtrader.Models;

namespace Subtrader
{
    public class OrderBook
    {
        public const int ScaleFactor = 10_000;
        private readonly object _orderLock = new object();
        private long _orderId = 1;

        public OrderBook()
        {
            // Always want a single limit so we can have less checks in the Ask and Bid methods.
            Limits = new RankedSet<Limit>(new LimitPriceComparer()) { new Limit { Price = 1 * ScaleFactor } };
            Subscriptions = new Dictionary<long, Subscription>();
            Orders = new Dictionary<long, Order>();
        }

        private protected RankedSet<Limit> Limits { get; }
        private protected IDictionary<long, Subscription> Subscriptions { get; }
        private protected IDictionary<long, Order> Orders { get; }

        public Order Ask(long userId, long price, int shares)
        {
            if (userId <= 0 || price <= 0 || shares <= 0)
            {
                // TODO: Return a message or something.
                return null;
            }

            price = price * ScaleFactor;

            lock (_orderLock)
            {
                // Get the users subscription.
                if (!Subscriptions.TryGetValue(userId, out Subscription subscription))
                {
                    // TODO: Return a message or something.
                    return null;
                }

                var index = Limits.Count - 1;
                var originalShares = shares;
                while (index >= 0 && shares > 0)
                {
                    var currentLimit = Limits.ElementAt(index);
                    if (currentLimit.Price < price)
                    {
                        break;
                    }

                    Order order = currentLimit.BidHead;
                    while (order != null && shares > 0)
                    {
                        if (order.Subscription.UserId == userId)
                        {
                            if (order.Next == null)
                            {
                                break;
                            }
                            else
                            {
                                order = order.Next;
                            }
                        }

                        // Always assume the bid will have a subscription even if it's empty.
                        if (order.Shares >= shares)
                        {
                            order.Subscription.Owned += shares;
                            order.Shares -= shares;
                            shares = 0;
                        }
                        else
                        {
                            order.Subscription.Owned += order.Shares;
                            shares -= order.Shares;
                            order.Shares = 0;
                        }

                        order = order.Next;
                    }

                    index--;
                }

                if (shares > 0)
                {
                    subscription.Owned -= originalShares - shares;

                    var newOrder = new Order { Id = /*Interlocked.Increment(ref _orderId)*/_orderId++, Shares = shares, Subscription = subscription };

                    // At this point Limits is guaranteed to have a single Limit.
                    var prevLimit = Limits.ElementAt(index == Limits.Count - 1 ? index : ++index);
                    if (prevLimit.Price == price)
                    {
                        newOrder.ParentLimit = prevLimit;
                        if (prevLimit.AskHead == null)
                        {
                            prevLimit.AskHead = newOrder;
                        }
                        else
                        {
                            newOrder.Next = prevLimit.AskHead;
                            prevLimit.AskHead.Prev = newOrder;
                            prevLimit.AskHead = newOrder;
                        }
                    }
                    else
                    {
                        var newLimit = new Limit { AskHead = newOrder, Price = price };
                        newOrder.ParentLimit = newLimit;
                        Limits.Add(newLimit);
                    }

                    Orders.Add(newOrder.Id, newOrder);
                    return newOrder;
                }
                else
                {
                    subscription.Owned -= originalShares;
                }
            }

            return null;
        }

        public Order Bid(long userId, long price, int shares)
        {
            if (userId <= 0 || price <= 0 || shares <= 0)
            {
                return null;
            }

            price = price * ScaleFactor;

            lock (_orderLock)
            {
                var index = 0;
                var originalShares = shares;
                while (index < Limits.Count && shares > 0)
                {
                    var currentLimit = Limits.ElementAt(index);
                    if (currentLimit.Price > price)
                    {
                        break;
                    }

                    Order order = currentLimit.AskHead;
                    while (order != null && shares > 0)
                    {
                        if (order.Subscription.UserId == userId)
                        {
                            if (order.Next == null)
                            {
                                break;
                            }
                            else
                            {
                                order = order.Next;
                            }
                        }

                        if (order.Shares >= shares)
                        {
                            order.Subscription.Owned -= shares;
                            order.Shares -= shares;
                            shares = 0;
                        }
                        else
                        {
                            order.Subscription.Owned -= order.Shares;
                            shares -= order.Shares;
                            order.Shares = 0;
                        }

                        order = order.Next;
                    }

                    index++;
                }

                if (!Subscriptions.TryGetValue(userId, out Subscription subscription))
                {
                    subscription = new Subscription { Owned = 0, UserId = userId };
                    Subscriptions.Add(subscription.UserId, subscription);
                }

                if (shares > 0)
                {
                    subscription.Owned += originalShares - shares;

                    var newOrder = new Order { Id = /*Interlocked.Increment(ref _orderId)*/_orderId++, Shares = shares, Subscription = subscription };

                    // At this point Limits is guaranteed to have a single Limit.
                    var prevLimit = Limits.ElementAt(index == 0 ? 0 : --index);
                    if (prevLimit.Price == price)
                    {
                        newOrder.ParentLimit = prevLimit;
                        if (prevLimit.BidHead == null)
                        {
                            prevLimit.BidHead = newOrder;
                        }
                        else
                        {
                            newOrder.Next = prevLimit.BidHead;
                            prevLimit.BidHead.Prev = newOrder;
                            prevLimit.BidHead = newOrder;
                        }
                    }
                    else
                    {
                        var newLimit = new Limit { BidHead = newOrder, Price = price };
                        newOrder.ParentLimit = newLimit;
                        Limits.Add(newLimit);
                    }

                    Orders.Add(newOrder.Id, newOrder);
                    return newOrder;
                }
                else
                {
                    subscription.Owned += originalShares;
                }
            }

            return null;
        }

        public void Cancel(long orderId)
        {
            lock (_orderLock)
            {
                if (Orders.TryGetValue(orderId, out Order order))
                {
                    if (order.Prev != null)
                    {
                        order.Prev.Next = order.Next;
                    }

                    if (order.Next != null)
                    {
                        order.Next.Prev = order.Prev;
                    }

                    if (order.ParentLimit.AskHead == order)
                    {
                        order.ParentLimit.AskHead = order.Next;
                    }
                    else if (order.ParentLimit.BidHead == order)
                    {
                        order.ParentLimit.BidHead = order.Next;
                    }

                    Orders.Remove(orderId);
                }
            }
        }

        public void AddSubscription(Subscription subscription)
        {
            if (!Subscriptions.ContainsKey(subscription.UserId))
            {
                Subscriptions.Add(subscription.UserId, subscription);
            }
        }
    }
}
