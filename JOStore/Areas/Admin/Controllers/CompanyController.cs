using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Store.DataAccess.Repository;
using Store.DataAccess.Repository.IRepository;
using Store.Models;
using Store.Models.ViewModels;
using Store.Utility;

namespace JOStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _iUnitOfWork;
        public CompanyController(IUnitOfWork iUnitOfWork)
        {
            _iUnitOfWork = iUnitOfWork;
        }
        public IActionResult Index()
        {
            List<Company> CompanyList = _iUnitOfWork
                .Company.GetAll().ToList();
            return View(CompanyList);
        }

        public IActionResult Upsert(int? Id)
        {
            if (Id == null || Id == 0)
            {
                //Create
                return View(new Company());

            }
            else
            {
                //update
                Company companyObj = _iUnitOfWork.Company.Get(x => x.Id == Id);
                return View(companyObj);

            }
        }
        [HttpPost]
        public IActionResult Upsert(Company company)
        {

            if (ModelState.IsValid)
            {
                
                if (company.Id == 0)
                {
                    _iUnitOfWork.Company.Add(company);

                }
                else
                {
                    _iUnitOfWork.Company.Update(company);

                }
                _iUnitOfWork.Save();
                TempData["Success"] = "Company Created Successfully";
                return RedirectToAction("Index", "Company");
            }
            else
            {
                
                return View(company);
            }


        }


        private IEnumerable<SelectListItem> GetCategoryList()
        {
            return _iUnitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var Companies = _iUnitOfWork.Company.GetAll()
                .Select(company => new
                {
                    id = company.Id,
                    name = company.Name,
                    streetAddress = company.StreetAdress, 
                    city = company.City,
                    state = company.State,
                    postalCode = company.PostalCode,
                    phoneNumber = company.PhoneNumber

                });

            return Json(new { data = Companies });
        }
        [HttpDelete]    
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _iUnitOfWork.Company.Get(x => x.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

           
            _iUnitOfWork.Company.Delete(CompanyToBeDeleted);
            _iUnitOfWork.Save();
            return Json(new { success = true, message = "Delet Success" });


            #endregion
        }
    }

    }
