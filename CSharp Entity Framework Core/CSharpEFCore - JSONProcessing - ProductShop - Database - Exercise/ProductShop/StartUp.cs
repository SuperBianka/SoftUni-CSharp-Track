using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.ExportDtos;
using ProductShop.ImportDtos;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string usersJson = File.ReadAllText("../../../Datasets/users.json");
            //ImportUsers(context, usersJson);

            //string productsJson = File.ReadAllText("../../../Datasets/products.json");
            //ImportProducts(context, productsJson);

            //string categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //ImportCategories(context, categoriesJson);

            //string categoriesProductsJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //ImportCategoryProducts(context, categoriesProductsJson);

            //string result = GetProductsInRange(context);

            //string result = GetSoldProducts(context);

            //string result = GetCategoriesByProductsCount(context);

            string result = GetUsersWithProducts(context);
            Console.WriteLine(result);
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportUserDto> dtoUsers = JsonConvert.DeserializeObject<ICollection<ImportUserDto>>(inputJson);

            ICollection<User> users = mapper.Map<ICollection<User>>(dtoUsers);

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportProductDto> dtoProducts = JsonConvert.DeserializeObject<ICollection<ImportProductDto>>(inputJson);

            ICollection<Product> products = mapper.Map<ICollection<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportCategoryDto> dtoCategories = JsonConvert.DeserializeObject<ICollection<ImportCategoryDto>>(inputJson);

            ICollection<Category> categories = mapper.Map<ICollection<Category>>(dtoCategories).Where(x => x.Name != null).ToList();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();

            ICollection<ImportCategoryProductDto> dtoCategoryProducts = JsonConvert.DeserializeObject<ICollection<ImportCategoryProductDto>>(inputJson);

            ICollection<CategoryProduct> categoryProducts = mapper.Map<ICollection<CategoryProduct>>(dtoCategoryProducts);

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            List<ExportProductDto> products = context.Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new ExportProductDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    Seller = $"{x.Seller.FirstName} {x.Seller.LastName}"
                })
                .ToList();

            JsonSerializerSettings jsonSettings = MyJsonSettings();

            string jsonProducts = JsonConvert.SerializeObject(products, jsonSettings);

            return jsonProducts;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            List<ExportUserSoldProductsDto> users = context.Users
                .Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Select(x => new ExportUserSoldProductsDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                    .Select(p => new ExportSoldProductDto
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                    .ToList()
                })
                .ToList();

            JsonSerializerSettings jsonSettings = MyJsonSettings();

            string jsonUsers = JsonConvert.SerializeObject(users, jsonSettings);

            return jsonUsers;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            List<ExportCategoryProductsDto> categories = context.Categories
                .OrderByDescending(x => x.CategoryProducts.Count)
                .Select(x => new ExportCategoryProductsDto
                {
                    Category = x.Name,
                    ProductsCount = x.CategoryProducts.Count,
                    AveragePrice = $"{x.CategoryProducts.Average(x => x.Product.Price):F2}",
                    TotalRevenue = $"{x.CategoryProducts.Sum(x => x.Product.Price):F2}"
                })
                .ToList();

            JsonSerializerSettings jsonSettings = MyJsonSettings();

            string jsonCategories = JsonConvert.SerializeObject(categories, jsonSettings);

            return jsonCategories;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            List<ExportUserProductsDto> users = context.Users
                .ToList()
                .Where(x => x.ProductsSold.Any(y => y.BuyerId != null))
                .Select(x => new ExportUserProductsDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProducts = new ExportProductsDto
                    {
                        Count = x.ProductsSold
                        .Count(x => x.BuyerId != null),

                        Products = x.ProductsSold
                        .Where(x => x.BuyerId != null)
                        .Select(x => new ExportProductDto
                        {
                            Name = x.Name,
                            Price = x.Price
                        })
                        .ToList()
                    }
                })
                .OrderByDescending(x => x.SoldProducts.Count)
                .ToList();

            ExportUsersWithSoldProductsDto resultUsers = new ExportUsersWithSoldProductsDto
            {
                UsersCount = users.Count,
                Users = users
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };

            string jsonResultUsers = JsonConvert.SerializeObject(resultUsers, jsonSettings);

            return jsonResultUsers;
        }

        private static JsonSerializerSettings MyJsonSettings()
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            JsonSerializerSettings jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = contractResolver
            };

            return jsonSettings;
        }

        private static void InitializeAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}