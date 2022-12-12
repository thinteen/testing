using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APITesting.Domain
{
    public class Product
    {
        public struct ApiResponse
        {
            public ApiResponse(dynamic content)
            {
                this.id = content.id;
                this.status = content.status;
            }

            public string id { get; set; }
            public bool status { get; set; }
        }
        public Product(string id, string category_id, string title, string alias, string content, string price, string old_price, string status, string keywords, string description, string img, string hit, string cat)
        {
            this.id = id;
            this.category_id = category_id;
            this.title = title;
            this.alias = alias;
            this.content = content;
            this.price = price;
            this.old_price = old_price;
            this.status = status;
            this.keywords = keywords;
            this.description = description;
            this.img = img;
            this.hit = hit;
            this.cat = cat;
        }

        public string id { get; set; }
        public string category_id { get; set; }
        public string title { get; set; }
        public string alias { get; set; }
        public string content { get; set; }
        public string price { get; set; }
        public string old_price { get; set; }
        public string status { get; set; }
        public string keywords { get; set; }
        public string description { get; set; }
        public string img { get; set; }
        public string hit { get; set; }
        public string cat { get; set; }

        public Product(Product other) : this(other.id, other.category_id, other.title, other.alias, other.content, other.price, other.old_price, other.status, other.keywords, other.description, other.img, other.hit, other.cat)
        {

        }

        public Product()
        {

        }
    }
}
