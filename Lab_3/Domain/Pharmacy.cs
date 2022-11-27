using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab_3.Domain
{
    public class Pharmacy
    {
        public Pharmacy(string name, string address, List<Medicine> medicines)
        {
            Name = name;
            Address = address;
            Medicines = medicines;
        }

        public string Name { get; set; }
        public string Address { get; set; }
        public List<Medicine> Medicines { get; set; }

        public void AddMedicine(Medicine medicine)
        {
            if (Medicines.FirstOrDefault(item => item.Name == medicine.Name) != null)
            {
                throw new ArgumentException();
            }

            Medicines.Add(medicine);
        }

        public void DeleteAllMedicines()
        {
            Medicines.Clear();
        }

        public Medicine GetMedicineByName(string name)
        {
            Medicine medicine = Medicines.FirstOrDefault(item => item.Name == name);

            if (medicine == null)
            {
                throw new ArgumentException($"Student with name {name} was not found ");
            }

            return medicine;
        }

        public List<Medicine> GetMedicinesOrderedByPrice()
        {
            var orderedMedicines = Medicines.OrderByDescending(item => item.Price).ToList();

            if (orderedMedicines.Count == 0)
            {
                throw new ArgumentException();
            }

            return orderedMedicines;
        }

        public void DeleteMedicineByName(string name)
        {
            Medicine medicine = Medicines.FirstOrDefault(item => item.Name == name);

            if (medicine == null)
            {
                throw new ArgumentException($"Student with name {name} was not found ");
            }

            Medicines.Remove(medicine);
        }
    }
}
