using Lab_9.Domain;
using Lab_9.Services;
using Microsoft.Extensions.Configuration;

namespace Tests;

public class Tests
{
    private readonly ProductService _service = new();

    private readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .AddJsonFile("D:/testing/Lab_9/Lab_9.Tests/Configurations/Configuration.json").Build();

    private readonly List<string> _productIds = new();


    private bool AreEqual(Product p1, Product p2)
    {
        Assert.That(p1.title == p2.title);
        Assert.That(p1.content == p2.content);
        Assert.That(p1.price == p2.price);
        Assert.That(p1.status == p2.status);
        Assert.That(p1.keywords == p2.keywords);
        Assert.That(p1.description == p2.description);
        Assert.That(p1.hit == p2.hit);

        return true;
    }

    [TearDown]
    public void Delete()
    {
        foreach (var id in _productIds)
            _service.Delete(id);
    }

    [Test]
    public void Should_Successfully_Get_All_Products()
    {
        var products = _service.GetAll();
        Assert.That(products is not null, "Список продуктов пуст");
    }

    [Test]
    public void Should_Successfully_Add_Product()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        _productIds.Add(creationResponse.id);

        var products = _service.GetAll();
        var createdProduct = _service.Get(creationResponse.id);

        Assert.That(creationResponse.id != "-1", "Ошибка на сервере");
        Assert.That(products!.Find(p => p.id == creationResponse.id) is not null, "Продукт не был создан");
        Assert.That(AreEqual(productToCreate, createdProduct), "При создании продукт был изменен");
    }

    [Test]
    public void Should_Successfully_Get_Product_By_Id()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        var product = _service.Get(creationResponse.id);

        _productIds.Add(creationResponse.id);

        Assert.That(product is not null, "Полученный продукт пуст");
        Assert.That(AreEqual(productToCreate, product), "Продукт был изменен при получении");
    }

    [Test]
    public void Should_Successfully_Add_Few_Products()
    {
        var productToCreate1 = _config.GetSection("valid_product_2").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_3").Get<Product>()!;

        var creationResponse1 = _service.Create(productToCreate1);
        var creationResponse2 = _service.Create(productToCreate2);


        _productIds.Add(creationResponse1.id);
        _productIds.Add(creationResponse2.id);

        var products = _service.GetAll();
        var createdProduct1 = _service.Get(creationResponse1.id);
        var createdProduct2 = _service.Get(creationResponse2.id);

        Assert.That(creationResponse1.id != "-1", "Ошибка на сервере");
        Assert.That(creationResponse2.id != "-1", "Ошибка на сервере");
        Assert.That(products!.Find(p => p.id == creationResponse1.id) is not null, "Первый продукт не был добавлен");
        Assert.That(products.Find(p => p.id == creationResponse2.id) is not null, "Второй продукт не был добавлен");
        Assert.That(AreEqual(productToCreate1, createdProduct1), "Первый продукт был изменен при создании");
        Assert.That(AreEqual(productToCreate2, createdProduct2), "Второй продукт был изменен при создании");
    }

    [Test]
    public void Should_Successfully_Add_Products_With_Same_Title()
    {
        var productToCreate1 = _config.GetSection("valid_product_1").Get<Product>()!;
        var productToCreate2 = _config.GetSection("valid_product_1").Get<Product>()!;

        var creationResponse1 = _service.Create(productToCreate1);
        var creationResponse2 = _service.Create(productToCreate2);


        _productIds.Add(creationResponse1.id);
        _productIds.Add(creationResponse2.id);


        var products = _service.GetAll();
        var createdProducts = products!.FindAll(p => p.id == creationResponse1.id
                                                     || p.id == creationResponse2.id);

        Assert.That(creationResponse1.id != "-1", "Ошибка на сервере");
        Assert.That(creationResponse2.id != "-1", "Ошибка на сервере");
        Assert.That(createdProducts[0] is not null, "Первый продукт пуст");
        Assert.That(createdProducts[1] is not null, "Второй продукт пуст");
        Assert.That(createdProducts[1]!.alias == createdProducts[0]!.alias + "-0", "Алиасы продуктов не отличаются на -0");
        Assert.That(AreEqual(productToCreate1, createdProducts[0]), "Первый продукт был изменен при создании");
        Assert.That(AreEqual(productToCreate2, createdProducts[1]), "Второй продукт был изменен при создании");

    }

    [Test]
    public void Should_Not_Add_Invalid_Product()
    {
        var productToCreate = _config.GetSection("invalid_product").Get<Product>()!;

        var creationResponse = _service.Create(productToCreate);

        var products = _service.GetAll();

        Assert.That(!products!.Exists(p => p.id == creationResponse.id), "Продукт был создан");
        Assert.That(!creationResponse.status, "Товар не был создан, но api говорит об обратном");
        Assert.That(creationResponse.id == "-1", "Ошибка на севрере");
    }

    [Test]
    public void Should_Not_Add_Null_Product()
    {
        var productToCreate = _config.GetSection("null_product").Get<Product>();

        var creationResponse = _service.Create(productToCreate);

        var products = _service.GetAll();

        Assert.That(products!.Find(p => p.id == creationResponse.id) is null, "Продукт был создан");
        Assert.That(!creationResponse.status, "Сервер вернул статус 1");
        Assert.That(creationResponse.id == "-1", "Ошибка на севрере");
    }

    [Test]
    public void Should_Successfully_Update_Product_And_Alias_Has_No_Id()
    {
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.id)!;
        updater.id = temp.id;
        updater.alias = temp.alias;

        bool isSuccess = _service.Update(updater);

        _productIds.Add(creationResponse.id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == updater.id);

        Assert.That(isSuccess, "Сервер вернул статус 0");
        Assert.That(AreEqual(updatedProduct, updater), "Обновленный товар отличается от товара, которым мы обновляли");
        Assert.That(!updatedProduct.alias.Contains(temp.id), "Алиас обновленного товара содержит айди товара");

    }


    [Test]
    public void Should_Successfully_Update_Product_And_Alias_Has_Id()
    {
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_2").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);
        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.id);

        updater.id = temp.id;
        updater.alias = temp.alias;
        updater.title = temp.title;

        bool isSuccess = _service.Update(updater);

        _productIds.Add(creationResponse.id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == updater.id);

        Assert.That(isSuccess, "Сервер вернул статус 0");
        Assert.That(AreEqual(updatedProduct, updater), "Обновленный товар отличается от товара, которым мы обновляли");
        Assert.That(updatedProduct?.alias != updater.alias, "Алиас обновленного товара совпадает с алиасом товара для обновления");
        Assert.That(updatedProduct.alias.Contains(temp.id), "Алиас обновленного товара не содержит айди товара");

    }

    [Test]
    public void Should_Do_Nothing_When_Updating_Not_Existing_Product()
    {
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;
        var updater = _config.GetSection("valid_update_product_1").Get<Product>()!;
        updater.id = notExistingProduct.id;

        bool isSuccess = _service.Update(updater);
        var products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == updater.id);

        _productIds.Add(products.Find(p => p.title == updater.title).id);

        Assert.That(!isSuccess, "Был создан новый продукт");
        Assert.That(updatedProduct is null, "Продукт был обновлен");
    }

    [Test]
    public void Should_Do_Nothing_When_Updating_With_Invalid_Product()
    {
        var productToUpdate = _config.GetSection("valid_product_1").Get<Product>()!;
        var updater = _config.GetSection("invalid_update_product").Get<Product>()!;

        var creationResponse = _service.Create(productToUpdate);

        var products = _service.GetAll();
        var temp = products!.Find(p => p.id == creationResponse.id);
        updater.id = temp.id;

        bool isSuccess = _service.Update(updater);

        _productIds.Add(creationResponse.id);

        products = _service.GetAll();
        var updatedProduct = products!.Find(p => p.id == temp.id);

        Assert.That(updatedProduct is not null, "Продукт был удален, во время изменения");
        Assert.That(!isSuccess, "Сервер вернул статус 1");
        Assert.That(AreEqual(updatedProduct, productToUpdate), "Продукт был изменен");
        Assert.That(updatedProduct?.alias == productToUpdate.alias, "Алиас продукта был изменен");
    }


    [Test]
    public void Should_Successfully_Delete_Existing_Product()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        var isSuccess = _service.Delete(creationResponse.id);


        _productIds.Add(creationResponse.id);

        var products = _service.GetAll();
        Assert.That(isSuccess, "Сервер вернул статус 0");
        Assert.That(!products!.Exists(p => p.id == creationResponse.id), "Товар не был удален");
    }

    [Test]
    public void Should_Do_Nothing_When_Deleting_Not_Existing_Product()
    {
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;
        var isSuccess = _service.Delete(notExistingProduct.id);
        Assert.That(!isSuccess, "Сервер вернул статус 1");
    }
}