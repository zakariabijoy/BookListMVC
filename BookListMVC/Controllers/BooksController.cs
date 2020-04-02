using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookListMVC.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Book Book { get; set; }

        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Book = new Book();
            if (id == null)
            {
                //create
                return View(Book);
            }
            //update
            Book = _db.Books.Find(id);
            if (Book == null)
            {
                return NotFound();
            }
            return View(Book);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        {
            if (ModelState.IsValid)
            {
                if (Book.Id == 0)
                {
                    //create
                    _db.Books.Add(Book);

                }
                else
                {
                    //update
                    _db.Books.Update(Book);
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(Book);
        }

        #region Api Calls

        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _db.Books.ToList() });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var book = _db.Books.Find(id);
            if (book == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _db.Books.Remove(book);
            _db.SaveChanges();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion

    }
}