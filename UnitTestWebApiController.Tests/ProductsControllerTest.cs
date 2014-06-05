using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using UnitTestWebApiController.Controllers;
using UnitTestWebApiController.Models;

namespace UnitTestWebApiController.Tests
{
    [TestClass]
    public class ProductsControllerTest
    {
        IProductRepository repository;

        [TestInitialize]
        public void Initialize()
        {
            // Create stub
            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(x => x.GetById(It.IsAny<int>()))
                .Returns((int id) => new Product { Id = id, Name = "Product" });

            repository = mockRepository.Object;

        }
        
        [TestMethod]
        public void GetReturnsProduct()
        {
            // Arrange
            var controller = new ProductsController(repository);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            // Act
            var response = controller.Get(10);

            // Assert
            Product product;
            Assert.IsTrue(response.TryGetContentValue<Product>(out product));
            Assert.AreEqual(10, product.Id);
        }

        [TestMethod]
        public void PostSetsLocationHeader()
        {
            // Arrange
            ProductsController controller = new ProductsController(repository);

            controller.Request = new HttpRequestMessage
            {
                RequestUri = new Uri("http://localhost/api/products")
            };
            controller.Configuration = new HttpConfiguration();
            controller.Configuration.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            controller.RequestContext.RouteData = new HttpRouteData(
                route: new HttpRoute(),
                values: new HttpRouteValueDictionary { { "controller", "products" } });

            // Act
            Product product = new Product() { Id = 42, Name = "Product1" };
            var response = controller.Post(product);

            // Assert
            Assert.AreEqual("http://localhost/api/products/42", response.Headers.Location.AbsoluteUri);
        }

        [TestMethod]
        public void PostSetsLocationHeader_MockVersion()
        {
            // This version uses a mock UrlHelper.

            // Arrange
            ProductsController controller = new ProductsController(repository);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            string locationUrl = "http://location/";

             //Create the mock and set up the Link method, which is used to create the Location header.
             //The mock version returns a fixed string.
            var mockUrlHelper = new Mock<UrlHelper>();
            mockUrlHelper.Setup(x => x.Link(It.IsAny<string>(), It.IsAny<object>())).Returns(locationUrl);
            controller.Url = mockUrlHelper.Object;

            // Act
            var response = controller.Post(new Product() { Id = 42 });

            var foop = controller.Url.Link("DefaultApi", new { id = 42 });
            // Assert
            Assert.AreEqual(locationUrl, response.Headers.Location.AbsoluteUri);
        }
    }
}
