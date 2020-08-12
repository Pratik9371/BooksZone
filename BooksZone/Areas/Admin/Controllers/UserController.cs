using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BooksZone.DataAccess.Data;
using BooksZone.DataAccess.Repository;
using BooksZone.DataAccess.Repository.IRepository;
using BooksZone.Models;
using BooksZone.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BooksZone.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Emp)]
    public class UserController : Controller
    {
        //private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public UserController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            //Refer Documents Named UserManagement
            var userList = _db.ApplicationUsers.Include(u => u.Company).ToList();   //We are getting all the users from the application user 
            var userRole = _db.UserRoles.ToList();    //getting the roles of the user (it's the mapping between the userList and roles)
            var roles = _db.Roles.ToList();       //list of all the roles

            foreach(var user in userList)
            {
                //Role Id
                var roleId = userRole.FirstOrDefault(i => i.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(i => i.Id == roleId).Name;
                if (user.Company == null)
                {
                    user.Company = new Company
                    {
                        Name = ""
                    };
                };
            }
         return Json(new { data = userList });
        }
        #endregion
    }
}