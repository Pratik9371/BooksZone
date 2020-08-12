using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksZone.DataAccess.Repository;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using BooksZone.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BooksZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Emp)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
          var allObj = _unitOfWork.Company.GetAll();
          return Json(new { data = allObj });
        }

        //Company Upsert Get Action 
        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                //This for Create 
                //We will return the empty object that we created(category)
                return View(company);
            }
            //This for Edit
            company = _unitOfWork.Company.Get(id.GetValueOrDefault()); //Reason we are using GetValueOrDefault because id could br null
            if (company == null)
            {
                return NotFound();
            }
            //Else
            return View(company);
        }

        [HttpPost]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    //Create
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    //Update
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var result = _unitOfWork.Company.Get(id);
            if (result == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(result);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}