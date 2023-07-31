using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class CashierSetsController : Controller
    {
        private const int session_time = 1;
        private Model1Container db = new Model1Container();

        // GET: CashierSets
        public ActionResult Index()
        {
            return View(db.CashierSet.ToList());
        }

        // GET: CashierSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashierSet cashierSet = db.CashierSet.Find(id);
            if (cashierSet == null)
            {
                return HttpNotFound();
            }
            return View(cashierSet);
        }
        // GET: PatientSets/SignIn
        public ActionResult SignIn()
        {
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            return View();
        }
        // GET: CashierSets/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CashierSets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,pwd")] CashierSet cashierSet)
        {
            if (ModelState.IsValid)
            {
                db.CashierSet.Add(cashierSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cashierSet);
        }
        public ActionResult ProcessSets()
        {
            if (Session["StartTime"] is DateTime startTime && Session["cashierSet"] is CashierSet cashierSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                CashierSet existingCashier = db.CashierSet.FirstOrDefault(p => p.name == cashierSet.name && p.pwd == cashierSet.pwd);
                if (existingCashier != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    return View(db.ProcessSet.ToList());
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            TempData.Remove("Success");
            TempData.Keep();
            return RedirectToAction("SignIn", "PatientSets");
        }

        public ActionResult ProcessSetsEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProcessSet processSet = db.ProcessSet.Find(id);
            if (processSet == null)
            {
                return HttpNotFound();
            }
            return View(processSet);
        }

        // POST: ProcessSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProcessSetsEdit([Bind(Include = "Id,Patient,Doctor,Room,startTime,payment,bodyPart")] ProcessSet processSet)
        {
            if (Session["StartTime"] is DateTime startTime && Session["cashierSet"] is CashierSet cashierSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                CashierSet existingCashier = db.CashierSet.FirstOrDefault(p => p.name == cashierSet.name && p.pwd == cashierSet.pwd);
                if (existingCashier != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    ProcessSet exsistingProcessSet = db.ProcessSet.Find(processSet.Id);
                    if (exsistingProcessSet == null)
                    {
                        TempData["Error"] = "Not such process";
                        return RedirectToAction("ProcessSets");
                    }
                    exsistingProcessSet.payment = "paid";

                    var existingProcessSets = db.ProcessSet.Where(p => p.Id == processSet.Id).ToList();
                    if (existingProcessSets != null && existingProcessSets.Count > 0)
                    {
                        // 遍历existingPatientSets中的全部个体，并将它们都设为Detached状态
                        foreach (var item in existingProcessSets)
                        {
                            db.Entry(item).State = System.Data.Entity.EntityState.Detached;
                        }
                    }
                    db.Entry(exsistingProcessSet).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ProcessSets");
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            TempData.Remove("Success");
            TempData.Keep();
            return RedirectToAction("SignIn", "PatientSets");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Id,name,pwd")] CashierSet cashierSet)
        {
            CashierSet existingCashier = db.CashierSet.FirstOrDefault(p => p.name == cashierSet.name && p.pwd == cashierSet.pwd);
            if (existingCashier == null)
            {
                // If the patient already exists, add a model error and return to the view
                ModelState.AddModelError("pwd", "Cashier doesn't exist or password is incorrect.");
                return View(existingCashier);
            }
            TempData["Success"] = "Successfully signed in";
            TempData.Remove("Error");
            Session["cashierSet"] = cashierSet;
            Session["StartTime"] = DateTime.Now;
            HttpContext.Application["GlobalVar"] = "cashier";
            return RedirectToAction("ProcessSets", "CashierSets");
        }

        // GET: CashierSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashierSet cashierSet = db.CashierSet.Find(id);
            if (cashierSet == null)
            {
                return HttpNotFound();
            }
            return View(cashierSet);
        }

        // POST: CashierSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,pwd")] CashierSet cashierSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashierSet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cashierSet);
        }

        // GET: CashierSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashierSet cashierSet = db.CashierSet.Find(id);
            if (cashierSet == null)
            {
                return HttpNotFound();
            }
            return View(cashierSet);
        }

        // POST: CashierSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashierSet cashierSet = db.CashierSet.Find(id);
            db.CashierSet.Remove(cashierSet);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
