using Microsoft.VisualStudio.TestTools.UnitTesting;
using Subtrader.Models;
using Subtrader.UTest.Comparers;

namespace Subtrader.UTest
{
    [TestClass]
    public partial class OrderBookTests : OrderBook
    {
        private LimitTestComparer _limitTestComparer;
        private OrderTestComparer _orderTestComparer;
        private SubscriptionTestComparer _subscriptionTestComparer;
        private static readonly long _userId1 = 1;
        private static readonly long _userId2 = 2;
        private static readonly long _userId3 = 3;
        private static readonly long _userId4 = 4;

        [TestInitialize]
        public void Initialize()
        {
            _limitTestComparer = new LimitTestComparer();
            _orderTestComparer = new OrderTestComparer();
            _subscriptionTestComparer = new SubscriptionTestComparer();
        }

        [TestMethod]
        public void ConstructorParameterless_CreatesStructure()
        {
            AssertConstructor();
        }

        [TestMethod]
        public void Cancel_DoesNothing_WhenPassedGuidIsNotFound()
        {
            // Arrange.
            AssertConstructor();

            // Act.
            Cancel(1);

            // Assert nothing should have changed.
            AssertConstructor();
        }

        // TODO: Needs CollectionAsserts.
        [TestMethod]
        public void Cancel_Removes1OrderFromExistingLimitAndRemovesFromOrders_WhenOrderIsAskType()
        {
            // Arrange.
            AssertConstructor();

            Subscriptions.Add(_userId1, new Subscription { Owned = 5, UserId = _userId1 });
            var newOrder = Ask(_userId1, 1, 5);

            // Make sure the above actually worked.
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);

            // Act.
            Cancel(newOrder.Id);

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
        }

        // TODO: Needs CollectionAsserts.
        [TestMethod]
        public void Cancel_Removes1OrderFromExistingLimitThatContains2OrdersAndRemovesFromOrders_WhenOrderIsAskType()
        {
            // Arrange.
            AssertConstructor();

            Subscriptions.Add(_userId1, new Subscription { Owned = 5, UserId = _userId1 });
            Subscriptions.Add(_userId2, new Subscription { Owned = 10, UserId = _userId2 });
            Subscriptions.Add(_userId3, new Subscription { Owned = 15, UserId = _userId3 });
            Ask(_userId1, 1, 5);
            var newOrder = Ask(_userId2, 1, 10);
            Ask(_userId3, 1, 15);

            // Make sure the above actually worked.
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(3, Subscriptions.Count);

            // Act.
            Cancel(newOrder.Id);

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(3, Subscriptions.Count);
        }

        // TODO: Needs CollectionAsserts.
        [TestMethod]
        public void Cancel_Removes1OrderFromExistingLimitAndRemovesFromOrders_WhenOrderIsBidType()
        {
            // Arrange.
            AssertConstructor();

            var newOrder = Bid(_userId1, 1, 5);

            // Make sure the above actually worked.
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(1, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);

            // Act.
            Cancel(newOrder.Id);

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(1, Subscriptions.Count);
        }

        // TODO: Needs CollectionAsserts.
        [TestMethod]
        public void Cancel_Removes1OrderFromExistingLimitThatContains2OrdersAndRemovesFromOrders_WhenOrderIsBidType()
        {
            // Arrange.
            AssertConstructor();

            Bid(_userId1, 1, 5);
            var newOrder = Bid(_userId2, 1, 10);
            Bid(_userId3, 1, 15);

            // Make sure the above actually worked.
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(3, Orders.Count);
            Assert.AreEqual(3, Subscriptions.Count);

            // Act.
            Cancel(newOrder.Id);

            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(2, Orders.Count);
            Assert.AreEqual(3, Subscriptions.Count);
        }

        private void AssertConstructor()
        {
            Assert.AreEqual(1, Limits.Count);
            Assert.AreEqual(0, Orders.Count);
            Assert.AreEqual(0, Subscriptions.Count);
            CollectionAssert.AreEqual(new Limit[] { new Limit { Price = 1 * ScaleFactor } }, Limits, _limitTestComparer);
        }
    }
}
