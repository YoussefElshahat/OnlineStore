using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Utility;

namespace JOStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            
            IEnumerable<Product> productsList = _unitOfWork
                .Product.GetAll(includeProperties: "Category,ProductImages");
            return View(productsList);
        }
        public IActionResult Details(int ProductId)
        {
            ShoppingCart shoppingCart = new()
            {
                Product = _unitOfWork
                .Product.Get(p => p.Id == ProductId, includeProperties: "Category,ProductImages"),
                Count = 1,
                ProductId = ProductId
            };
            return View(shoppingCart);
        }


        [Authorize]
        [HttpPost]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null || shoppingCart.ProductId == 0)
            {
                TempData["error"] = "Invalid user or product information.";
                return RedirectToAction(nameof(Index));
            }

            var product = _unitOfWork.Product.Get(p => p.Id == shoppingCart.ProductId);
            if (product == null)
            {
                TempData["error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            // Check if the product already exists in the user's cart
            ShoppingCart existingCartItem = _unitOfWork.ShoppingCart.Get(
                sc => sc.AppUserId == userId && sc.ProductId == shoppingCart.ProductId
            );

            if (existingCartItem != null)
            {
                existingCartItem.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(existingCartItem);
            }
            else
            {
                shoppingCart.AppUserId = userId;
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                var userCartCount = _unitOfWork.ShoppingCart.GetAll(sc => sc.AppUserId == userId).Count();
                HttpContext.Session.SetInt32(SD.SessionCart, userCartCount);
            }

           
           

            _unitOfWork.Save();
            TempData["success"] = "Cart Updated Successfully";
            return RedirectToAction(nameof(Index));
        }





        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
  
    }

}
