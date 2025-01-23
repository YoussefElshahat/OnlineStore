using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;
using Stripe.Checkout;
using System.Security.Claims;

namespace JOStore.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            // Create the ViewModel
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.AppUserId == userId,
                includeProperties: "Product"),
                OrderHeader = new()
            };

            // Calculate the total order price
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (double)cart.Product.Price * cart.Count;
            }

            return View(shoppingCartVM);
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account"); // Handle missing user ID

            var userId = userIdClaim.Value;

            // Create the ViewModel
            ShoppingCartVM = new ShoppingCartVM
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(
                    sc => sc.AppUserId == userId,
                    includeProperties: "Product"
                ),
                OrderHeader = new OrderHeader
                {
                    AppUser = _unitOfWork.AppUser.Get(a => a.Id == userId),
                }
            };

            // Calculate the total order price
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.ShoppingCartList
                .Sum(cart => (double)cart.Product.Price * cart.Count);

            // Pre-fill OrderHeader properties with AppUser data if necessary
            var appUser = ShoppingCartVM.OrderHeader.AppUser;
            if (appUser != null)
            {
                ShoppingCartVM.OrderHeader.Name = appUser.Name;
                ShoppingCartVM.OrderHeader.PhoneNumber = appUser.PhoneNumber;
                ShoppingCartVM.OrderHeader.PostalCode = appUser.PostalCode;
                ShoppingCartVM.OrderHeader.City = appUser.City;
                ShoppingCartVM.OrderHeader.Region = appUser.Region;
                ShoppingCartVM.OrderHeader.StreetAddress = appUser.StreetAddress;
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return RedirectToAction("Login", "Account"); // Handle missing user ID

            var userId = userIdClaim.Value;


            // Create the ViewModel
            ShoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sc => sc.AppUserId == userId
                                            , includeProperties: "Product");
            AppUser appUser = _unitOfWork.AppUser.Get(u => u.Id == userId);

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.AppUserId = userId;
            // Calculate the total order price
            ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.ShoppingCartList
                .Sum(cart => (double)cart.Product.Price * cart.Count);
            if (appUser.CompanyId.GetValueOrDefault() == null)
            {
                //it is regular user and we need to capture payment 
                ShoppingCartVM.OrderHeader.PayementStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
                ShoppingCartVM.OrderHeader.PayementStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.Id,
                    Price = (double)cart.Product.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();

            }
            if (appUser.CompanyId.GetValueOrDefault() == 0)
            {
                //it is regular user and we need to capture payment 
                //Stripe Logic
                var domain = "https://localhost:7002/";

                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain+ $"Customer/Cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                    CancelUrl = domain+"customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(), 
                    Mode = "payment",
                };
                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Product.Price * 100),//20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);                    
                }
                var service = new SessionService();
                Session session =  service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);



            }
            return RedirectToAction(nameof(OrderConfirmation)
                , new { id = ShoppingCartVM.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            // Fetch the order details
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(x => x.Id == id, includeProperties: "AppUser");

            if (orderHeader.OrderStatus != SD.PaymentStatusDelayedPayment)
            {
                // Regular customer order, check payment status via Stripe
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLowerInvariant() == "paid")
                {
                    // Update the payment and order details
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                }

                // Clear the user's shopping cart
                List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                    .GetAll(u => u.AppUserId == orderHeader.AppUserId)
                    .ToList();
                _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);

                // Save all changes at once
                _unitOfWork.Save();
            }

            // Return confirmation view with the order ID
            return View(id);
        }

        public IActionResult Plus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);
            cartFromDb.Count += 1;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);

            if (cartFromDb.Count == 1)
            {
                // If the count is 1, deleting the cart item will result in a count of 0
                _unitOfWork.ShoppingCart.Delete(cartFromDb);
            }
            else
            {
                // Otherwise, decrement the count
                cartFromDb.Count -= 1;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.Get(sc => sc.Id == cartId);
            _unitOfWork.ShoppingCart.Delete(cartFromDb);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
