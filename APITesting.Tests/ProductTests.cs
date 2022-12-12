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
        public void Should_Get_All_Products()
        {
            Assert.That(productService.GetList().Count != 0, "List is empty");
        }

        [Test]
        public void Should_Add_Product()
        {
            Product productForTestFirst = configuration.GetSection("validProductFirst").Get<Product>();
            Product productForTestSecond = configuration.GetSection("validProductSecond").Get<Product>();

            ApiResponse responseFirst = productService.Create(productForTestFirst);
            ApiResponse responseSecond = productService.Create(productForTestSecond);

            idList.Add(responseFirst.id);
            idList.Add(responseSecond.id);

            List<Product> products = productService.GetList();
            Product createdProductFirst = productService.Get(responseFirst.id);
            Product createdProductSecond = productService.Get(responseSecond.id);
            
            Assert.That(responseFirst.id != "-1", "Error on the server");
            Assert.That(responseSecond.id != "-1", "Error on the server");
            Assert.That(products.Find(item => item.id == responseFirst.id) != null, "The first product was not created");
            Assert.That(products.Find(item => item.id == responseSecond.id) != null, "The second product was not created");
            Assert.That(AreEqual(productForTestFirst, createdProductFirst), "The first product created does not match the input");
            Assert.That(AreEqual(productForTestSecond, createdProductSecond), "The second product created does not match the input");
        }

        [Test]
        public void Should_Add_Products_With_Same_Title_And_Alias_Should_Be_With_Zero_Sign()
        {
            Product productForTestFirst = configuration.GetSection("validProductFirst").Get<Product>();
            Product productForTestSecond = configuration.GetSection("validProductFirst").Get<Product>();

            ApiResponse responseFirst = productService.Create(productForTestFirst);
            ApiResponse responseSecond = productService.Create(productForTestSecond);

            idList.Add(responseFirst.id);
            idList.Add(responseSecond.id);

            List<Product> products = productService.GetList();

            Product createdProductFirst = productService.Get(responseFirst.id);
            Product createdProductSecond = productService.Get(responseSecond.id);
            var createdProducts = products.FindAll(p => p.id == responseFirst.id || p.id == responseSecond.id);

            Assert.That(responseFirst.id != "-1", "Error on the server");
            Assert.That(responseSecond.id != "-1", "Error on the server");
            Assert.That(products.Find(item => item.id == responseFirst.id) != null, "The first product was not created");
            Assert.That(products.Find(item => item.id == responseSecond.id) != null, "The second product was not created");
            Assert.That(createdProducts[1].alias == createdProducts[0].alias + "-0", "Alias do not differ");
        }

        [Test]
        public void Should_Not_Add_Invalid_Product_And_Null_Product()
        {
            Product invalidProductByCategoryId = configuration.GetSection("invalidProductByCategoryId").Get<Product>();
            Product invalidProductByHit = configuration.GetSection("invalidProductByHit").Get<Product>();
            Product invalidProductByStatus = configuration.GetSection("invalidProductByStatus").Get<Product>();
            Product productForTestNull = configuration.GetSection("nullProduct").Get<Product>();

            ApiResponse responseWithCategoryId = productService.Create(invalidProductByCategoryId);
            ApiResponse responseWithHit = productService.Create(invalidProductByHit);
            ApiResponse responseWithStatus = productService.Create(invalidProductByStatus);
            ApiResponse responseNull = productService.Create(productForTestNull);

            List<Product> products = productService.GetList();

            Assert.That(responseWithCategoryId.id == "-1", "Error on the server");
            Assert.That(responseWithHit.id == "-1", "Error on the server");
            Assert.That(responseWithStatus.id == "-1", "Error on the server");
            Assert.That(responseNull.id == "-1", "Error on the server");
            Assert.That(products.Find(p => p.id == responseWithCategoryId.id) == null, "The product with id has been created");
            Assert.That(products.Find(p => p.id == responseWithHit.id) == null, "The product with hit has been created");
            Assert.That(products.Find(p => p.id == responseWithStatus.id) == null, "The product with status has been created");
            Assert.That(products.Find(p => p.id == responseNull.id) == null, "The null product has been created");
        }

        [Test]
        public void Should_Delete_Product()
        {
            Product productForTest = configuration.GetSection("validProductFirst").Get<Product>();
            ApiResponse response = productService.Create(productForTest);

            idList.Add(response.id);

            bool isDeleted = productService.Delete(response.id);
            List<Product> products = productService.GetList();

            Assert.That(response.id != "-1", "Error on the server");
            Assert.That(isDeleted, "Product has not been removed");
            Assert.That(products.Find(p => p.id == response.id) == null, "Product has not been removed");
        }

        [Test]
        public void Should_Do_Nothing_When_Deleting_Not_Existing_Product()
        {
            Product notExistingProduct = configuration.GetSection("notExistingProduct").Get<Product>();

            bool isDeleted = productService.Delete(notExistingProduct.id);

            Assert.That(!isDeleted, "Product removed");
        }

        [Test]
        public void Should_Successfully_Update_Product()
        {
            Product productToUpdate = configuration.GetSection("validProductFirst").Get<Product>();
            Product updated = configuration.GetSection("validProductForUpdate").Get<Product>();
        
            ApiResponse response = productService.Create(productToUpdate);
        
            List<Product> products = productService.GetList();

            var temp = products.Find(p => p.id == response.id)!;
            updated.id = temp.id;

            bool isUpdated = productService.Update(updated);
        
            idList.Add(response.id);
            
            products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);

            Assert.That(response.id != "-1", "Error on the server");
            Assert.That(isUpdated, "Product not updated");
            Assert.That(AreEqual(updatedProduct, updated), "The updated product is different from the update");
        }

        [Test]
        public void Should_Do_Nothing_When_Updating_With_Invalid_Data()
        {
            Product productForUpdate = configuration.GetSection("validProductFirst").Get<Product>();
            Product updated = configuration.GetSection("invalidProductForUpdate").Get<Product>();

            ApiResponse response = productService.Create(productForUpdate);

            bool isUpdated = productService.Update(updated);

            idList.Add(response.id);

            List<Product> products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);

            Assert.That(response.id != "-1", "Error on the server");
            Assert.That(!isUpdated, "Product updated");
            Assert.That(updatedProduct != null, "Product removed");
        }

        [Test]
        public void Should_Do_Nothing_When_Updating_Not_Existing_Product()
        {
            Product productForUpdate = configuration.GetSection("notExistingProduct").Get<Product>();
            Product updated = configuration.GetSection("validProductFirst").Get<Product>();
            updated.id = productForUpdate.id;

            bool isUpdated = productService.Update(updated);

            List<Product> products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);

            idList.Add(products.Find(p => p.title == updated.title).id);

            Assert.That(!isUpdated, "Product updated");
            Assert.That(updatedProduct != null, "A new product was created while trying to update a non-existent product");
        }

        [Test]
        public void Should_Update_Product_And_Alias_Has_Id()
        {
            Product productForUpdate = configuration.GetSection("validProductFirst").Get<Product>();
            Product updated = configuration.GetSection("validProductForUpdate").Get<Product>();

            ApiResponse response = productService.Create(productForUpdate);
            List<Product> products = productService.GetList();

            var temp = products.Find(p => p.id == response.id);

            updated.id = temp.id;
            updated.alias = temp.alias;
            updated.title = temp.title;

            bool isUpdated = productService.Update(updated);

            idList.Add(response.id);

            products = productService.GetList();
            var updatedProduct = products.Find(p => p.id == updated.id);

            Assert.That(response.id != "-1", "Error on the server");
            Assert.That(isUpdated, "Product not updated");
            Assert.That(AreEqual(updatedProduct, updated), "The updated product is different from the update");
            Assert.That(updatedProduct.alias.Contains(updated.id), "Updated product alias does not contain product id");
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