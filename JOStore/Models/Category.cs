using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JOStore.Models
{
    public class Category
    {
        [Range(1, int.MaxValue,ErrorMessage = "Id Must Be From 1 to 2147483647")]
        public int Id { get; set; }
        [DisplayName("Category Name")]
        public string Name { get; set; }

    }
}
