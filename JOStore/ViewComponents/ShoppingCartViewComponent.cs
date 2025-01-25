using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository.IRepository;
using Store.Utility;
using System.Security.Claims;

namespace JOStore.ViewComponents
{
    public class ShoppingCartViewComponent :ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;

        public ShoppingCartViewComponent(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (userId != null)
            {
                if (HttpContext.Session.GetInt32(SD.SessionCart) == null)
                {
                    var userCartCount = _unitOfWork.ShoppingCart
                        .GetAll(sc => sc.AppUserId == userId.Value).Count();
                    HttpContext.Session.SetInt32(SD.SessionCart, userCartCount);
                    return View(HttpContext.Session.GetInt32(SD.SessionCart));
                }
               
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.Clear();
                return View(0);
            }
        }
    }
}
