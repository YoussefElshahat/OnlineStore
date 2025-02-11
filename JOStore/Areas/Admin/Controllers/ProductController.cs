using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;

namespace JOStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

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
                .Product.GetAll(includeProperties: "Category").ToList();
            return View(productList);
        }

        public IActionResult Upsert(int? Id)
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
                productVM.Product = _iUnitOfWork.Product.Get(x => x.Id == Id,includeProperties: "ProductImages");
                return View(productVM);

            }
        }
        [HttpPost]
        public IActionResult Upsert(Product product, List<IFormFile>? files)
        {

            if (ModelState.IsValid)
            {
                if (product.Id == 0)
                {
                    _iUnitOfWork.Product.Add(product);

                }
                else
                {
                    _iUnitOfWork.Product.Update(product);

                }
                _iUnitOfWork.Save();
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString()
                        + Path.GetExtension(file.FileName);
                        string productPath = @"images\products\product"+product.Id;
                        string finalPath = Path.Combine(wwwRootPath, productPath);
                        if (!Directory.Exists(finalPath))
                          Directory.CreateDirectory(finalPath);

                        using (var fileStream = new FileStream(Path.
                        Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }
                        ProductImage productImage = new()
                        {
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId = product.Id
                        };
                        if(product.ProductImages == null)
                        {
                            product.ProductImages = new List<ProductImage>();
                        }
                        product.ProductImages.Add(productImage);
                        
                    }
                    _iUnitOfWork.Product.Update(product);
                    _iUnitOfWork.Save();

                }
                
                TempData["Success"] = "Product Created/updated Successfully";
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


        private IEnumerable<SelectListItem> GetCategoryList()
        {
            return _iUnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _iUnitOfWork.Product.GetAll(includeProperties: "Category")
                .Select(product => new
                {
                    name = product.Name,
                    price = product.Price,
                    category = product.Category?.Name, // Avoid full Category object
                    //imageUrl = product.ImageUrl,
                    id = product.Id,

                });

            return Json(new { data = products });
        }

        public IActionResult DeleteImage(int imageId)
        {
            var imageToBeDeleted = _iUnitOfWork.ProductImage.Get(x => x.Id == imageId);

            if (imageToBeDeleted == null)
            {
                TempData["Error"] = "Image not found";
                return RedirectToAction(nameof(Upsert));
            }

            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var imagePath = Path.Combine(
                    _webHostEnvironment.WebRootPath,
                    imageToBeDeleted.ImageUrl.TrimStart('\\')
                );

                if (System.IO.File.Exists(imagePath))
                {
                    try
                    {
                        System.IO.File.Delete(imagePath);
                    }
                    catch (Exception ex)
                    {
                        TempData["Error"] = $"Error deleting image: {ex.Message}";
                        return RedirectToAction(nameof(Upsert), new { id = imageToBeDeleted.ProductId });
                    }
                }
            }

            _iUnitOfWork.ProductImage.Delete(imageToBeDeleted);
            _iUnitOfWork.Save();

            TempData["Success"] = "Image Deleted Successfully";
            return RedirectToAction(nameof(Upsert), new { id = imageToBeDeleted.ProductId });
        }

        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _iUnitOfWork.Product.Get(x => x.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            string productPath = Path.Combine("images", "products", "product" + id);
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                try
                {
                    Directory.Delete(finalPath, true);
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"Error deleting directory: {ex.Message}" });
                }
            }

            _iUnitOfWork.Product.Delete(productToBeDeleted);
            _iUnitOfWork.Save();

            return Json(new { success = true, message = "Delete Success" });
        }

        #endregion

    }

}
