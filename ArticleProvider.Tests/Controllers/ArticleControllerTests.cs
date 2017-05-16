using ArticleProvider.Services;
using ArticleProvider.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL;
using System.Threading.Tasks;
using DAL.Model;
using Moq;
using System.Data.Entity;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace ArticleProvider.Tests.Controllers
{
    [TestClass]
    public class ArticleControllerTests
    {
        private void ConfigMockSet<T>(Mock mockSet, IQueryable<T> data)
        {
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());
        }

        /// <summary>
        /// This test method checks whether an article is liked before by a specific user before.
        /// </summary>
        [TestMethod]
        public void ArticleLikedBeforeCheckTest()
        {
            var likes = new List<ArticleLike>()
            {
                new ArticleLike() {ArticleId = 5, UserId = "testUser" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ArticleLike>>();
            ConfigMockSet<ArticleLike>(mockSet, likes);

            var mockContext = new Mock<ArticleContext>();
            mockContext.Setup(c => c.Likes).Returns(mockSet.Object);

           

            var identityMock = new Mock<ControllerContext>();
            identityMock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("testUser");
            identityMock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            ArticlesController controlller = new ArticlesController(new TestConfigProvider(), mockContext.Object);
            controlller.ControllerContext = identityMock.Object;

            PrivateObject prio = new PrivateObject(controlller);
            var result = prio.Invoke("IsArticleLikedBefore", 5);
            Assert.AreEqual(true, result);
        }

        /// <summary>
        /// This testchecks whether a user exceeded the total number of likes in a day 
        /// that is configured by the system
        /// </summary>
        [TestMethod]
        public void UserLikesPerDayLimitTest()
        {
            var likes = new List<ArticleLike>()
            {
                new ArticleLike() {ArticleId = 5, UserId = "testUser" }
            };

            var mockSet = new Mock<DbSet<ArticleLike>>();
            ConfigMockSet<ArticleLike>(mockSet, likes.AsQueryable());

            var mockContext = new Mock<ArticleContext>();
            mockContext.Setup(c => c.Likes).Returns(mockSet.Object);

            var identityMock = new Mock<ControllerContext>();
            identityMock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("testUser");
            identityMock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);

            ArticlesController controlller = new ArticlesController(new TestConfigProvider(), mockContext.Object);
            controlller.ControllerContext = identityMock.Object;
            PrivateObject prio = new PrivateObject(controlller);
            var result = prio.Invoke("IsArticleLikeCountExceededForUser");
            Assert.AreEqual(false, result);
            var exceeded = new TestConfigProvider().GetMaximumLikesPerDay();
            for (int i = 0; i < exceeded+3; i++)
            {
                likes.Add(new ArticleLike() { Date = DateTime.Now, UserId = "testUser", Id = 10 + i });
            }

            result = prio.Invoke("IsArticleLikeCountExceededForUser");
            Assert.AreEqual(true, result);
        }
}

    public class TestConfigProvider : IConfigurationProvider
    {
        public int GetMaximumLikesPerDay()
        {
            return 5;
        }
    }
}
