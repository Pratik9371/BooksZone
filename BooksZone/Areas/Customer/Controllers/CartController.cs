using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using BooksZone.Models.ViewModels;
using BooksZone.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace BooksZone.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;

        private readonly UserManager<IdentityUser> _userManager;
        private object returnUrl;

        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserId == claim.Value,includeProperties: "Product")
            };

            //Retrieving the Application user and storing it in VM OrderHeader.ApplicationUser
            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);
           
            foreach(var list in ShoppingCartVM.ListCart)
            {
                //Adding the overall order total to OrderHeader OrderTotal
                ShoppingCartVM.OrderHeader.OrderTotal += list.Product.Price * list.Count;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartId)
        {
           var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties:"Product");
            cart.Count += 1;

            var result = cart.Product.Price * cart.Count;
            cart.Price = result;

            //HttpContext.Session.SetString("UpdatedPrice", result);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        } 

        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Product");
            cart.Count -= 1;

            var result = cart.Product.Price * cart.Count;
            cart.Price = result;

            //HttpContext.Session.SetString("UpdatedPrice", result);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(u => u.Id == cartId, includeProperties: "Product");

            _unitOfWork.ShoppingCart.Remove(cart);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new Models.OrderHeader(),
                ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,
                                                          includeProperties:"Product")
            };

            //first we need Application User & we want to populate to the orderheader application user

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser
                                                         .GetFirstOrDefault(u => u.Id == claim.Value,
                                                         includeProperties:"Company");

            //Adding the Application User data in OrderHeader.ApplicationUser
            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.StreetAddress = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAddress;

            //Now we want OrderTotal
            foreach (var list in ShoppingCartVM.ListCart)
            {
                ShoppingCartVM.OrderHeader.OrderTotal += list.Product.Price * list.Count;
            }

            //Now We will populate the orderheader properties from application user

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);

            ShoppingCartVM.ListCart = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value,includeProperties:"Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            //Lets add OrderHeader to the database
            _unitOfWork.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _unitOfWork.Save();

            //OrderDeatils
            foreach(var item in ShoppingCartVM.ListCart)
            {
                OrderDetails orderDetails = new OrderDetails()
                {
                    ProductId = item.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Product.Price * item.Count
                };
                ShoppingCartVM.OrderHeader.OrderTotal += item.Product.Price * item.Count;
                _unitOfWork.OrderDetails.Add(orderDetails);
                _unitOfWork.Save();
            }

            _unitOfWork.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.Save();

            HttpContext.Session.SetInt32("CartCount", 0);
            return RedirectToAction("OrderConfrimation","Cart",new { id = ShoppingCartVM.OrderHeader.Id } );
        }

        public IActionResult OrderConfrimation(int id)
        {
            return View(id);
        }


        [HttpPost]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(u => u.Id == claim.Value);


            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "verification email is empty");
            }


            //For Resending the email confirmation
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code},
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            //Show Sent message
            ModelState.AddModelError(string.Empty, "Verfication email sent. Please check your email");
            return RedirectToAction("Index");
        }
    }
}