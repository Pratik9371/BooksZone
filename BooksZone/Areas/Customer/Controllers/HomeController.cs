using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BooksZone.Models;
using BooksZone.Models.ViewModels;
using BooksZone.DataAccess.Repository;
using BooksZone.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using BooksZone.Utility;

namespace BooksZone.Areas.Customer.Controllers
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
            IEnumerable<Product> productList = _unitOfWork.Product.GetAll();

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim != null)  //This means user is signed in
            {
                var count = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == claim.Value).ToList().Count();
                    
                HttpContext.Session.SetInt32("CartCount", count);
            }

            return View(productList);
        }

        public IActionResult Details(int id)
        {
            var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == id, includeProperties: "Category,CoverType");
            ShoppingCart cartObj = new ShoppingCart()
            {
                Product = productFromDb,          //Product is from shopping cart(Product)
                ProductId = productFromDb.Id
            };
            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart CartObj)
        {
            CartObj.Id = 0;
            if (ModelState.IsValid)
            {
                //then we will add to cart
                //we need Id of the logged in user
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                //Claim.Value will have the actual user id of the logged in user
                CartObj.ApplicationUserId = claim.Value;

                //Now lets retrieve the shopping cart from the database based on userId and ProductId
                ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.GetFirstOrDefault(
                    u => u.ApplicationUserId == CartObj.ApplicationUserId && u.ProductId == CartObj.ProductId
                    );

                if (cartFromDb == null)
                {
                    //no record exists in the database for that product for that user(add to db)
                    _unitOfWork.ShoppingCart.Add(CartObj);
                }
                else
                {
                    //Update the cart in db
                    cartFromDb.Count += CartObj.Count;
                    _unitOfWork.ShoppingCart.Update(cartFromDb);
                }
                _unitOfWork.Save();

                //(Cart Count)Taking the count from the database of the user of all his added products
                var count = _unitOfWork.ShoppingCart.GetAll(i => i.ApplicationUserId == CartObj.ApplicationUserId).ToList().Count();

                //Storing the Count value in the session
                HttpContext.Session.SetInt32("CartCount", count);

                return RedirectToAction("Index");
            }
            else
            {
                //return back to the view
                var productFromDb = _unitOfWork.Product.GetFirstOrDefault(u => u.Id == CartObj.ProductId, includeProperties: "Category,CoverType");
                ShoppingCart cartObj = new ShoppingCart()
                {
                    Product = productFromDb,
                    ProductId = productFromDb.Id
                };
                return View(cartObj);
            }
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
