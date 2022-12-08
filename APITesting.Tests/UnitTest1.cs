using APITesting.Domain;
using APITesting.Services;
using Microsoft.Extensions.Configuration;
using static APITesting.Domain.Product;

namespace APITesting.Tests
{
    public class Tests
    {
        const string configurationFile = "D:/testing/APITesting/APITesting.Tests/Configuration/Configuration.json";
        private readonly ProductService productService = new();
        private readonly IConfigurationRoot configuration = new ConfigurationBuilder().AddJsonFile(configurationFile).Build();

        private readonly List<string> idList = new();

        private bool AreEqual(Product productFirst, Product productSecond)
        {
            Assert.That(productFirst.title == productSecond.title);
            Assert.That(productFirst.content == productSecond.content);
            Assert.That(productFirst.price == productSecond.price);
            Assert.That(productFirst.status == productSecond.status);
            Assert.That(productFirst.keywords == productSecond.keywords);
            Assert.That(productFirst.description == productSecond.description);
            Assert.That(productFirst.hit == productSecond.hit);

            return true;
        }

        [Test]
        public void Should_Successfully_Get_All_Products()
        {
            //Arrange

            //Act

            //Assert
            Assert.That(productService.GetList().Count != 0, "Лист с продуктами пустой");
        }

        [Test]
        public void Should_Successfully_Add_Product()
        {
            //Arrange
            Product productForTestFirst = configuration.GetSection("validProductFirst").Get<Product>();
            Product productForTestSecond = configuration.GetSection("validProductSecond").Get<Product>();

            var responseFirst = productService.Create(productForTestFirst);
            var responseSecond = productService.Create(productForTestSecond);

            idList.Add(responseFirst.id);
            idList.Add(responseSecond.id);

            //Act
            List<Product> products = productService.GetList();
            var createdProductFirst = productService.Get(responseFirst.id);
            var createdProductSecond = productService.Get(responseSecond.id);
        
            //Assert
            Assert.That(responseFirst.id != "-1", "Ошибка на сервере");
            Assert.That(responseSecond.id != "-1", "Ошибка на сервере");
            Assert.That(products.Find(item => item.id == responseFirst.id) != null, "Первый продукт не был создан");
            Assert.That(products.Find(item => item.id == responseSecond.id) != null, "Второй продукт не был создан");
            Assert.That(AreEqual(productForTestFirst, createdProductFirst), "Первый созданный продукт не совпадает с входными данными");
            Assert.That(AreEqual(productForTestSecond, createdProductSecond), "Второй созданный продукт не совпадает с входными данными");
        }

        [Test]
        public void Should_Successfully_Add_Products_With_Same_Title()
        {
            //Arrange
            Product productForTestFirst = configuration.GetSection("validProductFirst").Get<Product>();
            Product productForTestSecond = configuration.GetSection("validProductFirst").Get<Product>();

            var responseFirst = productService.Create(productForTestFirst);
            var responseSecond = productService.Create(productForTestSecond);

            idList.Add(responseFirst.id);
            idList.Add(responseSecond.id);

            //Act
            List<Product> products = productService.GetList();
            var createdProductFirst = productService.Get(responseFirst.id);
            var createdProductSecond = productService.Get(responseSecond.id);

            var createdProducts = products.FindAll(p => p.id == responseFirst.id || p.id == responseSecond.id);

            //Assert
            Assert.That(responseFirst.id != "-1", "Ошибка на сервере");
            Assert.That(responseSecond.id != "-1", "Ошибка на сервере");
            Assert.That(products.Find(item => item.id == responseFirst.id) != null, "Первый продукт не был создан");
            Assert.That(products.Find(item => item.id == responseSecond.id) != null, "Второй продукт не был создан");
            Assert.That(AreEqual(productForTestFirst, createdProductFirst), "Первый созданный продукт не совпадает с входными данными");
            Assert.That(AreEqual(productForTestSecond, createdProductSecond), "Второй созданный продукт не совпадает с входными данными");

            Assert.That(createdProducts[1].alias == createdProducts[0].alias + "-0", "Alias продукта не отличаются");
        }

        
        [Test]
        public void Should_Not_Add_Invalid_Product()
        {
            //Arrange
            Product productForTest = configuration.GetSection("invalidProduct").Get<Product>();
        
            ApiResponse response = productService.Create(productForTest);
        
            //Act
            List<Product> products = productService.GetList();

            //Assert
            Assert.That(products.Find(p => p.id == response.id) == null, "Продукт был создан");
        }
        
        [Test]
        public void Should_Not_Add_nullObject()
        {
            //Arrange
            var productForTest = configuration.GetSection("nullObject").Get<Product>();
        
            var response = productService.Create(productForTest);
        
            //Act
            List<Product> products = productService.GetList();
        
            //Assert
            Assert.That(products.Find(p => p.id == response.id) == null, "Продукт был создан");
        }

        [Test]
        public void Should_Successfully_Delete_Existing_Product()
        {
            //Arrange
            Product productForTest = configuration.GetSection("validProductFirst").Get<Product>();
            ApiResponse response = productService.Create(productForTest);



            idList.Add(response.id);

            //Act
            var isSuccess = productService.Delete(response.id);
            List<Product> products = productService.GetList();

            //Assert
            Assert.That(products.Find(p => p.id == response.id) == null, "Товар не был удален");
        }

        [Test]
        public void Should_Do_Nothing_When_Deleting_Not_Existing_Product()
        {
            //Arrange
            Product notExistingProduct = configuration.GetSection("notExistingProduct").Get<Product>();

            //Act
            var isDeleted = productService.Delete(notExistingProduct.id);

            //Assert
            Assert.That(!isDeleted, "Товар удален");
        }

        [Test]
        public void Should_Successfully_Update_Product()
        {
            //Arrange
            Product productToUpdate = configuration.GetSection("validProductFirst").Get<Product>();
            Product updated = configuration.GetSection("validProductForUpdate").Get<Product>();
        
            var response = productService.Create(productToUpdate);
        
            List<Product> products = productService.GetList();

            var temp = products.Find(p => p.id == response.id)!;
            updated.id = temp.id;

            //Act
            bool isSuccess = productService.Update(updated);
        
            idList.Add(response.id);
            
            products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);
            
            //Assert
            Assert.That(AreEqual(updatedProduct, updated), "Обновленный товар отличается от обновления");
        
        }

        [Test]
        public void Should_Do_Nothing_When_Updating_With_Invalid_Data()
        {
            //Arrange
            Product productForUpdate = configuration.GetSection("validProductFirst").Get<Product>();
            Product updated = configuration.GetSection("invalidProductForUpdate").Get<Product>();

            var response = productService.Create(productForUpdate);

            idList.Add(response.id);

            //Act
            List<Product> products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);

            //Assert
            Assert.That(updatedProduct != null, "Продукт был удален");
        }
        
        [TearDown]
        public void DeleteAfterTests()
        {
            foreach (string id in idList)
            {
                productService.Delete(id);
            }
        }
    }
}