using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtrader.Models;

namespace Subtrader.UTest
{
    public partial class OrderBookTests : OrderBook
    {
        [TestMethod]
        public void Ask_DoesNothing_With0PriceAndShares()
        {
            Ask(_userId1, 0, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_With0PriceAnd1Shares()
        {
            Ask(_userId1, 0, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_With1PriceAnd0Shares()
        {
            Ask(_userId1, 1, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_WithNegative1PriceAnd0Shares()
        {
            Ask(_userId1, -1, 0);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_With0PriceAndNegative1Shares()
        {
            Ask(_userId1, 0, -1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_With0lUserId()
        {
            Ask(0, 1, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_WithNegativelUserId()
        {
            Ask(-1, 1, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        public void Ask_DoesNothing_WithNoSubscription()
        {
            Ask(_userId1, 1, 1);
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }

        [TestMethod]
        [Ignore]
        public void Ask_SkipsSameUsersOrder_OnExistingLimit()
        {

        }

        [TestMethod]
        public void Ask_Adds1Order_OnExistingLimit_With1OrderFromTheSameUser()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 100, Shares = 5, Subscription = new Subscription { Owned = 5, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;

            Orders.Add(existingBidOrder.Id, existingBidOrder);
            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);

            // Act.
            Ask(_userId1, 1, 5);

            // Assert. 
            var existingBidOrderClone = new Order { Id = 100, Shares = 5, Subscription = new Subscription { Owned = 5, UserId = _userId1 } };
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
                AskHead = order,
                Price = 1 * ScaleFactor
            };

            order.ParentLimit = limit;
            existingBidOrderClone.ParentLimit = limit;
            limit.BidHead = existingBidOrderClone;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { limit }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrderClone, order }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { order.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_Adds1Order_OnExistingLimit_With0Orders()
        {
            // Arrange.
            Subscriptions.Add(_userId1, new Subscription { Owned = 5, UserId = _userId1 });

            // Act.
            Ask(_userId1, 1, 5);

            // Assert. 
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
                AskHead = order,
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
        public void Ask_Adds1Limit_With1Order_When1LimitExists_ButPriceGreaterThanExistingLimit()
        {
            // Arrange.
            var sub = new Subscription { Owned = 5, UserId = _userId1 };
            Subscriptions.Add(sub.UserId, sub);

            // Act.
            Ask(_userId1, 2, 1);

            // Assert.
            var order = new Order
            {
                Id = 1,
                Shares = 1,
                Subscription = new Subscription
                {
                    Owned = 5,
                    UserId = _userId1
                }
            };

            var limit = new Limit
            {
                AskHead = order,
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
        public void Ask_Adds1Limit_With1Order_When1LimitExists_ButPriceLessThanExistingLimit()
        {
            // Arrange.
            Limits.First().Price = 2 * ScaleFactor;
            Subscriptions.Add(_userId1, new Subscription { Owned = 5, UserId = _userId1 });

            // Act.
            Ask(_userId1, 1, 5);

            // Assert.
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
                AskHead = order,
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
        public void Ask_Adds1Order_OnExistingLimit_With1Order()
        {
            // Arrange.
            var existingOrder = new Order { Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };

            var existingLimit = Limits.First();
            existingLimit.AskHead = existingOrder;
            existingOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingOrder.Id, existingOrder);
            Subscriptions.Add(existingOrder.Subscription.UserId, existingOrder.Subscription);
            Subscriptions.Add(_userId1, new Subscription { Owned = 5, UserId = _userId1 });

            // Act.
            Ask(_userId1, 1, 5);

            // Assert.
            var existingOrder2 = new Order { Shares = 100, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor };
            existingOrder2.ParentLimit = existingLimit2;

            var order = new Order
            {
                Id = 1,
                Next = existingOrder2,
                Shares = 5,
                Subscription = new Subscription
                {
                    Owned = 5,
                    UserId = _userId1
                }
            };

            existingLimit2.AskHead = order;
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
        public void Ask_FullyExecutes_AginstUser1Bid()
        {
            // Arrange.
            var existingBidOrder = new Order { Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingBidOrder.Id, existingBidOrder);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId2 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId2, 1, 100);

            // Assert.
            var existingBidOrder2 = new Order { Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrder2 };
            existingBidOrder2.ParentLimit = existingLimit2;

            var existingSellerSubscription2 = new Subscription { Owned = 400, UserId = _userId2 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrder2 }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrder2.Subscription, existingSellerSubscription2 }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_PartialExecutes_AginstUser1Bid_ButAskSharesIsLessThanBidShares()
        {
            // Arrange.
            var existingBidOrder = new Order { Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingBidOrder.Id, existingBidOrder);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId2 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId2, 1, 40);

            // Assert.
            var existingBidOrder2 = new Order { Shares = 60, Subscription = new Subscription { Owned = 40, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrder2 };
            existingBidOrder2.ParentLimit = existingLimit2;

            var existingSellerSubscription2 = new Subscription { Owned = 460, UserId = _userId2 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrder2 }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrder2.Subscription, existingSellerSubscription2 }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_PartialExecutes_AginstUser1Bid_ButAskSharesIsGreaterThanBidShares()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingBidOrder.Id, existingBidOrder);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId2 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId2, 1, 200);

            // Assert.
            var existingBidOrder2 = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };

            var existingLimit2 = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrder2 };
            existingBidOrder2.ParentLimit = existingLimit2;

            var existingSellerSubscription2 = new Subscription { Owned = 400, UserId = _userId2 };
            var remainingSellersOrder = new Order
            {
                Id = 1,
                ParentLimit = existingLimit2,
                Shares = 100,
                Subscription = existingSellerSubscription2
            };

            existingLimit2.AskHead = remainingSellersOrder;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(2, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimit2 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrder2, remainingSellersOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrder2.Subscription, existingSellerSubscription2 }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_FullyExecutes_1Limit3Bids()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 1, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingBidOrder2 = new Order { Id = 2, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingBidOrder3 = new Order { Id = 3, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };
            existingBidOrder.Next = existingBidOrder2;
            existingBidOrder2.Prev = existingBidOrder;
            existingBidOrder2.Next = existingBidOrder3;
            existingBidOrder3.Prev = existingBidOrder2;

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;
            existingBidOrder2.ParentLimit = existingLimit;
            existingBidOrder3.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingBidOrder.Id, existingBidOrder);
            Orders.Add(existingBidOrder2.Id, existingBidOrder2);
            Orders.Add(existingBidOrder3.Id, existingBidOrder3);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            Subscriptions.Add(existingBidOrder2.Subscription.UserId, existingBidOrder2.Subscription);
            Subscriptions.Add(existingBidOrder3.Subscription.UserId, existingBidOrder3.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId4 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId4, 1, 300);

            // Assert.
            var existingBidOrderClone = new Order { Id = 1, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingBidOrder2Clone = new Order { Id = 2, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingBidOrder3Clone = new Order { Id = 3, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };

            existingBidOrderClone.Next = existingBidOrder2Clone;
            existingBidOrder2Clone.Prev = existingBidOrder;
            existingBidOrder2Clone.Next = existingBidOrder3Clone;
            existingBidOrder3Clone.Prev = existingBidOrder2Clone;

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrderClone };
            existingBidOrderClone.ParentLimit = existingLimitClone;
            existingBidOrder2Clone.ParentLimit = existingLimitClone;
            existingBidOrder3Clone.ParentLimit = existingLimitClone;

            var existingSellerSubscriptionClone = new Subscription { Owned = 200, UserId = _userId4 };

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrderClone, existingBidOrder2Clone, existingBidOrder3Clone }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrderClone.Subscription, existingBidOrder2Clone.Subscription, existingBidOrder3Clone.Subscription, existingSellerSubscriptionClone }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_PartialExecutes_1Limit3Bids()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingBidOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingBidOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };
            existingBidOrder.Next = existingBidOrder2;
            existingBidOrder2.Prev = existingBidOrder;
            existingBidOrder2.Next = existingBidOrder3;
            existingBidOrder3.Prev = existingBidOrder2;

            var existingLimit = Limits.First();
            existingLimit.BidHead = existingBidOrder;
            existingBidOrder.ParentLimit = existingLimit;
            existingBidOrder2.ParentLimit = existingLimit;
            existingBidOrder3.ParentLimit = existingLimit;

            Limits.Add(existingLimit);
            Orders.Add(existingBidOrder.Id, existingBidOrder);
            Orders.Add(existingBidOrder2.Id, existingBidOrder2);
            Orders.Add(existingBidOrder3.Id, existingBidOrder3);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            Subscriptions.Add(existingBidOrder2.Subscription.UserId, existingBidOrder2.Subscription);
            Subscriptions.Add(existingBidOrder3.Subscription.UserId, existingBidOrder3.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId4 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId4, 1, 400);

            // Assert.
            var existingBidOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingBidOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingBidOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };
            existingBidOrderClone.Next = existingBidOrder2Clone;
            existingBidOrder2Clone.Prev = existingBidOrder;
            existingBidOrder2Clone.Next = existingBidOrder3Clone;
            existingBidOrder3Clone.Prev = existingBidOrder2Clone;

            var newAskOrder = new Order { Id = 1, Shares = 100, Subscription = new Subscription { Owned = 200, UserId = _userId4 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrderClone, AskHead = newAskOrder };
            existingBidOrderClone.ParentLimit = existingLimitClone;
            existingBidOrder2Clone.ParentLimit = existingLimitClone;
            existingBidOrder3Clone.ParentLimit = existingLimitClone;
            newAskOrder.ParentLimit = existingLimitClone;

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(4, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrderClone, existingBidOrder2Clone, existingBidOrder3Clone, newAskOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrderClone.Subscription, existingBidOrder2Clone.Subscription, existingBidOrder3Clone.Subscription, newAskOrder.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_FullyExecutes_3Limits3Bids1OnEachLimit()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingBidOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingBidOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };

            var existingLimit = Limits.First();
            var limit2 = new Limit { Price = 2 * ScaleFactor };
            var limit3 = new Limit { Price = 3 * ScaleFactor };

            existingLimit.BidHead = existingBidOrder;
            limit2.BidHead = existingBidOrder2;
            limit3.BidHead = existingBidOrder3;

            existingBidOrder.ParentLimit = existingLimit;
            existingBidOrder2.ParentLimit = limit2;
            existingBidOrder3.ParentLimit = limit3;

            Limits.Add(existingLimit);
            Limits.Add(limit2);
            Limits.Add(limit3);
            Orders.Add(existingBidOrder.Id, existingBidOrder);
            Orders.Add(existingBidOrder2.Id, existingBidOrder2);
            Orders.Add(existingBidOrder3.Id, existingBidOrder3);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            Subscriptions.Add(existingBidOrder2.Subscription.UserId, existingBidOrder2.Subscription);
            Subscriptions.Add(existingBidOrder3.Subscription.UserId, existingBidOrder3.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId4 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId4, 1, 300);

            // Assert.
            var existingBidOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingBidOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingBidOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrderClone };
            var existingLimit2Clone = new Limit { Price = 2 * ScaleFactor, BidHead = existingBidOrder2Clone };
            var existingLimit3Clone = new Limit { Price = 3 * ScaleFactor, BidHead = existingBidOrder3Clone };
            existingBidOrderClone.ParentLimit = existingLimitClone;
            existingBidOrder2Clone.ParentLimit = existingLimit2Clone;
            existingBidOrder3Clone.ParentLimit = existingLimit3Clone;

            var existingSellerSubscriptionClone = new Subscription { Owned = 200, UserId = _userId4 };

            Assert.AreEqual(3, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone, limit2, limit3 }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrderClone, existingBidOrder2Clone, existingBidOrder3Clone }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrderClone.Subscription, existingBidOrder2Clone.Subscription, existingBidOrder3Clone.Subscription, existingSellerSubscriptionClone }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }

        [TestMethod]
        public void Ask_PartialExecutes_3Limits3Bids1OnEachLimit()
        {
            // Arrange.
            var existingBidOrder = new Order { Id = 100, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId1 } };
            var existingBidOrder2 = new Order { Id = 200, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId2 } };
            var existingBidOrder3 = new Order { Id = 300, Shares = 100, Subscription = new Subscription { Owned = 0, UserId = _userId3 } };

            var existingLimit = Limits.First();
            var limit2 = new Limit { Price = 2 * ScaleFactor };
            var limit3 = new Limit { Price = 3 * ScaleFactor };

            existingLimit.BidHead = existingBidOrder;
            limit2.BidHead = existingBidOrder2;
            limit3.BidHead = existingBidOrder3;

            existingBidOrder.ParentLimit = existingLimit;
            existingBidOrder2.ParentLimit = limit2;
            existingBidOrder3.ParentLimit = limit3;

            Limits.Add(existingLimit);
            Limits.Add(limit2);
            Limits.Add(limit3);
            Orders.Add(existingBidOrder.Id, existingBidOrder);
            Orders.Add(existingBidOrder2.Id, existingBidOrder2);
            Orders.Add(existingBidOrder3.Id, existingBidOrder3);

            Subscriptions.Add(existingBidOrder.Subscription.UserId, existingBidOrder.Subscription);
            Subscriptions.Add(existingBidOrder2.Subscription.UserId, existingBidOrder2.Subscription);
            Subscriptions.Add(existingBidOrder3.Subscription.UserId, existingBidOrder3.Subscription);
            var existingSellerSubscription = new Subscription { Owned = 500, UserId = _userId4 };
            Subscriptions.Add(existingSellerSubscription.UserId, existingSellerSubscription);

            // Act.
            Ask(_userId4, 1, 400);

            // Assert.
            var existingBidOrderClone = new Order { Id = 100, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId1 } };
            var existingBidOrder2Clone = new Order { Id = 200, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId2 } };
            var existingBidOrder3Clone = new Order { Id = 300, Shares = 0, Subscription = new Subscription { Owned = 100, UserId = _userId3 } };

            var newAskOrder = new Order { Id = 1, Shares = 100, Subscription = new Subscription { Owned = 200, UserId = _userId4 } };

            var existingLimitClone = new Limit { Price = 1 * ScaleFactor, BidHead = existingBidOrderClone, AskHead = newAskOrder };
            var existingLimit2Clone = new Limit { Price = 2 * ScaleFactor, BidHead = existingBidOrder2Clone };
            var existingLimit3Clone = new Limit { Price = 3 * ScaleFactor, BidHead = existingBidOrder3Clone };
            existingBidOrderClone.ParentLimit = existingLimitClone;
            existingBidOrder2Clone.ParentLimit = existingLimit2Clone;
            existingBidOrder3Clone.ParentLimit = existingLimit3Clone;
            newAskOrder.ParentLimit = existingLimitClone;

            Assert.AreEqual(3, Limits.Count);
            Assert.AreEqual(4, Orders.Count);
            Assert.AreEqual(4, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { existingLimitClone, existingLimit2Clone, existingLimit3Clone }, Limits, _limitTestComparer);
            CollectionAssert.AreEqual(new Order[] { existingBidOrderClone, existingBidOrder2Clone, existingBidOrder3Clone, newAskOrder }, (ICollection)Orders.Values, _orderTestComparer);
            CollectionAssert.AreEqual(new Subscription[] { existingBidOrderClone.Subscription, existingBidOrder2Clone.Subscription, existingBidOrder3Clone.Subscription, newAskOrder.Subscription }, (ICollection)Subscriptions.Values, _subscriptionTestComparer);
        }
    }
}
