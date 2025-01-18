using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        [Required]
        [ForeignKey("OrderHeaderId")]
        [ValidateNever]
        public int OrderHeaderId { get; set; }
        public OrderHeader OrderHeader { get; set; }
        [ForeignKey("ProductId")]
        [ValidateNever]
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Count { get; set; }
        public double Price     { get; set; }
    }
}
