using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.Configuration;
using System.Data;
using ASMIK_MAGI.Repository;
using ASMIK_MAGI.Models;
using System.Web.Security;

namespace ASMIK_MAGI.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        [HttpGet]
        public ActionResult Index()
        {
            LoginModels model = new LoginModels();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index(LoginModels model, string Submit)
        {
            try
            {
                DataTable dtlogin = Common.ExecuteQuery("SP_LOGIN'" + model.username + "', '"+ model.password +"', ''");
                if (dtlogin.Rows.Count > 0)
                {
                    string role = dtlogin.Rows[0]["Role"].ToString();
                    if (role == "Super Admin")
                    {
                        Session["Rolez"] = "Super Admin";
                    }
                    else if(role == "Admin")
                    {
                        Session["Rolez"] = "Admin";
                    }
                    else
                    {
                        Session["Rolez"] = "Marketing Officer";
                    }
                    Session["UserID"] = dtlogin.Rows[0]["Username"].ToString();
                    string cek = Session["Rolez"].ToString();
                    return RedirectToAction("Search_NB_Asmik", "Home"); 
                }
                else
                {
                    Response.Write("<script>alert('Invalid Username or Password');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Invalid Username or Password');</script>");
            }

            return View(model);
        }
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Abandon();
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Login");
        }
    }
}