using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Syringe.Core.Extensions;

namespace Syringe.Tests.Unit.Core.Extensions
{
    [TestFixture]
    public class PagerTests
    {
        [Test]
        [TestCase(5, 1, 5)]
        [TestCase(5, -10, 5)]
        public void GetPaged_should_get_correct_pagedItems(int noOfResults, int pageNumber, int expectedAmount)
        {
            // given
            List<int> items = new List<int> { 1, 2, 3, 4, 5, 6 };
            // when
            var pagedItems = items.GetPaged(noOfResults, pageNumber);
            // then
            Assert.AreEqual(pagedItems.Count(), expectedAmount);
        }
    }
}
