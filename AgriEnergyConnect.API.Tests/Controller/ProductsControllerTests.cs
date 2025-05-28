using AgriEnergyConnect.API.Controllers;
using AgriEnergyConnect.API.Models;
using AgriEnergyConnect.API.Services;
using AgriEnergyConnect.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgriEnergyConnect.API.Tests.Controllers
{
    [TestClass]
    public class ProductsControllerTests : TestBase
    {
        private Mock<IProductService> _mockProductService;
        private Mock<ILogger<ProductsController>> _mockLogger;
        private ProductsController _controller;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockProductService = new Mock<IProductService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            _controller = new ProductsController(_mockProductService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task GetFilteredProductsByFarmer_ReturnsBadRequest_WhenFarmerIdIsEmpty()
        {
            // Act
            var result = await _controller.GetFilteredProductsByFarmer("", null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task GetFilteredProductsByFarmer_ReturnsNotFound_WhenFarmerDoesNotExist()
        {
            // Arrange
            _mockProductService.Setup(x => x.FarmerExistsAsync("nonexistent"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.GetFilteredProductsByFarmer("nonexistent", null, null, null);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task GetFilteredProductsByFarmer_ReturnsOk_WithValidParameters()
        {
            // Arrange
            var farmerId = "farmer1";
            var expectedProducts = new List<ProductModel> { new ProductModel() };

            _mockProductService.Setup(x => x.FarmerExistsAsync(farmerId))
                .ReturnsAsync(true);
            _mockProductService.Setup(x => x.GetFilteredProductsByFarmerAsync(farmerId, null, null, null))
                .ReturnsAsync(expectedProducts);

            // Act
            var result = await _controller.GetFilteredProductsByFarmer(farmerId, null, null, null);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(expectedProducts, okResult.Value);
        }

        [TestMethod]
        public async Task CreateProduct_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Name", "Required");
            var model = new CreateProductModel();

            // Act
            var result = await _controller.CreateProduct(model);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        
    }
}