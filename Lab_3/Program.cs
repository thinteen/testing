using Lab_3.Domain;

namespace Lab_3
{
    class Program
    {
        static void Main(string[] args)
        {
            const string pharmacyName = "Дежурный аптекарь";
            const string pharmacyAddress = "г.Йошкар-Ола";
            List<Medicine> medicines = new();

            Pharmacy pharmacy = new(pharmacyName, pharmacyAddress, medicines);

            const string medicineName = "Йод";
            const string medicineName2 = "Спирт";
            const decimal medicinePrice = 49.89m;
            const decimal medicinePrice2 = 59.89m;

            Medicine medicine = new(medicineName, medicinePrice);

            //pharmacy.AddMedicine(medicine);
            //pharmacy.DeleteMedicineByName(medicineName2);
            //Assert.True(pharmacy.GetAll()[0].Name == expectedMedicines[0].Name);
            Console.WriteLine(medicines.Count);
        }
    }
}