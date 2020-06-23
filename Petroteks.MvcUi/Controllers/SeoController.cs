﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Petroteks.Bll.Abstract;
using Petroteks.Bll.Helpers;
using Petroteks.Entities.ComplexTypes;
using Petroteks.Entities.Concreate;
using Petroteks.MvcUi.Services;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using static Petroteks.Bll.Helpers.FriendlyUrlHelper;

namespace Petroteks.MvcUi.Controllers
{
    public class SeoController : GlobalController
    {
        private readonly ICategoryService categoryService;
        private readonly IProductService productService;
        private readonly IDynamicPageService dynamicPageService;
        private readonly IMainPageService mainPageService;
        private readonly IAboutUsObjectService aboutUsObjectService;
        private readonly IUI_ContactService uI_ContactService;
        private readonly IBlogService blogService;
        private readonly IPrivacyPolicyObjectService privacyPolicyObjectService;


        public SeoController(ICategoryService categoryService,
            IWebsiteService websiteService,
            IPrivacyPolicyObjectService privacyPolicyObjectService,
            IProductService productService,
            ILanguageCookieService languageCookieService,
            ILanguageService languageService,
            IDynamicPageService dynamicPageService,
            IMainPageService mainPageService,
            IAboutUsObjectService aboutUsObjectService,
            IUI_ContactService uI_ContactService,
            IHttpContextAccessor httpContextAccessor,
            IBlogService blogService)
            : base(websiteService, languageService, languageCookieService, httpContextAccessor)
        {
            this.categoryService = categoryService;
            this.productService = productService;
            this.dynamicPageService = dynamicPageService;
            this.mainPageService = mainPageService;
            this.aboutUsObjectService = aboutUsObjectService;
            this.uI_ContactService = uI_ContactService;
            this.blogService = blogService;
            this.privacyPolicyObjectService = privacyPolicyObjectService;
        }


        [Route("sitemap.xml")]
        public IActionResult SitemapXml()
        {
            Response.Clear();
            Response.ContentType = "text/xml";
            XmlTextWriter xtr = new XmlTextWriter(Response.Body, Encoding.UTF8);
            xtr.WriteStartDocument();
            xtr.WriteStartElement("urlset");
            xtr.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
            xtr.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            xtr.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            string siteUrl = WebsiteContext.CurrentWebsite.BaseUrl.Replace("www.", "", System.StringComparison.CurrentCultureIgnoreCase);

            if (siteUrl.Equals("https://localhost:44316"))
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("bentonitepellets", "Home")}");
                xtr.WriteEndElement();
            }

            MainPage mainPage = mainPageService.Get(x => x.WebSiteid == WebsiteContext.CurrentWebsite.id && x.IsActive);
            if (mainPage != null)
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}");
                xtr.WriteElementString("lastmod", $"{(mainPage.UpdateDate ?? mainPage.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                xtr.WriteEndElement();
            }


            AboutUsObject aboutUs = aboutUsObjectService.Get(x => x.WebSiteid == WebsiteContext.CurrentWebsite.id && x.IsActive);
            if (aboutUs != null)
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("AboutUs", "Home")}");
                xtr.WriteElementString("lastmod", $"{(aboutUs.UpdateDate ?? aboutUs.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                xtr.WriteEndElement();
            }
            PrivacyPolicyObject privacy = privacyPolicyObjectService.Get(x => x.WebSiteid == WebsiteContext.CurrentWebsite.id && x.IsActive);
            if (privacy!= null)
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("PrivacyPolicy", "Home")}");
                xtr.WriteElementString("lastmod", $"{(privacy.UpdateDate ?? privacy.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                xtr.WriteEndElement();
            }
           
            UI_Contact contact = uI_ContactService.Get(x => x.WebSiteid == WebsiteContext.CurrentWebsite.id && x.IsActive);
            if (contact != null)
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("Contact", "Home")}");
                xtr.WriteElementString("lastmod", $"{(contact.UpdateDate ?? contact.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                xtr.WriteEndElement();
            }

            xtr.WriteStartElement("url");
            xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("PetroBlog", "Home")}");
            xtr.WriteEndElement();

            if (siteUrl.Equals("https://petroteks.com"))
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("SondajKopugu", "Home")}");
                xtr.WriteEndElement();
            }
            if (siteUrl.Equals("http://rbsbentonit.com"))
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("bentonitepellets", "Home")}");
                xtr.WriteEndElement();
            }
            if (siteUrl.Equals("http://rbsbentonit.com"))
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("hddbentonite", "Home")}");
                xtr.WriteEndElement();
            }
            if (siteUrl.Equals("http://rbsbentonit.com"))
            {
                xtr.WriteStartElement("url");
                xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("catlitter", "Home")}");
                xtr.WriteEndElement();
            }


            ICollection<Category> Categories = new List<Category>();

            try
            {
                Categories = categoryService.GetMany(x => x.WebSiteid == Bll.Helpers.WebsiteContext.CurrentWebsite.id && x.IsActive == true);

                foreach (Category item in Categories)
                {
                    xtr.WriteStartElement("url");
                    xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("CategoryDetail", "Detail", new { categoryName = GetFriendlyTitle(item.Name), page = 1, category = item.id })}");
                    xtr.WriteElementString("lastmod", $"{(item.UpdateDate ?? item.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                    xtr.WriteEndElement();
                }
            }
            catch { }

            try
            {
                ICollection<Product> Products = productService.GetMany(x => x.IsActive == true);

                var WebsiteProducts =
                                    from category in Categories
                                    join prod in Products on category.id equals prod.Categoryid
                                    select new { ProductName = prod.SupTitle, id = prod.id, UpdateDate = prod.UpdateDate, CreateDate = prod.CreateDate };


                foreach (var item in WebsiteProducts)
                {
                    xtr.WriteStartElement("url");
                    xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("ProductDetail", "Detail", new { produtname = GetFriendlyTitle(item.ProductName), id = item.id })}");
                    xtr.WriteElementString("lastmod", $"{(item.UpdateDate ?? item.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                    xtr.WriteEndElement();
                }
            }
            catch { }

            try
            {
                ICollection<DynamicPage> dynamicPages = dynamicPageService.GetMany(x => x.WebSiteid == Bll.Helpers.WebsiteContext.CurrentWebsite.id && x.IsActive == true);

                foreach (DynamicPage item in dynamicPages)
                {
                    xtr.WriteStartElement("url");
                    xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("DynamicPageView", "Home", new { pageName = GetFriendlyTitle(item.Name), id = item.id })}");
                    xtr.WriteElementString("lastmod", $"{(item.UpdateDate ?? item.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                    xtr.WriteEndElement();
                }
            }
            catch { }


            try
            {
                ICollection<Blog> blogs = blogService.GetMany(x => x.WebSiteid == Bll.Helpers.WebsiteContext.CurrentWebsite.id && x.IsActive == true);

                foreach (Blog item in blogs)
                {
                    xtr.WriteStartElement("url");
                    xtr.WriteElementString("loc", $"{siteUrl}{Url.Action("BlogDetail", "Home", new { title = GetFriendlyTitle(item.Title), id = item.id })}");
                    xtr.WriteElementString("lastmod", $"{(item.UpdateDate ?? item.CreateDate).ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss")}+03:00");
                    xtr.WriteEndElement();
                }
            }
            catch { }
            xtr.WriteEndElement();
            xtr.WriteEndDocument();
            xtr.Flush();
            xtr.Close();

            return View();
        }
    }
}