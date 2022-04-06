﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProductGrpc.Models;
using System.Linq;

namespace ProductGrpc.Data
{
    public class ProductsContextSeed
    {
        public static async Task SeedAsync(ProductsContext productsContext)
        {
            if (!productsContext.Products.Any())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        Id = 1,
                        Name = "Mi10T",
                        Description = "New Xiaomi Phone Mi10T",
                        Price = 699,
                        Status = ProductGrpc.Models.ProductStatus.INSTOCK,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 2,
                        Name = "P40",
                        Description = "New Huawei Phone P40",
                        Price = 899,
                        Status = ProductGrpc.Models.ProductStatus.INSTOCK,
                        CreatedTime = DateTime.UtcNow
                    },
                    new Product
                    {
                        Id = 3,
                        Name = "A50",
                        Description = "New Samsung Phone A50",
                        Price = 399,
                        Status = ProductGrpc.Models.ProductStatus.INSTOCK,
                        CreatedTime = DateTime.UtcNow
                    }
                };
                productsContext.Products.AddRange(products);
                await productsContext.SaveChangesAsync();
            }
        }
    }
}