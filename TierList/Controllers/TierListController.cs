using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TierList.DAL;
using TierList.Models;
using Microsoft.AspNetCore.Http;
using TierList.Extensions;
using System.ComponentModel.DataAnnotations;


namespace TierList.Controllers
{
    public class TierListController : Controller
    {
        private ITierListDAL _dal;
        private string _sessionKey = "UserImages";

        public TierListController(ITierListDAL dal)
        {
            _dal = dal;
        }

        [HttpGet]
        public IActionResult Index()
        {
            IList<TierListModel> tierLists = _dal.GetTierLists();
            return View(tierLists);
        }

        [HttpGet]
        public IActionResult AddTierList()
        {

            IList<Image> images = GetSessionImages();
            return View(images);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddImage(Image image)
        {
            IList<Image> images = GetSessionImages();
            images.Add(image);


            SaveSessionImages(images);

            return RedirectToAction("AddTierList");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddTierList(string serializedTierList)
        {
            _dal.SaveTierList(_dal.DeserializeTierList(serializedTierList));
            return RedirectToAction("Index");
        }

        private void SaveSessionImages(IList<Image> images)
        {
            HttpContext.Session.Set<IList<Image>>(_sessionKey, images);
        }

        private IList<Image> GetSessionImages()
        {
            IList<Image> images = null;

            if (HttpContext.Session.Get<IList<Image>>(_sessionKey) != null)
            {
                images = HttpContext.Session.Get<IList<Image>>(_sessionKey);
            }
            else
            {
                images = new List<Image>();
            }

            return images;
        }

    }
}