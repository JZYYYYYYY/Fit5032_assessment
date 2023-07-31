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
    public class DoctorSetsController : Controller
    {
        private const int session_time = 1;
        private Model1Container db = new Model1Container();

        // GET: DoctorSets
        public ActionResult Index()
        {
            return View(db.DoctorSet.ToList());
        }

        // GET: DoctorSets/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorSet doctorSet = db.DoctorSet.Find(id);
            if (doctorSet == null)
            {
                return HttpNotFound();
            }
            return View(doctorSet);
        }

        // GET: DoctorSets/Create
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult SignIn()
        {
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn([Bind(Include = "Id,name,pwd")] DoctorSet doctorSet)
        {
            DoctorSet existingDoctor = db.DoctorSet.FirstOrDefault(p => p.name == doctorSet.name && p.pwd == doctorSet.pwd);
            if (existingDoctor == null)
            {
                // If the patient already exists, add a model error and return to the view
                ModelState.AddModelError("pwd", "Doctor doesn't exist or password is incorrect.");
                return View(existingDoctor);
            }
            TempData["Success"] = "Successfully signed in";
            TempData.Remove("Error");
            Session["doctorSet"] = doctorSet;
            Session["StartTime"] = DateTime.Now;
            HttpContext.Application["GlobalVar"] = "doctor";
            return RedirectToAction("ProcessSets", "DoctorSets");
        }

        public ActionResult ProcessSets()
        {
            if (Session["StartTime"] is DateTime startTime && Session["doctorSet"] is DoctorSet doctorSet)
            {
                // 获取当前系统时间
                DateTime currentTime = DateTime.Now;

                // 计算时间间隔
                TimeSpan timeDifference = currentTime - startTime;
                DoctorSet existingDoctor = db.DoctorSet.FirstOrDefault(p => p.name == doctorSet.name && p.pwd == doctorSet.pwd);
                if (existingDoctor != null || timeDifference.TotalMinutes <= session_time)
                {
                    Session["StartTime"] = currentTime;
                    return View(db.ProcessSet.Where(p => p.Doctor == doctorSet.name));
                }
            }
            Session.Clear();
            HttpContext.Application["GlobalVar"] = "";
            TempData["Error"] = "Not signed in or session timeout";
            TempData.Remove("Success");
            TempData.Keep();
            return RedirectToAction("SignIn", "PatientSets");
        }
        // POST: DoctorSets/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,name,pwd,endTime")] DoctorSet doctorSet)
        {
            if (ModelState.IsValid)
            {
                db.DoctorSet.Add(doctorSet);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctorSet);
        }

        // GET: DoctorSets/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorSet doctorSet = db.DoctorSet.Find(id);
            if (doctorSet == null)
            {
                return HttpNotFound();
            }
            return View(doctorSet);
        }

        // POST: DoctorSets/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性；有关
        // 更多详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,name,pwd,endTime")] DoctorSet doctorSet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctorSet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctorSet);
        }

        // GET: DoctorSets/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DoctorSet doctorSet = db.DoctorSet.Find(id);
            if (doctorSet == null)
            {
                return HttpNotFound();
            }
            return View(doctorSet);
        }

        // POST: DoctorSets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DoctorSet doctorSet = db.DoctorSet.Find(id);
            db.DoctorSet.Remove(doctorSet);
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
