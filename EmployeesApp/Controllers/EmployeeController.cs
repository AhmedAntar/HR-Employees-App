using Microsoft.AspNetCore.Mvc;
using EmployeesApp.Models;

namespace EmployeesApp.Controllers
{
    public enum SortDirection
    {
       Ascending,
       Descending,
    }
    public class EmployeeController : Controller
    {
        //instance from DbContext
        HRDatabaseContext dbContext = new HRDatabaseContext();

        public IActionResult Index(string SortField, string CurrentSortField, SortDirection SortDirection, string SearchByName)
        {
            var employees = GetEmployees();
            if (!string.IsNullOrEmpty(SearchByName))
                employees = employees.Where(e => e.EmployeeName.ToLower().Contains(SearchByName.ToLower())).ToList();
            return View(this.SortEmployees(employees, SortField, CurrentSortField, SortDirection));
        }

        private List<Employee> GetEmployees()
        {
            var employees = (from employee in dbContext.Employees
                             join department in dbContext.Departments on employee.DepartmentId equals department.DepartmentId
                             select new Employee
                             {
                                 EmployeeId = employee.EmployeeId,
                                 EmployeeName = employee.EmployeeName,
                                 EmployeeNumber = employee.EmployeeNumber,
                                 DOB = employee.DOB,
                                 HiringDate = employee.HiringDate,
                                 GrossSalary = employee.GrossSalary,
                                 NetSalary = employee.NetSalary,
                                 DepartmentId = employee.DepartmentId,
                                 DepartmentName = department.DepartmentName,
                             }).ToList();
            return employees;
            
        }

        //to view the create page 
        public IActionResult Create()
        {
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create (Employee model)
        {
            //remov validation this attributs
            ModelState.Remove("EmployeeId");
            ModelState.Remove("Department");
            ModelState.Remove("DepartmentName");
            if(ModelState.IsValid)
            {
                dbContext.Employees.Add(model);
                dbContext.SaveChanges();
                TempData["Success"] = "Created succesfully!.";
                return RedirectToAction("Index");
            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            return View();
        }
        public IActionResult Edit(int ID)
        {
            Employee data = dbContext.Employees.Where(e=>e.EmployeeId == ID).FirstOrDefault();
            ViewBag.Departments = this.dbContext.Departments.ToList();
            
            return View("Create", data);
        }
        [HttpPost]
        public IActionResult Edit(Employee model) 
        {
            //remov validation this attributs
            ModelState.Remove("EmployeeId");
            ModelState.Remove("Department");
            ModelState.Remove("DepartmentName");
            if (ModelState.IsValid)
            {
                dbContext.Employees.Update(model);
                dbContext.SaveChanges();
                TempData["Success"] = "Edit succesfully!.";
                return RedirectToAction("Index");
            }
            ViewBag.Departments = this.dbContext.Departments.ToList();
            
            return View("Create", model);
        }
        

        public IActionResult Delete (int ID) 
        {
            Employee data = dbContext.Employees.Where(e => e.EmployeeId == ID).FirstOrDefault();
            if(data != null) 
            {
                dbContext.Employees.Remove(data);
                dbContext.SaveChanges();
                TempData["Success"] = "Deleted succesfully!.";
            }
            return RedirectToAction("Index");
        }
        private List<Employee> SortEmployees(List<Employee> employees, string sortField, string currentSortField, SortDirection sortDirection)
        {
            if (string.IsNullOrEmpty(sortField)) 
            {
                ViewBag.SortField = "EmployeeNumber";
                ViewBag.SortDirection = SortDirection.Ascending;

            }
            else
            {
                if (currentSortField == sortField)
                    ViewBag.SortDirection = sortDirection == SortDirection.Ascending ? SortDirection.Descending : SortDirection.Ascending;
                else
                    ViewBag.SortDirection = SortDirection.Ascending;
                ViewBag.SortField = sortField;
            }

            var propertyInfo = typeof(Employee).GetProperty(ViewBag.SortField);
            if (ViewBag.sortDirection == SortDirection.Ascending)
                employees = employees.OrderBy(e => propertyInfo.GetValue(e, null)).ToList();
            else
                employees = employees.OrderByDescending(e => propertyInfo.GetValue(e, null)).ToList();
            return employees;
        }
       
    }
}
