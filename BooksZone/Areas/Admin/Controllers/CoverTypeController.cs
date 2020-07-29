using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksZone.DataAccess.Repository;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooksZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CoverTypeController(IUnitOfWork unitOfWork)
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
            var allObj = _unitOfWork.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        //Category Upsert Get Action 
        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                //This for Create 
                //We will return the empty object that we created(category)
                return View(coverType);
            }
            //This for Edit
            coverType = _unitOfWork.CoverType.Get(id.GetValueOrDefault()); //Reason we are using GetValueOrDefault because id could br null
            if (coverType == null)
            {
                return NotFound();
            }
            //Else
            return View(coverType);
        }

        [HttpPost]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    //Create
                    _unitOfWork.CoverType.Add(coverType);
                }
                else
                {
                    //Update
                    _unitOfWork.CoverType.Update(coverType);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var result = _unitOfWork.CoverType.Get(id);
            if (result == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.CoverType.Remove(result);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}