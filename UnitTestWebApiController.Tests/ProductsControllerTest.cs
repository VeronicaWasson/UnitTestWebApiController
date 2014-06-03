using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Routing;
using UnitTestWebApiController.Controllers;
using UnitTestWebApiController.Models;
using UnitTestWebApiController.Models.Fakes;

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
            repository = new StubIProductRepository
            {
                GetByIdInt32 = (id) => new Product { Id = id, Name = "Product" }
            };
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
        public void PostSetsLocationHeader_FakesVersion()
        {
            // This version uses a stub for UrlHelper.

            // Arrange
            ProductsController controller = new ProductsController(repository);
            controller.Request = new HttpRequestMessage();
            controller.Configuration = new HttpConfiguration();

            var stubUrlHelper = new System.Web.Http.Routing.Fakes.StubUrlHelper
            {
                LinkStringObject = (str, obj) => "http://location_header/"
            };

            controller.Url = stubUrlHelper;

            // Act
            Product product = new Product() { Name = "Product1" };
            var response = controller.Post(product);

            // Assert
            Assert.AreEqual("http://location_header/", response.Headers.Location.AbsoluteUri);
        }    
    }
}
