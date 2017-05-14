using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DAL.Model;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace ArticleProvider.Controllers
{
    public class ArticlesController : Controller
    {
        private ArticleContext db = new ArticleContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if(_userManager == null)
                    _userManager =  HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }            
        }

        private ApplicationUser GetUser()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }

        // GET: Articles
        [Authorize]
        public ActionResult Index()
        {
            ApplicationUser user = GetUser();
            var result = db.Articles;
            ViewBag.IsEditor = user.IsEditor;
            ViewBag.UserName = User.Identity.Name;
            return View(result.ToList());
        }

        [Authorize]
        [HttpPost]
        //[Route("articles/like/{aritcleId}")]
        public ActionResult Like(int? articleId)
        {
            if (articleId == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You have passed an invalid article.");           
            if(!IsArticleLikedBefore(articleId.Value))
            {
                ArticleLike newLike = db.Likes.Create();
                newLike.ArticleId = articleId.Value;
                newLike.UserId = User.Identity.GetUserId();
                newLike.Date = DateTime.Now;
                db.Likes.Add(newLike);
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { id = articleId });
        }

        private bool IsArticleLikedBefore(int id)
        {
            var userId = User.Identity.GetUserId();
            var query = db.Likes.Where((like) => like.ArticleId == id && like.UserId == userId);
            if (query.Count() > 0)
                return true;
            return false;
        }

        // GET: Articles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserName = User.Identity.Name;
            if (IsArticleLikedBefore(id.Value))
                ViewBag.LikedBefore = true;
            return View(article);
        }

        // GET: Articles/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Content,AuthorId,CreationDate,LastUpdateDate")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.CreationDate = DateTime.Now;
                article.AuthorId = User.Identity.Name;
                db.Articles.Add(article);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(article);
        }

        // GET: Articles/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Content")] Article article)
        {
            if (ModelState.IsValid)
            {
                // As we are only getting Id, Title and content fields, We need to update only them.
                // because CreationDate and AuthorId is only set when creation is made.
                var entry = db.Entry(article);
                var dbArticle= db.Articles.Attach(article);
                dbArticle.Title = article.Title;
                dbArticle.Content = article.Content;
                dbArticle.LastUpdateDate = DateTime.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(article);
        }

        // GET: Articles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Articles.Find(id);
            if (article == null)
            {
                return HttpNotFound();
            }
            return View(article);
        }

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Article article = db.Articles.Find(id);
            db.Articles.Remove(article);
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
