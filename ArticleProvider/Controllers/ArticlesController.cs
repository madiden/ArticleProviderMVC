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
using System.Web.Configuration;
using System.Diagnostics;
using ArticleProvider.Services;

namespace ArticleProvider.Controllers
{
    public class ArticlesController : Controller
    {
        private ArticleContext db;
        private IConfigurationProvider _configProvider;
        public ArticlesController(IConfigurationProvider confProvider, ArticleContext context)
        {
            _configProvider = confProvider;
            db = context;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                    _userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
                return _userManager;
            }
        }


        private ApplicationUser GetUser()
        {
            return UserManager.FindById(User.Identity.GetUserId());
        }

        // GET: Articles
        public ActionResult Index()
        {
            ApplicationUser user = GetUser();
            var result = db.Articles.ToList();
            foreach (var article in result)
            {
                article.LikeCount = article.Likes.Count();
            }
            int max = result.Max(a => a.LikeCount);
            ViewBag.MinimumLikes = result.Min(a => a.LikeCount);
            foreach (var article in result)
            {
                if (max > 0)
                    article.Percentage = (int)(article.LikeCount * 100 / max);
            }
            ViewBag.IsEditor = false;
            if (user != null)
            {
                ViewBag.IsEditor = user.IsEditor;
                ViewBag.UserName = User.Identity.Name;
            }
            return View(result.ToList());
        }

        public ActionResult Comment()
        {
            // Return to Index page after a login on licomment operation.
            return RedirectToAction("Index");
        }

        public ActionResult Like()
        {
            // Return to Index page after a login on like operation.
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult Comment([Bind(Include = "ArticleId, Comment")]ArticleComments artComment)
        {
            if (artComment.ArticleId <= 0)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "You have passed an invalid article.");
            if(db.Articles.Any((art => art.Id == artComment.ArticleId)))
            {
                if (string.IsNullOrWhiteSpace(artComment.Comment))
                {
                    TempData["EmptyComment"] = true;
                }
                else
                {
                    artComment.Date = DateTime.Now;
                    artComment.UserId = User.Identity.Name;
                    db.Comments.Add(artComment);
                    db.SaveChanges();
                }
            }else
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound, "Article does not exist.");
            }
            return RedirectToAction("Details", new { id = artComment.ArticleId });
        }

        [OutputCache(NoStore =true, Duration =0)]
        [HttpPost]

        //[Route("articles/like/{aritcleId}")]
        public ActionResult Like(int? articleId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                Response.StatusCode = 403;
                return Json(new {Message = "You must log in to be able to like." });
            }
            bool success = false;
            string message = string.Empty;
            if (articleId == null)
                message = "You have passed an invalid article.";
            else
            {
                bool likedBefore = IsArticleLikedBefore(articleId.Value);
                if (!likedBefore)
                {
                    bool likeNumberExceeded = IsArticleLikeCountExceededForUser();
                    if (!likeNumberExceeded)
                    {
                        ArticleLike newLike = db.Likes.Create();
                        newLike.ArticleId = articleId.Value;
                        newLike.UserId = User.Identity.Name;
                        newLike.Date = DateTime.Now;
                        db.Likes.Add(newLike);
                        db.SaveChanges();
                        success = true;
                    }
                    else
                        message = "You have exceeded total number of likesfor today";
                }
                else
                {
                    message = "You have liked this article before";
                }
            }         
            
            return Json(new { Success = success, Message = message }, JsonRequestBehavior.AllowGet);
        }
        
        private bool IsArticleLikedBefore(int id)
        {
            var userId = User.Identity.Name;
            var query = db.Likes.Where((like) => like.ArticleId == id && like.UserId == userId);
            return query.Count() > 0;
        }

        private bool IsArticleLikeCountExceededForUser()
        {
            var userId = User.Identity.Name;
            DateTime now = DateTime.Now.Date;
            DateTime tomorrow = now.AddDays(1).AddMilliseconds(-1);
            var query = db.Likes.Where((like) =>
                       like.UserId == userId &&
                       like.Date > now &&
                       like.Date < tomorrow);
            return query.Count() >= _configProvider.GetMaximumLikesPerDay();
        }

        [OutputCache(NoStore =true, Duration =0)]
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
            // Check for previous likes. If coming from Like Redirection do not check again
            if (TempData["LikedBefore"] == null && IsArticleLikedBefore(id.Value))
                TempData["LikedBefore"] = true;
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
        public ActionResult Create([Bind(Include = "Title,Content")] Article article)
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
            if (User.Identity.Name != article.AuthorId)
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized, "You are not the author of the article.");
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
                var original = db.Articles.Find(article.Id);
                if(original != null)
                {
                    if (original.AuthorId == User.Identity.Name)
                    {
                        original.Title = article.Title;
                        original.Content = article.Content;
                        original.LastUpdateDate = DateTime.Now;
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
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
