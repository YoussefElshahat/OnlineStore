using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class Product
    {
        //[Range(1, int.MaxValue, ErrorMessage = "Id Must Be From 1 to 2147483647")]
        public int Id { get; set; }
        [DisplayName("Product Name")]
        public string? Name { get; set; }

        public decimal Price {  get; set; } 
        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }



    }
}
