﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebFormApp
{
    public class ProductsController : ApiController
    {
        private readonly IList<Product> _products;

        public ProductsController()
        {
            _products = new List<Product>
            {
                new Product { Id = 1, Name = "Tomato Soup", Category = "Groceries", Price = 1 },
                new Product { Id = 2, Name = "Yo-yo", Category = "Toys", Price = 3.75M },
                new Product { Id = 3, Name = "Hammer", Category = "Hardware", Price = 16.99M }
            };
        }

        public IEnumerable<Product> GetAllProducts()
        {
            return _products;
        }

        public Product GetProductById(int id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if(product == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return product;
        }

        public IEnumerable<Product> GetProductByCategory(string category)
        {
            var products = _products.Where(p=>string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));

            return products;
        }

        
    }
}