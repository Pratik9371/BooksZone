using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using BooksZone.Models.ViewModels;
using BooksZone.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        [BindProperty]
        public OrderDetailsVM OrderDetailsVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int id)
        {
            OrderDetailsVM = new OrderDetailsVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id,includeProperties:"ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetails.GetAll(o=>o.OrderId == id,includeProperties:"Product")
            };
            return View(OrderDetailsVM);
        }

        public IActionResult StartProcessing(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);
            orderHeader.OrderStatus = SD.OrderStatusInProcess;
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ShipOrder()
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == OrderDetailsVM.OrderHeader.Id);

            orderHeader.OrderStatus = SD.OrderStatusShipped;
            orderHeader.TrackingNumber = OrderDetailsVM.OrderHeader.TrackingNumber;
            orderHeader.ShippingDate = DateTime.Now;
            orderHeader.Carrier = OrderDetailsVM.OrderHeader.Carrier;

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        public IActionResult CancelOrder(int id)
        {
           OrderHeader orderHeader =  _unitOfWork.OrderHeader.GetFirstOrDefault(u => u.Id == id);

            orderHeader.OrderStatus = SD.OrderStatusCancelled;

            _unitOfWork.Save();

            return RedirectToAction("Index");

        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetOrdersList(string status)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            IEnumerable<OrderHeader> orderHeaderList;
            orderHeaderList = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");

            switch (status)
            {
                case "pending":
                    orderHeaderList = orderHeaderList.Where(o => o.PaymentStatus == SD.PaymentStatusDelayedPayment || 
                                                                 o.OrderStatus == SD.OrderStatusPending);
                    break;
                case "inprocess":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.OrderStatusInProcess);
                    break;
                case "completed":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.OrderStatusShipped);
                    break;
                case "rejected":
                    orderHeaderList = orderHeaderList.Where(o => o.OrderStatus == SD.OrderStatusCancelled || 
                                                                 o.OrderStatus == SD.OrderStatusRefunded ||
                                                                 o.OrderStatus == SD.PaymentStatusRejected);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaderList });
        }

        #endregion
    }
}