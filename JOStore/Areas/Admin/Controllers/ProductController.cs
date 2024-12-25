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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork iUnitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _iUnitOfWork = iUnitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            List<Product> productList = _iUnitOfWork
                .Product.GetAll(includeProperties:"Category").ToList();
            return View(productList);
        }

        public IActionResult Upsert(int? Id )
        {
            ProductVM productVM = new ProductVM()
            {
                CategoryList = GetCategoryList(),
                Product = new Product()
    
            };
            if (Id == null || Id == 0) 
            {
                //Create
                return View(productVM);

            }
            else
            {
                //update
                productVM.Product = _iUnitOfWork.Product.Get(x => x.Id == Id);
                return View(productVM);

            }
        }
        [HttpPost]
        public IActionResult Upsert(Product product,IFormFile? file)
        {
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null) 
                {
                    string fileName = Guid.NewGuid().ToString() 
                        + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, "Images", "Product");
                    if (!string.IsNullOrEmpty(product.ImageUrl))
                    {
                        //Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, product.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        
                    }

                    using (var fileStream = new FileStream(Path.
                        Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);

                    }
                     product.ImageUrl = "/Images/Product/" + fileName;

                }
                if(product.Id == 0)
                {
                    _iUnitOfWork.Product.Add(product);

                }
                else
                {
                    _iUnitOfWork.Product.Update(product);

                }
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
