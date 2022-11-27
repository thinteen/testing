using Lab_3.Domain;
using Moq;

namespace Lab3.Tests
{
    public class Tests
    {
        private Mock<Medicine> CreateMedicineMock(string medicineName, decimal medicinePrice)
        {
            var mockMedicine = new Mock<Medicine>();
            mockMedicine.Object.Name = medicineName;
            mockMedicine.Object.Price = medicinePrice;

            return mockMedicine;
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Should_Add_Medicine()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";
            List<Medicine> medicines = new();

            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineName = "Йод";
            const decimal medicinePrice = 49.89m;

            var medicineMock = CreateMedicineMock(medicineName, medicinePrice);

            //Act
            pharmacy.AddMedicine(medicineMock.Object);

            //Assert
            Assert.That(pharmacy.Medicines.Contains(medicineMock.Object), Is.EqualTo(true));
        }

        [Test]
        public void Should_Throw_ArgumentException_When_Add_Medicine_With_Repeat_Name()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 49.89m;

            Medicine medicineFirst = new(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Йод";
            const decimal medicinePriceSecond = 59.89m;

            Medicine medicineSecond = new(medicineNameSecond, medicinePriceSecond);

            //Act
            pharmacy.AddMedicine(medicineFirst);

            //Assert
            Assert.Throws<ArgumentException>(() => pharmacy.AddMedicine(medicineSecond));
        }
        [Test]
        public void Should_Delete_All_Medicines()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);
        
            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 49.89m;
        
            var medicineMockFirst = CreateMedicineMock(medicineNameFirst, medicinePriceFirst);
        
            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;
        
            var medicineMockSecond = CreateMedicineMock(medicineNameSecond, medicinePriceSecond);
        
            //Act
            pharmacy.AddMedicine(medicineMockFirst.Object);
            pharmacy.AddMedicine(medicineMockSecond.Object);

            pharmacy.DeleteAllMedicines();

            //Assert
            Assert.True(medicines.Count == 0);
        }

        [Test]
        public void Should_Get_Medicine_By_Name()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 49.89m;

            Medicine medicineFirst = new(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;

            Medicine medicineSecond = new(medicineNameSecond, medicinePriceSecond);

            Medicine expectedMedicine = new(medicineNameFirst, medicinePriceFirst);

            //Act
            pharmacy.AddMedicine(medicineFirst);
            pharmacy.AddMedicine(medicineSecond);
            var item = pharmacy.GetMedicineByName(medicineNameFirst);

            //Assert
            Assert.True(item.Name == expectedMedicine.Name && item.Price == expectedMedicine.Price);
        }
        [Test]
        public void Should_Throw_ArgumentException_When_Get_By_NonExistent_Name()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 49.89m;

            Medicine medicineFirst = new(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;

            Medicine medicineSecond = new(medicineNameSecond, medicinePriceSecond);

            const string medicineNameForTest = "Бинт";

            //Act
            pharmacy.AddMedicine(medicineFirst);
            pharmacy.AddMedicine(medicineSecond);

            //Assert
            Assert.Throws<ArgumentException>(() => pharmacy.GetMedicineByName(medicineNameForTest));
        }

        [Test]
        public void Should_Get_Medicines_Ordered_By_Price()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 19.89m;

            Medicine medicineFirst = new(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;

            Medicine medicineSecond = new(medicineNameSecond, medicinePriceSecond);

            const string medicineNameThird = "Бинт";
            const decimal medicinePriceThird = 79.89m;

            Medicine medicineThird = new(medicineNameThird, medicinePriceThird);


            //Act
            pharmacy.AddMedicine(medicineFirst);
            pharmacy.AddMedicine(medicineSecond);
            pharmacy.AddMedicine(medicineThird);

            var orderedMedicines = pharmacy.GetMedicinesOrderedByPrice();

            //Assert
            Assert.True(orderedMedicines[0].Price == medicineThird.Price);
            Assert.True(orderedMedicines[1].Price == medicineSecond.Price);
            Assert.True(orderedMedicines[2].Price == medicineFirst.Price);
        }

        [Test]
        public void Should_Throw_ArgumentException_When_Get_Medicines_Ordered_By_Price()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";

            List<Medicine> medicines = new();
            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 19.89m;

            Medicine medicineFirst = new(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;

            Medicine medicineSecond = new(medicineNameSecond, medicinePriceSecond);

            const string medicineNameThird = "Бинт";
            const decimal medicinePriceThird = 79.89m;

            Medicine medicineThird = new(medicineNameThird, medicinePriceThird);

            //Act

            //Assert
            Assert.Throws<ArgumentException>(() => pharmacy.GetMedicinesOrderedByPrice());
        }

        [Test]
        public void Should_Delete_Medicine_By_Name()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";
            List<Medicine> medicines = new();

            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineNameFirst = "Йод";
            const decimal medicinePriceFirst = 49.89m;

            var medicineMockFirst = CreateMedicineMock(medicineNameFirst, medicinePriceFirst);

            const string medicineNameSecond = "Спирт";
            const decimal medicinePriceSecond = 59.89m;

            var medicineMockSecond = CreateMedicineMock(medicineNameSecond, medicinePriceSecond);

            //Act
            pharmacy.AddMedicine(medicineMockFirst.Object);
            pharmacy.AddMedicine(medicineMockSecond.Object);

            pharmacy.DeleteMedicineByName(medicineMockFirst.Object.Name);

            //Assert

            Assert.That(pharmacy.Medicines.Contains(medicineMockFirst.Object), Is.EqualTo(false));
            Assert.That(pharmacy.Medicines.Contains(medicineMockSecond.Object), Is.EqualTo(true));
        }

        [Test]
        public void Should_Throw_ArgumentException_When_Delete_By_NonExistent_Name()
        {
            //Arrange
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";
            List<Medicine> medicines = new();

            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineName = "Йод";
            const decimal medicinePrice = 49.89m;

            var medicineMock = CreateMedicineMock(medicineName, medicinePrice);

            const string medicineNameForDelete = "Спирт";

            //Act
            pharmacy.AddMedicine(medicineMock.Object);

            //Assert
            Assert.Throws<ArgumentException>(() => pharmacy.DeleteMedicineByName(medicineNameForDelete));
        }
    }
}