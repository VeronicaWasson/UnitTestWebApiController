using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using UnitTestWebApiController.Controllers;
using UnitTestWebApiController.Models;
using UnitTestWebApiController.Models.Fakes;

namespace UnitTestWebApiController.Tests
{
    [TestClass]
    public class Products2ControllerTest
    {
        [TestMethod]
        public void GetReturnsProductWithSameId()
        {
            var controller = new Products2Controller(new StubIProductRepository
            {
                GetByIdInt32 = (id) => new Product { Id = id, Name = "Product" }
            });

            IHttpActionResult actionResult = controller.Get(10);
            var contentResult = actionResult as OkNegotiatedContentResult<Product>;

            Assert.IsNotNull(contentResult);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(10, contentResult.Content.Id);
        }

        [TestMethod]
        public void GetReturnsNotFound()
        {
            var controller = new Products2Controller(new StubIProductRepository());
            IHttpActionResult actionResult = controller.Get(10);
            Assert.IsInstanceOfType(actionResult, typeof(NotFoundResult));
        }

        [TestMethod]
        public void DeleteReturnsOk()
        {
            var controller = new Products2Controller(new StubIProductRepository());
            IHttpActionResult actionResult = controller.Delete(10);
            Assert.IsInstanceOfType(actionResult, typeof(OkResult));
        }

        [TestMethod]
        public void PostMethodSetsLocationHeader()
        {
            var controller = new Products2Controller(new StubIProductRepository());

            IHttpActionResult actionResult = controller.Post(new Product { Id = 10, Name = "Product1" });

            var createdResult = actionResult as CreatedAtRouteNegotiatedContentResult<Product>;

            Assert.IsNotNull(createdResult);
            Assert.AreEqual("DefaultApi", createdResult.RouteName);
            Assert.AreEqual(10, createdResult.RouteValues["id"]);
        }

        [TestMethod]
        public void PutReturnsContentResult()
        {
            var controller = new Products2Controller(new StubIProductRepository());

            IHttpActionResult actionResult = controller.Put(new Product { Id = 10, Name = "Product" });
            var contentResult = actionResult as NegotiatedContentResult<Product>;
            Assert.IsNotNull(contentResult);
            Assert.AreEqual(HttpStatusCode.Accepted, contentResult.StatusCode);
            Assert.IsNotNull(contentResult.Content);
            Assert.AreEqual(10, contentResult.Content.Id);
        }

    }
}
