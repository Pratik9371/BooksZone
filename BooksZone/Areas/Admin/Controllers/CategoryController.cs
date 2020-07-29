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
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
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
          var allObj = _unitOfWork.Category.GetAll();
          return Json(new { data = allObj });
        }

        //Category Upsert Get Action 
        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                //This for Create 
                //We will return the empty object that we created(category)
                return View(category);
            }
            //This for Edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault()); //Reason we are using GetValueOrDefault because id could br null
            if (category == null)
            {
                return NotFound();
            }
            //Else
            return View(category);
        }

        [HttpPost]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid)
            {
                if (category.Id == 0)
                {
                    //Create
                    _unitOfWork.Category.Add(category);
                }
                else
                {
                    //Update
                    _unitOfWork.Category.Update(category);
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var result = _unitOfWork.Category.Get(id);
            if (result == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Category.Remove(result);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}