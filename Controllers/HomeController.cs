using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Agricultural_Web_Application.Models;

namespace Agricultural_Web_Application.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult About()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            else if (Session["role"].ToString() == "Farmer")
            {
                return RedirectToAction("Login");
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            var ulist = db.Users.Where(n => n.role == "Farmer").ToList().Take(4);
            return View(ulist);
        }

        public ActionResult Farmers()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            else if (Session["role"].ToString() == "Farmer")
            {
                return RedirectToAction("Login");
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            var ulist = db.Users.Where(n => n.role == "Farmer").ToList();
            return View(ulist);
        }
        public ActionResult ProductDetails(int id)
        {
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            var product = db.Products.Where(n => n.id == id).FirstOrDefault();
            return View(product);
        }
        public ActionResult Products()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            int id = 0;
            if (Session["role"].ToString() == "Farmer")
            {
                id = int.Parse(Session["uid"].ToString());
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            var plist = from p in db.Products
                        join u in db.Users on p.uid equals u.id
                        select new ProductViewModel
                        {
                            PId = p.id,
                            PName = p.pname,
                            Pimage = p.pimage,
                            Price = p.price.ToString(),
                            Category = p.category,
                            Description = p.description,
                            Production_date = p.production_date.ToString(),
                            UId = u.id,
                            UserName = u.fullname,
                        };
            if (id != 0)
            {
                plist = (from p in db.Products
                         join u in db.Users on p.uid equals u.id
                         select new ProductViewModel
                         {
                             PId = p.id,
                             PName = p.pname,
                             Pimage = p.pimage,
                             Price = p.price.ToString(),
                             Category = p.category,
                             Description = p.description,
                             Production_date = p.production_date.ToString(),
                             UId = u.id,
                             UserName = u.fullname,
                         }).Where(n => n.UId == id);
            }

            return View(plist);
        }
        [HttpPost]
        public ActionResult Products(string price, string category)
        {
            int id = 0;
            if (Session["role"].ToString() == "Farmer")
            {
                id = int.Parse(Session["uid"].ToString());
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();

            var plist = (from p in db.Products
                         join u in db.Users on p.uid equals u.id
                         select new ProductViewModel
                         {
                             PId = p.id,
                             PName = p.pname,
                             Pimage = p.pimage,
                             Price = p.price.ToString(),
                             Category = p.category,
                             Description = p.description,
                             Production_date = p.production_date.ToString(),
                             UId = u.id,
                             UserName = u.fullname,
                         }).AsEnumerable(); 

            if (id != 0)
            {
                plist = plist.Where(n => n.UId == id);
            }

            if (!string.IsNullOrEmpty(price) && price != "All")
            {
                if (price == "Under 50")
                {
                    plist = plist.Where(p => int.Parse(p.Price) <= 50);
                }
                else if (price == "50-100")
                {
                    plist = plist.Where(p => int.Parse(p.Price) >= 50 && int.Parse(p.Price) <= 100);
                }
                else if (price == "Over 100")
                {
                    plist = plist.Where(p => int.Parse(p.Price) >= 100);
                }
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                plist = plist.Where(p => p.Category == category);
            }

            return View(plist.ToList()); 
        }

        public ActionResult Profile()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            int uid = int.Parse(Session["uid"].ToString());
            var user = db.Users.Where(n => n.id == uid).FirstOrDefault();
            return View(user);
        }
        public ActionResult UserDetails(int id)
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
            var user = db.Users.Where(n => n.id == id).FirstOrDefault();
            return View(user);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(User u)

        {

            Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();

            var user = db.Users.Where(n => n.email == u.email && n.password == u.password && n.role == u.role).FirstOrDefault();

            if (user != null)

            {

                if (user.role == "Employee")

                {

                    Session["uid"] = user.id.ToString();

                    Session["username"] = user.username.ToString();

                    Session["role"] = user.role.ToString();

                    return RedirectToAction("Index", "Home");

                }

                else

                {

                    Session["uid"] = user.id.ToString();

                    Session["username"] = user.username.ToString();

                    Session["role"] = user.role.ToString();

                    return RedirectToAction("Index", "Home");

                }

            }

            else

            {

                ViewBag.Message = "Invalid Email or Password!";

            }

            return View();

        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User u, HttpPostedFileBase imgFile)
        {
            if (imgFile != null && imgFile.ContentLength > 0)
            {
                string uploadPath = Server.MapPath("~/UserImages/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Path.GetFileName(imgFile.FileName);
                string ext = Path.GetExtension(fileName);
                string[] extarr = { ".png", ".PNG", ".jpg", ".bmp" };
                if (extarr.Contains(ext))
                {
                    string filePath = Path.Combine(uploadPath, fileName);
                    string path = "/UserImages/" + fileName.ToString();
                    imgFile.SaveAs(filePath);
                    Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                    User user = new User();
                    user.imgPath = path;
                    user.username = u.username.ToString();
                    user.fullname = u.fullname.ToString();
                    user.email = u.email.ToString();
                    user.contact = u.contact.ToString();
                    user.password = u.password.ToString();
                    user.role = "Employee";
                    db.Users.Add(user);
                    db.SaveChanges();
                    Response.Write("<script type = 'text/javascript'>alert('Successfully Uploaded Your User');</script>");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                User user = new User();
                user.imgPath = "/UserImages/Default.png";
                user.username = u.username.ToString();
                user.fullname = u.fullname.ToString();
                user.email = u.email.ToString();
                user.contact = u.contact.ToString();
                user.password = u.password.ToString();
                user.role = "Employee";
                db.Users.Add(user);
                db.SaveChanges();
                Response.Write("<script type = 'text/javascript'>alert('Successfully Uploaded Your User');</script>");
                return RedirectToAction("Index");
            }
            Response.Write("<script type = 'text/javascript'>alert('Failed Uploaded Your User');</script>");
            return View();
        }
        public ActionResult AddFarmer()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            else if (Session["role"].ToString() == "Farmer")
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddFarmer(User u, HttpPostedFileBase imgFile)
        {
            if (imgFile != null && imgFile.ContentLength > 0)
            {
                string uploadPath = Server.MapPath("~/UserImages/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Path.GetFileName(imgFile.FileName);
                string ext = Path.GetExtension(fileName);
                string[] extarr = { ".png", ".PNG", ".jpg", ".bmp" };
                if (extarr.Contains(ext))
                {
                    string filePath = Path.Combine(uploadPath, fileName);
                    string path = "/UserImages/" + fileName.ToString();
                    imgFile.SaveAs(filePath);
                    Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                    User user = new User();
                    user.imgPath = path;
                    user.username = u.username.ToString();
                    user.fullname = u.fullname.ToString();
                    user.email = u.email.ToString();
                    user.contact = u.contact.ToString();
                    user.password = u.password.ToString();
                    user.role = "Farmer";
                    db.Users.Add(user);
                    db.SaveChanges();
                    Response.Write("<script type = 'text/javascript'>alert('Successfully add new farmer');</script>");
                    return RedirectToAction("Index");
                }
            }
            else
            {
                Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                User user = new User();
                user.imgPath = "/UserImages/Default.png";
                user.username = u.username.ToString();
                user.fullname = u.fullname.ToString();
                user.email = u.email.ToString();
                user.contact = u.contact.ToString();
                user.password = u.password.ToString();
                user.role = "Farmer";
                db.Users.Add(user);
                db.SaveChanges();
                Response.Write("<script type = 'text/javascript'>alert('Successfully Add New Farmer');</script>");
                return RedirectToAction("Index");
            }
            Response.Write("<script type = 'text/javascript'>alert('Failed to add new farmer');</script>");
            return View();
        }
        public ActionResult AddProduct()
        {
            if (Session["uid"] == null)
            {
                return RedirectToAction("Login");
            }
            else if (Session["role"].ToString() == "Employee")
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddProduct(Product p, HttpPostedFileBase imgFile)
        {
            if (imgFile != null && imgFile.ContentLength > 0)
            {
                string uploadPath = Server.MapPath("~/ProductImages/");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fileName = Path.GetFileName(imgFile.FileName);
                string ext = Path.GetExtension(fileName);
                string[] extarr = { ".png", ".PNG", ".jpg", ".bmp" };
                if (extarr.Contains(ext))
                {
                    string filePath = Path.Combine(uploadPath, fileName);
                    string path = "/ProductImages/" + fileName.ToString();
                    imgFile.SaveAs(filePath);
                    Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                    Product product = new Product();
                    product.pimage = path;
                    product.pname = p.pname.ToString();
                    product.description = p.description.ToString();
                    product.category = p.category.ToString();
                    product.price = int.Parse(p.price.ToString());
                    product.production_date = p.production_date;
                    product.uid = int.Parse(Session["uid"].ToString());
                    db.Products.Add(product);
                    db.SaveChanges();
                    Response.Write("<script type = 'text/javascript'>alert('Successfully Uploaded Your User');</script>");
                    return RedirectToAction("Products");
                }
            }
            else
            {
                Agricultural_Web_ApplicationEntities db = new Agricultural_Web_ApplicationEntities();
                Product product = new Product();
                product.pimage = "/ProductImages/default_product.jpg";
                product.pname = p.pname.ToString();
                product.description = p.description.ToString();
                product.category = p.category.ToString();
                product.price = int.Parse(p.price.ToString());
                product.production_date = p.production_date;
                product.uid = int.Parse(Session["uid"].ToString());
                db.Products.Add(product);
                db.SaveChanges();
                Response.Write("<script type = 'text/javascript'>alert('Successfully Uploaded Your Product');</script>");
                return RedirectToAction("Products");
            }
            Response.Write("<script type = 'text/javascript'>alert('Failed Uploaded Your Product');</script>");
            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}