using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Utility;

namespace JOStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public CategoryController(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;

        }
        public IActionResult Index()
        {
            List<Category> categoryList = _iUnitOfWork.Category.GetAll().ToList();
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();

        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _iUnitOfWork.Category.Add(category);
                _iUnitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }

        public IActionResult Edit(int? Id)
        {
            if (Id.HasValue == false)
            {
                return NotFound();
            }

            Category? categoryFromDb =
                _iUnitOfWork.Category.Get(x => x.Id == Id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);

        }
        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                _iUnitOfWork.Category.Update(category);
                _iUnitOfWork.Save();
                TempData["Success"] = "Category Updated Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }
        public IActionResult Delete(int? Id)
        {
            if (Id.HasValue == false)
            {
                return NotFound();
            }

            Category? categoryFromDb = _iUnitOfWork.Category.Get(x => x.Id == Id);


            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);

        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = _iUnitOfWork.Category.Get(x => x.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            _iUnitOfWork.Category.Delete(category);
            _iUnitOfWork.Save();
            TempData["Alert"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }

    }
}
