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
        Assert.That(products is not null, "������ ��������� ����");
    }

    [Test]
    public void Should_Successfully_Add_Product()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        _productIds.Add(creationResponse.id);

        var products = _service.GetAll();
        var createdProduct = _service.Get(creationResponse.id);

        Assert.That(creationResponse.id != "-1", "������ �� �������");
        Assert.That(products!.Find(p => p.id == creationResponse.id) is not null, "������� �� ��� ������");
        Assert.That(AreEqual(productToCreate, createdProduct), "��� �������� ������� ��� �������");
    }

    [Test]
    public void Should_Successfully_Get_Product_By_Id()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        var product = _service.Get(creationResponse.id);

        _productIds.Add(creationResponse.id);

        Assert.That(product is not null, "���������� ������� ����");
        Assert.That(AreEqual(productToCreate, product), "������� ��� ������� ��� ���������");
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

        Assert.That(creationResponse1.id != "-1", "������ �� �������");
        Assert.That(creationResponse2.id != "-1", "������ �� �������");
        Assert.That(products!.Find(p => p.id == creationResponse1.id) is not null, "������ ������� �� ��� ��������");
        Assert.That(products.Find(p => p.id == creationResponse2.id) is not null, "������ ������� �� ��� ��������");
        Assert.That(AreEqual(productToCreate1, createdProduct1), "������ ������� ��� ������� ��� ��������");
        Assert.That(AreEqual(productToCreate2, createdProduct2), "������ ������� ��� ������� ��� ��������");
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

        Assert.That(creationResponse1.id != "-1", "������ �� �������");
        Assert.That(creationResponse2.id != "-1", "������ �� �������");
        Assert.That(createdProducts[0] is not null, "������ ������� ����");
        Assert.That(createdProducts[1] is not null, "������ ������� ����");
        Assert.That(createdProducts[1]!.alias == createdProducts[0]!.alias + "-0", "������ ��������� �� ���������� �� -0");
        Assert.That(AreEqual(productToCreate1, createdProducts[0]), "������ ������� ��� ������� ��� ��������");
        Assert.That(AreEqual(productToCreate2, createdProducts[1]), "������ ������� ��� ������� ��� ��������");

    }

    [Test]
    public void Should_Not_Add_Invalid_Product()
    {
        var productToCreate = _config.GetSection("invalid_product").Get<Product>()!;

        var creationResponse = _service.Create(productToCreate);

        var products = _service.GetAll();

        Assert.That(!products!.Exists(p => p.id == creationResponse.id), "������� ��� ������");
        Assert.That(!creationResponse.status, "����� �� ��� ������, �� api ������� �� ��������");
        Assert.That(creationResponse.id == "-1", "������ �� �������");
    }

    [Test]
    public void Should_Not_Add_Null_Product()
    {
        var productToCreate = _config.GetSection("null_product").Get<Product>();

        var creationResponse = _service.Create(productToCreate);

        var products = _service.GetAll();

        Assert.That(products!.Find(p => p.id == creationResponse.id) is null, "������� ��� ������");
        Assert.That(!creationResponse.status, "������ ������ ������ 1");
        Assert.That(creationResponse.id == "-1", "������ �� �������");
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

        Assert.That(isSuccess, "������ ������ ������ 0");
        Assert.That(AreEqual(updatedProduct, updater), "����������� ����� ���������� �� ������, ������� �� ���������");
        Assert.That(!updatedProduct.alias.Contains(temp.id), "����� ������������ ������ �������� ���� ������");

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

        Assert.That(isSuccess, "������ ������ ������ 0");
        Assert.That(AreEqual(updatedProduct, updater), "����������� ����� ���������� �� ������, ������� �� ���������");
        Assert.That(updatedProduct?.alias != updater.alias, "����� ������������ ������ ��������� � ������� ������ ��� ����������");
        Assert.That(updatedProduct.alias.Contains(temp.id), "����� ������������ ������ �� �������� ���� ������");

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

        Assert.That(!isSuccess, "��� ������ ����� �������");
        Assert.That(updatedProduct is null, "������� ��� ��������");
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

        Assert.That(updatedProduct is not null, "������� ��� ������, �� ����� ���������");
        Assert.That(!isSuccess, "������ ������ ������ 1");
        Assert.That(AreEqual(updatedProduct, productToUpdate), "������� ��� �������");
        Assert.That(updatedProduct?.alias == productToUpdate.alias, "����� �������� ��� �������");
    }


    [Test]
    public void Should_Successfully_Delete_Existing_Product()
    {
        var productToCreate = _config.GetSection("valid_product_1").Get<Product>()!;
        var creationResponse = _service.Create(productToCreate);

        var isSuccess = _service.Delete(creationResponse.id);


        _productIds.Add(creationResponse.id);

        var products = _service.GetAll();
        Assert.That(isSuccess, "������ ������ ������ 0");
        Assert.That(!products!.Exists(p => p.id == creationResponse.id), "����� �� ��� ������");
    }

    [Test]
    public void Should_Do_Nothing_When_Deleting_Not_Existing_Product()
    {
        var notExistingProduct = _config.GetSection("not_existing_product").Get<Product>()!;
        var isSuccess = _service.Delete(notExistingProduct.id);
        Assert.That(!isSuccess, "������ ������ ������ 1");
    }
}