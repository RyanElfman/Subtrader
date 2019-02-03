using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtrader.Models;

namespace Subtrader.UTest
{
    public partial class OrderBookTests : OrderBook
    {
        [TestMethod]
        public void Bid_DoesNothing_With0PriceAndShares()
        {
            Bid(_userId1, 0, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_With0PriceAnd1Shares()
        {
            Bid(_userId1, 0, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_With1PriceAnd0Shares()
        {
            Bid(_userId1, 1, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_WithNegative1PriceAnd0Shares()
        {
            Bid(_userId1, -1, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_With0PriceAndNegative1Shares()
        {
            Bid(_userId1, 0, -1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_With0UserId()
        {
            Bid(0, 1, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Bid_DoesNothing_WithNegativeUserId()
        {
            Bid(-1, 1, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        [Ignore]
        public void Bid_SkipsSameUsersOrder_OnExistingLimit()
        {

        }

        [TestMethod]
        public void Bid_Adds1Order_OnExistingLimit_With1OrderFromTheSameUser()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 5, Subscription = new Subscription { Owned = 5, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;

            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);

            // Act.
            Bid(_userId1, 1, 5);

            // Assert. 
            var existingAskOrderClone = new Order { Id = 100, Shares = 5, Subscription = new Subscription { Owned = 5, UserId = _userId1 } };
            var order = new Order
            {
                Id = 1,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 5,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                BidHead = order,
                Price = 1 * ScaleFactor
            };

            order.ParentLimit = limit;
            existingAskOrderClone.ParentLimit = limit;
            limit.AskHead = existingAskOrderClone;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { limit }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrderClone, order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_Adds1OrderAndSubscription_OnExistingLimit_With0Orders()
        {
            // Act.
            Bid(_userId1, 1, 5);

            // Assert.
            var order = new Order
            {
                Id = 1,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 0,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                BidHead = order,
                Price = 1 * ScaleFactor
            };

            order.ParentLimit = limit;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { limit }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_Adds1OrderAndSubscription_OnExistingLimit_With0Orders_With1Subscription()
        {
            // Arrange.
            Subscriptions.Add(_userId1, new Subscription { Owned = 0, UserId = _userId1 });

            // Act.
            Bid(_userId1, 1, 5);

            // Assert.
            var order = new Order
            {
                Id = 1,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 0,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                BidHead = order,
                Price = 1 * ScaleFactor
            };

            order.ParentLimit = limit;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { limit }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_Adds1Limit_With1Order_When1LimitExists_ButPriceGreaterThanExistingLimit()
        {
            // Act.
            Bid(_userId1, 2, 1);

            // Assert.
            var order = new Order
            {
                Id = 1,
                Shares = 1,
                Subscription = new Subscription
                {
                    Owned = 0,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                BidHead = order,
                Price = 2 * ScaleFactor
            };

            order.ParentLimit = limit;

            Assert.AreEqual(2, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor }, limit }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_Adds1Limit_With1Order_When1LimitExists_ButPriceLessThanExistingLimit()
        {
            // Arrange.
            Limits.First().Price = 2 * ScaleFactor;

            // Act.
            Bid(_userId1, 1, 5);

            // Assert.
            var order = new Order
            {
                Id = 1,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 0,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                BidHead = order,
                Price = 1 * ScaleFactor
            };

            order.ParentLimit = limit;

            Assert.AreEqual(2, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { limit, new Limit { Price = 2 * ScaleFactor } }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_Adds1Order_OnExistingLimit_With1Order()
        {
            // Arrange.
            var existingOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingOrder;
            existingOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingOrder.Id, existingOrder);
            Subscriptions.Add(existingOrder.Subscription.UserId, existingOrder.Subscription);

            // Act.
            Bid(_userId1, 1, 5);

            // Assert.
            var existingOrder2 = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor };
            existingOrder2.ParentLimit = existingLimit2;

            var order = new Order
            {
                Id = 1,
                Next = existingOrder2,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 0,
                    UserId = _userId1
                }
            };

            existingLimit2.BidHead = order;
            existingOrder2.Prev = order;
            order.ParentLimit = existingLimit2;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingOrder2, order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingOrder2.Subscription, order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_FullyExecutes_AginstUser1Ask()
        {
            // Arrange.
            var existingAskOrder = new Order { Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);

            // Act.
            Bid(_userId2, 1, 100);

            // Assert.
            var existingAskOrder2 = new Order { Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrder2 };
            existingAskOrder2.ParentLimit = existingLimit2;

            var newBiddersSubscription = new Subscription { Owned = 100, UserId = _userId2 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrder2 }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrder2.Subscription, newBiddersSubscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_PartialExecutes_AginstUser1Ask_ButBidSharesIsLessThanAskShares()
        {
            // Arrange.
            var existingAskOrder = new Order { Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);

            // Act.
            Bid(_userId2, 1, 40);

            // Assert.
            var existingAskOrder2 = new Order { Shares = 60, Subscription = new Subscription { Owned = 60, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrder2 };
            existingAskOrder2.ParentLimit = existingLimit2;

            var newBiddersSubscription = new Subscription { Owned = 40, UserId = _userId2 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrder2 }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrder2.Subscription, newBiddersSubscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_PartialExecutes_AginstUser1Ask_ButBidSharesIsGreaterThanAskShares()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingAskOrder.Id, existingAskOrder);

            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);

            // Act.
            Bid(_userId2, 1, 200);

            // Assert.
            var existingAskOrder2 = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrder2 };
            existingAskOrder2.ParentLimit = existingLimit2;

            var newBiddersSubscription = new Subscription { Owned = 100, UserId = _userId2 };
            var remainingSellersOrder = new Order
            {
                Id = 1,
                ParentLimit = existingLimit2,
                Shares = 100,
                Subscription = newBiddersSubscription
            };

            existingLimit2.BidHead = remainingSellersOrder;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrder2, remainingSellersOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrder2.Subscription, newBiddersSubscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_FullyExecutes_1Limit3Asks()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingAskOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingAskOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };
            existingAskOrder.Next = existingAskOrder2;
            existingAskOrder2.Prev = existingAskOrder;
            existingAskOrder2.Next = existingAskOrder3;
            existingAskOrder3.Prev = existingAskOrder2;

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;
            existingAskOrder2.ParentLimit = existingLimit;
            existingAskOrder3.ParentLimit = existingLimit;

            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Orders.Add(existingAskOrder2.Id, existingAskOrder2);
            Orders.Add(existingAskOrder3.Id, existingAskOrder3);

            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);
            Subscriptions.Add(existingAskOrder2.Subscription.UserId, existingAskOrder2.Subscription);
            Subscriptions.Add(existingAskOrder3.Subscription.UserId, existingAskOrder3.Subscription);

            // Act.
            Bid(_userId4, 1, 300);

            // Assert.
            var existingAskOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingAskOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingAskOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };
            existingAskOrderClone.Next = existingAskOrder2Clone;
            existingAskOrder2Clone.Prev = existingAskOrder;
            existingAskOrder2Clone.Next = existingAskOrder3Clone;
            existingAskOrder3Clone.Prev = existingAskOrder2Clone;

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrderClone };
            existingAskOrderClone.ParentLimit = existingLimitClone;
            existingAskOrder2Clone.ParentLimit = existingLimitClone;
            existingAskOrder3Clone.ParentLimit = existingLimitClone;

            var newBidderSubscription = new Subscription { Owned = 300, UserId = _userId4 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrderClone, existingAskOrder2Clone, existingAskOrder3Clone }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrderClone.Subscription, existingAskOrder2Clone.Subscription, existingAskOrder3Clone.Subscription, newBidderSubscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_PartialExecutes_1Limit3Bids()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingAskOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingAskOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };
            existingAskOrder.Next = existingAskOrder2;
            existingAskOrder2.Prev = existingAskOrder;
            existingAskOrder2.Next = existingAskOrder3;
            existingAskOrder3.Prev = existingAskOrder2;

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingAskOrder;
            existingAskOrder.ParentLimit = existingLimit;
            existingAskOrder2.ParentLimit = existingLimit;
            existingAskOrder3.ParentLimit = existingLimit;

            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Orders.Add(existingAskOrder2.Id, existingAskOrder2);
            Orders.Add(existingAskOrder3.Id, existingAskOrder3);

            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);
            Subscriptions.Add(existingAskOrder2.Subscription.UserId, existingAskOrder2.Subscription);
            Subscriptions.Add(existingAskOrder3.Subscription.UserId, existingAskOrder3.Subscription);

            // Act.
            Bid(_userId4, 1, 400);

            // Assert.
            var existingAskOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingAskOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingAskOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };
            existingAskOrderClone.Next = existingAskOrder2Clone;
            existingAskOrder2Clone.Prev = existingAskOrder;
            existingAskOrder2Clone.Next = existingAskOrder3Clone;
            existingAskOrder3Clone.Prev = existingAskOrder2Clone;

            var newBidOrder = new Order { Id = 1, Shares = 100, Subscription = new Subscription { Owned = 300, UserId = _userId4 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, BidHead = newBidOrder, AskHead = existingAskOrderClone };
            existingAskOrderClone.ParentLimit = existingLimitClone;
            existingAskOrder2Clone.ParentLimit = existingLimitClone;
            existingAskOrder3Clone.ParentLimit = existingLimitClone;
            newBidOrder.ParentLimit = existingLimitClone;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(4, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrderClone, existingAskOrder2Clone, existingAskOrder3Clone, newBidOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrderClone.Subscription, existingAskOrder2Clone.Subscription, existingAskOrder3Clone.Subscription, newBidOrder.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_FullyExecutes_3Limits3Bids1OnEachLimit()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingAskOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingAskOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };

            var existingLimit = Limits.First();
            var limit2 = new Limit { Price = 2 * ScaleFactor };
            var limit3 = new Limit { Price = 3 * ScaleFactor };

            existingLimit.AskHead = existingAskOrder;
            limit2.AskHead = existingAskOrder2;
            limit3.AskHead = existingAskOrder3;

            existingAskOrder.ParentLimit = existingLimit;
            existingAskOrder2.ParentLimit = limit2;
            existingAskOrder3.ParentLimit = limit3;

            Limits.Add(limit2);
            Limits.Add(limit3);
            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Orders.Add(existingAskOrder2.Id, existingAskOrder2);
            Orders.Add(existingAskOrder3.Id, existingAskOrder3);

            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);
            Subscriptions.Add(existingAskOrder2.Subscription.UserId, existingAskOrder2.Subscription);
            Subscriptions.Add(existingAskOrder3.Subscription.UserId, existingAskOrder3.Subscription);

            // Act.
            Bid(_userId4, 3, 300);

            // Assert.
            var existingAskOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingAskOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingAskOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrderClone };
            var existingLimit2Clone = new Limit { Price = 2 * ScaleFactor, AskHead = existingAskOrder2Clone };
            var existingLimit3Clone = new Limit { Price = 3 * ScaleFactor, AskHead = existingAskOrder3Clone };
            existingAskOrderClone.ParentLimit = existingLimitClone;
            existingAskOrder2Clone.ParentLimit = existingLimit2Clone;
            existingAskOrder3Clone.ParentLimit = existingLimit3Clone;

            var newBidSubscription = new Subscription { Owned = 300, UserId = _userId4 };

            Assert.AreEqual(3, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone, limit2, limit3 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrderClone, existingAskOrder2Clone, existingAskOrder3Clone }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrderClone.Subscription, existingAskOrder2Clone.Subscription, existingAskOrder3Clone.Subscription, newBidSubscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Bid_PartialExecutes_3Limits3Bids1OnEachLimit()
        {
            // Arrange.
            var existingAskOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingAskOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingAskOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };

            var existingLimit = Limits.First();
            var limit2 = new Limit { Price = 2 * ScaleFactor };
            var limit3 = new Limit { Price = 3 * ScaleFactor };

            existingLimit.AskHead = existingAskOrder;
            limit2.AskHead = existingAskOrder2;
            limit3.AskHead = existingAskOrder3;

            existingAskOrder.ParentLimit = existingLimit;
            existingAskOrder2.ParentLimit = limit2;
            existingAskOrder3.ParentLimit = limit3;

            Limits.Add(limit2);
            Limits.Add(limit3);
            Orders.Add(existingAskOrder.Id, existingAskOrder);
            Orders.Add(existingAskOrder2.Id, existingAskOrder2);
            Orders.Add(existingAskOrder3.Id, existingAskOrder3);

            Subscriptions.Add(existingAskOrder.Subscription.UserId, existingAskOrder.Subscription);
            Subscriptions.Add(existingAskOrder2.Subscription.UserId, existingAskOrder2.Subscription);
            Subscriptions.Add(existingAskOrder3.Subscription.UserId, existingAskOrder3.Subscription);

            // Act.
            Bid(_userId4, 3, 400);

            // Assert.
            var existingAskOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingAskOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingAskOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };

            var newAskOrder = new Order { Id = 1, Shares = 100, Subscription = new Subscription { Owned = 300, UserId = _userId4 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, AskHead = existingAskOrderClone };
            var existingLimit2Clone = new Limit { Price = 2 * ScaleFactor, AskHead = existingAskOrder2Clone };
            var existingLimit3Clone = new Limit { Price = 3 * ScaleFactor, BidHead = newAskOrder, AskHead = existingAskOrder3Clone };
            existingAskOrderClone.ParentLimit = existingLimitClone;
            existingAskOrder2Clone.ParentLimit = existingLimit2Clone;
            existingAskOrder3Clone.ParentLimit = existingLimit3Clone;
            newAskOrder.ParentLimit = existingLimitClone;

            Assert.AreEqual(3, Limits.Count);
            Assert.AreEqual(4, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone, existingLimit2Clone, existingLimit3Clone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingAskOrderClone, existingAskOrder2Clone, existingAskOrder3Clone, newAskOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingAskOrderClone.Subscription, existingAskOrder2Clone.Subscription, existingAskOrder3Clone.Subscription, newAskOrder.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }
    }
}
