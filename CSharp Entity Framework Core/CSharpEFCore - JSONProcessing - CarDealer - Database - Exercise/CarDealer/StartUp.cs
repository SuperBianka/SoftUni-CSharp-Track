using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.ExportDtos;
using CarDealer.DTO.ImportDtos;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            //ImportSuppliers(context, suppliersJson);

            //string partsJson = File.ReadAllText("../../../Datasets/parts.json");
            //ImportParts(context, partsJson);

            //string carsJson = File.ReadAllText("../../../Datasets/cars.json");
            //ImportCars(context, carsJson);

            //string customersJson = File.ReadAllText("../../../Datasets/customers.json");
            //ImportCustomers(context, customersJson);

            //string salesJson = File.ReadAllText("../../../Datasets/sales.json");
            //string result = ImportSales(context, salesJson);
            //Console.WriteLine(result);

            //string result = GetOrderedCustomers(context);

            //string result = GetCarsFromMakeToyota(context);

            //string result = GetLocalSuppliers(context);

            //string result = GetCarsWithTheirListOfParts(context);

            //string result = GetTotalSalesByCustomer(context);

            string result = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(result);
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportSupplierDto> dtoSuppliers = JsonConvert.DeserializeObject<ICollection<ImportSupplierDto>>(inputJson);

            ICollection<Supplier> suppliers = mapper.Map<ICollection<Supplier>>(dtoSuppliers);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            
            return $"Successfully imported {suppliers.Count}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportPartDto> dtoParts = JsonConvert.DeserializeObject<ICollection<ImportPartDto>>(inputJson)
                .Where(x => context.Suppliers.Select(y => y.Id).Contains(x.SupplierId))
                .ToList();

            ICollection<Part> parts = mapper.Map<ICollection<Part>>(dtoParts);

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportCarDto> dtoCars = JsonConvert.DeserializeObject<ICollection<ImportCarDto>>(inputJson);
                
            ICollection<Car> cars = mapper.Map<ICollection<Car>>(dtoCars);

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportCustomerDto> dtoCustomers = JsonConvert.DeserializeObject<ICollection<ImportCustomerDto>>(inputJson);

            ICollection<Customer> customers = mapper.Map<ICollection<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportSaleDto> dtoSales = JsonConvert.DeserializeObject<ICollection<ImportSaleDto>>(inputJson);

            ICollection<Sale> sales = mapper.Map<ICollection<Sale>>(dtoSales);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            List<ExportCustomerDto> customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new ExportCustomerDto
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    IsYoungDriver = x.IsYoungDriver
                })
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                DateFormatString = "dd/MM/yyyy"
            };

            string jsonCustomers = JsonConvert.SerializeObject(customers, jsonSettings);

            return jsonCustomers;
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(x => x.Make == "Toyota")
                .OrderBy(x => x.Model)
                .ThenByDescending(x => x.TravelledDistance)
                .Select(x => new ExportCarDto
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance
                })
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonCars = JsonConvert.SerializeObject(cars, jsonSettings);

            return jsonCars;
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            List<ExportSupplierDto> suppliers = context.Suppliers
                .Where(x => !x.IsImporter)
                .Select(x => new ExportSupplierDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                })
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonSuppliers = JsonConvert.SerializeObject(suppliers, jsonSettings);

            return jsonSuppliers;
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            List<ExportCarPartsDto> carsWithParts = context.Cars
                .Select(x => new ExportCarPartsDto
                {
                    Car = new ExportOnlyCarDto
                    {
                        Make = x.Make,
                        Model = x.Model,
                        TravelledDistance = x.TravelledDistance
                    },
                    Parts = x.PartCars
                    .Select(p => new ExportPartDto
                    {
                        Name = p.Part.Name,
                        Price = $"{p.Part.Price:F2}"
                    })
                    .ToList()
                })
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented               
            };

            string jsonCarsWithParts = JsonConvert.SerializeObject(carsWithParts, jsonSettings);

            return jsonCarsWithParts;
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            List<ExportCustomerSalesDto> customerTotalSales = context.Customers
                .Where(x => x.Sales.Count > 0)
                .Select(x => new ExportCustomerSalesDto
                {
                    FullName = x.Name,
                    BoughtCars = x.Sales.Count,
                    SpentMoney = x.Sales
                    .SelectMany(x => x.Car.PartCars)
                    .Sum(y => y.Part.Price)
                })
                .OrderByDescending(x => x.SpentMoney)
                .ThenByDescending(x => x.BoughtCars)
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonCustomerTotalSales = JsonConvert.SerializeObject(customerTotalSales, jsonSettings);

            return jsonCustomerTotalSales;
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            List<ExportSaleInformationDto> salesWithDiscount = context.Sales
                .Select(x => new ExportSaleInformationDto
                {
                    Car = new ExportOnlyCarDto
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TravelledDistance = x.Car.TravelledDistance
                    },
                    CustomerName = x.Customer.Name,
                    Discount = $"{x.Discount:F2}",
                    Price = $"{x.Car.PartCars.Sum(y => y.Part.Price):F2}",
                    PriceWithDiscount = $"{x.Car.PartCars.Sum(y => y.Part.Price) * (1 - (x.Discount / 100)):F2}"
                })
                .Take(10)
                .ToList();

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };

            string jsonSalesWithDiscount = JsonConvert.SerializeObject(salesWithDiscount, jsonSettings);

            return jsonSalesWithDiscount;
        }

        private static void InitializeAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}