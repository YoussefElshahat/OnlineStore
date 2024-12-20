using JOStore.Data;
using Microsoft.AspNetCore.Mvc;
using Store.Models;

namespace JOStore.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public CategoryController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            
        }
        public IActionResult Index()
        {
            List<Category> categoryList = _appDbContext.Categories.ToList();
            return View(categoryList);
        }

        public IActionResult Create ()
        {
            return View();
        
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                _appDbContext.Categories.Add(category);
                _appDbContext.SaveChanges();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index", "Category");
            }
            return View();

        }

        public IActionResult Edit(int? Id)
        {
            if(Id.HasValue == false)
            {
                return NotFound();
            }

            Category? categoryFromDb = _appDbContext.
                Categories.Find(Id); //Find is more efficient than FirstOrDefault
      
            //because it checks the primary key directly
            //and utilizes the DbContext's in-memory
            //tracking before querying the database.

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
                _appDbContext.Categories.Update(category);
                _appDbContext.SaveChanges();
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

            Category? categoryFromDb = _appDbContext.
                Categories.Find(Id); //Find is more efficient than FirstOrDefault

            //because it checks the primary key directly
            //and utilizes the DbContext's in-memory
            //tracking before querying the database.

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);

        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent CSRF attacks
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id) { 
            var category = _appDbContext.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            _appDbContext.Categories.Remove(category);
            _appDbContext.SaveChanges();
            TempData["Alert"] = "Category Deleted Successfully";
            return RedirectToAction("Index", "Category");
        }

    }
}
