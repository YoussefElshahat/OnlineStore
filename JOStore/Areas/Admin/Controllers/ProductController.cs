using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;

namespace JOStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public ProductController(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;

        }
        public IActionResult Index()
        {
            List<Product> productList = _iUnitOfWork.Product.GetAll().ToList();
            return View(productList);
        }

        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = GetCategoryList(),
                Product = new Product()
    
            };
            return View(productVM);
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (_iUnitOfWork.Product.GetAll().Any(x => x.Id == product.Id))
            {
                ModelState.AddModelError("ID", "ID Exist Already");
            }

            if (ModelState.IsValid)
            {
                _iUnitOfWork.Product.Add(product);
                _iUnitOfWork.Save();
                TempData["Success"] = "Product Created Successfully";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                ProductVM productVM = new ProductVM()
                {
                    CategoryList = GetCategoryList(),
                    Product = new Product()
                };
                return View(productVM);
            }


        }

        public IActionResult Edit(int? Id)
        {
            if (Id.HasValue == false)
            {
                return NotFound();
            }

            Product? productFromDb =
                _iUnitOfWork.Product.Get(x => x.Id == Id);


            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);

        }
        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                _iUnitOfWork.Product.Update(product);
                _iUnitOfWork.Save();
                TempData["Success"] = "Product Updated Successfully";
                return RedirectToAction("Index", "Product");
            }
            return View();

        }
        public IActionResult Delete(int? Id)
        {
            if (Id.HasValue == false)
            {
                return NotFound();
            }

            Product? productFromDb = _iUnitOfWork.Product.Get(x => x.Id == Id);


            if (productFromDb == null)
            {
                return NotFound();
            }

            return View(productFromDb);

        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var product = _iUnitOfWork.Product.Get(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            _iUnitOfWork.Product.Delete(product);
            _iUnitOfWork.Save();
            TempData["Alert"] = "Product Deleted Successfully";
            return RedirectToAction("Index", "Product");
        }
        private IEnumerable<SelectListItem> GetCategoryList()
        {
            return _iUnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

    }
}
