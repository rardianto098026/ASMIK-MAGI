using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using ASMIK_MAGI.Models;
using System.Configuration;
using ASMIK_MAGI.Repository;
using System.Net;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Web.UI.WebControls;

namespace ASMIK_MAGI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageLogin(string Username, string Password, string submit, string Role)
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            string role = Session["Rolez"].ToString();
            if (Session["Message"] != null)
            {
                TempData["message"] = Session["Message"].ToString();
                Session["Message"] = null;
            } 
            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();

            if (submit == "Add")
            {
                if (Username == "" || Password == "" || role == "")
                {
                    TempData["message"] = "<script>alert('Masukkan data login dengan lengkap');</script>";
                    model.Content = List_Login(Session["Rolez"].ToString());
                    return View(model);
                }
                DataTable dt01 = Common.ExecuteQuery("dbo.[SP_LOGIN] '"+Username+"', '', ''");
                if (dt01.Rows.Count > 0)
                {
                    TempData["message"] = "<script>alert('Username sudah pernah digunakan');</script>";
                    model.Content = List_Login(Session["Rolez"].ToString());
                    return View(model);
                }
               
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_INSERT_MST_LOGIN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERNAME", string.IsNullOrEmpty(Username) == true ? "" : Username);
                        cmd.Parameters.AddWithValue("@PASSWORD", string.IsNullOrEmpty(Password) == true ? "" : Password);
                        cmd.Parameters.AddWithValue("@CREATEDBY", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@ROLE", string.IsNullOrEmpty(Role) == true ? "" : Role);
                        
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                TempData["message"] = "<script>alert('Data Saved');</script>";
            }

            model.Content = List_Login(Session["Rolez"].ToString());
            return View(model);
        }

        public ActionResult Edit_Login(PagedList<ASMIKModels> model,string idxHeader, string Username, string Password, string Role, string submit)
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            if(idxHeader != null)
            {
                Session["idx"] = idxHeader;
                DataTable dt01 = Common.ExecuteQuery("dbo.[SP_LOGIN] '', '', '" + idxHeader + "'");
                if (dt01.Rows.Count > 0)
                {
                    model.Username = dt01.Rows[0]["Username"].ToString();
                    model.Password = dt01.Rows[0]["Password"].ToString();
                    Session["Username"] = model.Username;
                    Session["Password"] = model.Password;
                    Session["Role"] = dt01.Rows[0]["Role"].ToString();
                    string role = Session["Role"].ToString();
                    //model.Role = dt01.Rows[0]["Role"].ToString();
                    if (dt01.Rows[0]["Role"].ToString() == "Super Admin")
                    {
                        TempData["Super Admin"] = "selected = \"selected\"";
                    }
                    else
                    {
                        TempData["Admin"] = "selected = \"selected\"";
                    }
                    //TempData[Role] = "selected = \"selected\"";
                }
            }
            
            if (submit == "Update")
            {
                if (model.Username == Session["Username"].ToString() || model.Password == Session["Password"].ToString() || Role == Session["Role"].ToString())
                {
                    TempData["message"] = "<script>alert('Anda belum melakukan perubahan');</script>";
                }
                else
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("SP_UPDATE_MST_LOGIN", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@USERNAME", string.IsNullOrEmpty(Username) == true ? "" : Username);
                            cmd.Parameters.AddWithValue("@PASSWORD", string.IsNullOrEmpty(Password) == true ? "" : Password);
                            cmd.Parameters.AddWithValue("@UPDATEBY", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@ROLE", string.IsNullOrEmpty(Role) == true ? "" : Role);
                            cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(Session["idx"].ToString()) == true ? "" : Session["idx"]);
                            cmd.Parameters.AddWithValue("@COMMAND", string.IsNullOrEmpty("UPDATE") == true ? "" : "UPDATE");

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                    TempData["message"] = "<script>alert('Data Updated');</script>";
                    return RedirectToAction("ManageLogin", "Home");
                }
            }
            else if(submit == "Delete")
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_UPDATE_MST_LOGIN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@USERNAME", string.IsNullOrEmpty(Username) == true ? "" : Username);
                        cmd.Parameters.AddWithValue("@PASSWORD", string.IsNullOrEmpty(Password) == true ? "" : Password);
                        cmd.Parameters.AddWithValue("@UPDATEBY", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                        cmd.Parameters.AddWithValue("@ROLE", string.IsNullOrEmpty(Role) == true ? "" : Role);
                        cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(Session["idx"].ToString()) == true ? "" : Session["idx"]);
                        cmd.Parameters.AddWithValue("@COMMAND", string.IsNullOrEmpty("DELETE") == true ? "" : "DELETE");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                //TempData["messageRequest"] = "<script>alert('Data will be updated after being approved by HR');</script>";
                TempData["message"] = "<script>alert('Data Deleted');</script>";
                return RedirectToAction("ManageLogin", "Home");
            }
            else if(submit == "Back")
            {
                return RedirectToAction("ManageLogin", "Home");
            }
            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Add_ASMIK()
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";

            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            model.Product_ASMIK = "ASMIK PA";
            model.Jenis_Kelamin_1_ddl = DDL_JENISKELAMIN();
            model.HUB_DGN_NASABAH_DDL = DDL_HUBNASABAH();
            model.Kode_Cabang_BMRI_DDL = DDL_KODE_CABANG_BMRI();

            DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_DATA_HEADER_NB] '" + model.No_KTP_Nasabah + "'");
            if (dt01.Rows.Count > 0)
            {
                model.Nama_Cabang_BMRI = dt01.Rows[0]["Nama_Cabang_BMRI"].ToString();
                model.Product_ASMIK = dt01.Rows[0]["Product_ASMIK"].ToString();
                model.Nama_Lengkap_Nasabah = dt01.Rows[0]["Nama_Lengkap_Nasabah"].ToString();
                model.No_Rekening_Nasabah = dt01.Rows[0]["No_Rekening_Nasabah"].ToString();
                model.Nama_Lengkap_Nasabah = dt01.Rows[0]["Nama_Lengkap_Nasabah"].ToString();
                model.Kode_MO_BMRI = dt01.Rows[0]["Kode_MO_BMRI"].ToString();
                model.Kode_Cabang_BMRI = dt01.Rows[0]["Kode_Cabang_BMRI"].ToString();
                model.No_KTP_Nasabah = dt01.Rows[0]["No_KTP_Nasabah"].ToString();
                model.Nama_Lengkap_AHLI_WARIS = dt01.Rows[0]["NAMA_LENGKAP_AHLI_WARIS"].ToString();
                model.HUB_DGN_NASABAH = dt01.Rows[0]["HUB_DGN_NASABAH_AHLI_WARIS"].ToString();
                model.Alamat_AHLI_WARIS = dt01.Rows[0]["ALAMAT_AHLI_WARIS"].ToString();
            }

            model.Content = ListNB(0);

            return View(model);
        }
        [HttpPost]
        public ActionResult Add_ASMIK(PagedList<ASMIKModels> model = null, string Submit ="")
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            model.Product_ASMIK = "ASMIK PA";
            model.Jenis_Kelamin_1_ddl = DDL_JENISKELAMIN();
            model.HUB_DGN_NASABAH_DDL = DDL_HUBNASABAH();
            model.Kode_Cabang_BMRI_DDL = DDL_KODE_CABANG_BMRI();

            Int32 countingPolis = 0;

            if (model.KTP_NO_SEARCHING == "0" || model.KTP_NO_SEARCHING == "")
            {
                model.KTP_NO_SEARCHING = "";
            }

            if(model.Penghasilan_Perbulan_1 == "0" || model.Penghasilan_Perbulan_1 == "")
            {
                model.Penghasilan_Perbulan_1 = "0";
            }


            Int32 CountHeader = 0;
            DataTable dtHeader = Common.ExecuteQuery("dbo.[sp_GET_DATA_HEADER_NB] '" + model.No_KTP_Nasabah + "'");
            if (dtHeader.Rows.Count > 0)
            {
                CountHeader = Convert.ToInt32(dtHeader.Rows[0]["COUNTING"].ToString());
            }
            else
            {
                CountHeader = 0;
            }

            if(CountHeader > 0)
            {
                model.Content = ListNB_By_KTP(("0"));
                TempData["messageRequest"] = "<script>alert('Data Nasabah Sudah Pernah di Input');</script>";
                return View(model);
            }

            if (Submit == "Add")
            {
                if (model.Nama_Lengkap_Tertanggung_1 != null && model.Nama_Lengkap_Tertanggung_1 != "")
                {
                    if (ModelState.IsValid)
                    {
                        model.Content = ListNB_By_KTP((model.No_KTP_Nasabah));

                        DataTable dt01X = Common.ExecuteQuery("dbo.[sp_GET_COUNT_NB_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                        if (dt01X.Rows.Count > 0)
                        {
                            countingPolis = Convert.ToInt32(dt01X.Rows[0]["COUNTING"].ToString());
                        }

                        if (countingPolis > 5)
                        {
                            TempData["messageRequest"] = "<script>alert('Jumlah data Tertanggung sudah melebihi Maksimum input Data');</script>";
                            return View(model);
                        }

                        if (model.Kode_MO_BMRI == "" || model.Kode_MO_BMRI == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Kode MO BMRI');</script>";
                            return View(model);
                        }

                        if (model.Kode_Cabang_BMRI == "" || model.Kode_Cabang_BMRI == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Kode Cabang BMRI');</script>";
                            return View(model);
                        }

                        if (model.Nama_Cabang_BMRI == "" || model.Nama_Cabang_BMRI == null)
                        {
                            model.Nama_Cabang_BMRI = "";
                            TempData["messageRequest"] = "<script>alert('Please Fill Nama Cabang BMRI');</script>";
                            return View(model);

                        }

                        if (model.Product_ASMIK == "" || model.Product_ASMIK == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Product ASMIK');</script>";
                            return View(model);
                        }

                        if (model.Nama_Lengkap_Nasabah == "" || model.Nama_Lengkap_Nasabah == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Nama Lengkap Nasabah');</script>";
                            return View(model);
                        }


                        if (model.No_Rekening_Nasabah == "" || model.No_Rekening_Nasabah == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill No. Rekening Nasabah');</script>";
                            return View(model);
                        }

                        if (model.No_KTP_Nasabah == "" || model.No_KTP_Nasabah == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill No. KTP Nasabah');</script>";
                            return View(model);
                        }

                        DataTable dtREFFNO = Common.ExecuteQuery("dbo.[sp_GET_REFFNO_ASMIK] '" + model.No_KTP_Nasabah + "'");
                        if (dtREFFNO.Rows.Count > 0)
                        {
                            model.ReffNo_ASMIK = dtREFFNO.Rows[0]["ReffNo_ASMIK"].ToString();
                        }

                        if (model.Tanggal_Lahir_1 == "" && model.Tanggal_Lahir_1 == null)
                        {
                            model.Tanggal_Lahir_1 = "1900-01-01";
                        }

                        if (model.Nama_Lengkap_Tertanggung_1 == "" || model.Nama_Lengkap_Tertanggung_1 == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Nama Tertanggung');</script>";
                            return View(model);
                        }

                        if (model.Alamat_1 == "" || model.Alamat_1 == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Alamat Tertanggung');</script>";
                            return View(model);
                        }

                        if (model.Tempat_Lahir_1 == "" || model.Tempat_Lahir_1 == null)
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Tempat Lahir Tertanggung');</script>";
                            return View(model);
                        }

                        if (model.Tanggal_Lahir_1 == "" || model.Tanggal_Lahir_1 == null || model.Tanggal_Lahir_1 == "1900-01-01")
                        {
                            TempData["messageRequest"] = "<script>alert('Please Fill Tanggal Lahir Tertanggung');</script>";
                            return View(model);
                        }

                        if (model.Jenis_Kelamin_1 == "" || model.Jenis_Kelamin_1 == null || model.Jenis_Kelamin_1 == "--Choose--")
                        {
                            TempData["messageRequest"] = "<script>alert('Please Choose Jenis Kelamin Tertanggung');</script>";
                            return View(model);
                        }

                        //if (model.No_KTP_1 == "" || model.No_KTP_1 == null)
                        //{
                        //    TempData["messageRequest"] = "<script>alert('Please Fill No. KTP Tertanggung');</script>";
                        //    return View(model);
                        //}

                        //if (model.No_HP_1 == "" || model.No_HP_1 == null)
                        //{
                        //    TempData["messageRequest"] = "<script>alert('Please Fill No. HP Tertanggung');</script>";
                        //    return View(model);
                        //}

                       
                        DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_REFF_NO_ASMIK_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                        if (dt01.Rows.Count > 0)
                        {
                            model.ReffNo_ASMIK = dt01.Rows[0]["REFFNO"].ToString();
                        }


                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                        {
                            using (SqlCommand cmd = new SqlCommand("sp_INSERT_TRN_ASMIK_NB", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@BUTTON", string.IsNullOrEmpty(Submit) == true ? "" : Submit);
                                cmd.Parameters.AddWithValue("@Kode_MO_BMRI", string.IsNullOrEmpty(model.Kode_MO_BMRI) == true ? "" : model.Kode_MO_BMRI);
                                cmd.Parameters.AddWithValue("@Kode_Cabang_BMRI", string.IsNullOrEmpty(model.Kode_Cabang_BMRI) == true ? "" : model.Kode_Cabang_BMRI);
                                cmd.Parameters.AddWithValue("@Nama_Cabang_BMRI", string.IsNullOrEmpty(model.Nama_Cabang_BMRI) == true ? "" : model.Nama_Cabang_BMRI);
                                cmd.Parameters.AddWithValue("@Product_ASMIK", string.IsNullOrEmpty(model.Product_ASMIK) == true ? "" : model.Product_ASMIK);
                                cmd.Parameters.AddWithValue("@Nama_Lengkap_Nasabah", string.IsNullOrEmpty(model.Nama_Lengkap_Nasabah) == true ? "" : model.Nama_Lengkap_Nasabah);
                                cmd.Parameters.AddWithValue("@No_KTP_Nasabah", string.IsNullOrEmpty(model.No_KTP_Nasabah) == true ? "" : model.No_KTP_Nasabah);

                                cmd.Parameters.AddWithValue("@No_Rekening_Nasabah", string.IsNullOrEmpty(model.No_Rekening_Nasabah) == true ? "" : model.No_Rekening_Nasabah);
                                cmd.Parameters.AddWithValue("@Status", "");
                                cmd.Parameters.AddWithValue("@Nama_Lengkap_Tertanggung", string.IsNullOrEmpty(model.Nama_Lengkap_Tertanggung_1) == true ? "" : model.Nama_Lengkap_Tertanggung_1);
                                cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(model.Alamat_1) == true ? "" : model.Alamat_1);
                                cmd.Parameters.AddWithValue("@Kode_Pos", string.IsNullOrEmpty(model.Kode_Pos_1) == true ? "" : model.Kode_Pos_1);

                                cmd.Parameters.AddWithValue("@Tempat_Lahir", string.IsNullOrEmpty(model.Tempat_Lahir_1) == true ? "" : model.Tempat_Lahir_1);
                                cmd.Parameters.AddWithValue("@Tanggal_Lahir", string.IsNullOrEmpty(model.Tanggal_Lahir_1) == true ? "1900-01-01" : GetDateInYYYYMMDD(model.Tanggal_Lahir_1));
                                cmd.Parameters.AddWithValue("@Jenis_Kelamin", string.IsNullOrEmpty(model.Jenis_Kelamin_1) == true ? "" : model.Jenis_Kelamin_1);
                                cmd.Parameters.AddWithValue("@No_KTP", string.IsNullOrEmpty(model.No_KTP_1) == true ? "" : model.No_KTP_1);
                                cmd.Parameters.AddWithValue("@No_HP", string.IsNullOrEmpty(model.No_HP_1) == true ? "" : model.No_HP_1);

                                cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(model.Email_1) == true ? "" : model.Email_1);
                                cmd.Parameters.AddWithValue("@NPWP", string.IsNullOrEmpty(model.NPWP_1) == true ? "" : model.NPWP_1);
                                cmd.Parameters.AddWithValue("@Pekerjaan", string.IsNullOrEmpty(model.Pekerjaan_1) == true ? "" : model.Pekerjaan_1);
                                cmd.Parameters.AddWithValue("@Penghasilan_Perbulan", string.IsNullOrEmpty(model.Penghasilan_Perbulan_1) == true ? 0 : Convert.ToDecimal(model.Penghasilan_Perbulan_1));
                                cmd.Parameters.AddWithValue("@Sumber_penghasilan", string.IsNullOrEmpty(model.Sumber_penghasilan_1) == true ? "" : model.Sumber_penghasilan_1);

                                cmd.Parameters.AddWithValue("@Created_By", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                                cmd.Parameters.AddWithValue("@IDX_ASMIK", string.IsNullOrEmpty(model.IDX_ASMIK) == true ? "0" : model.IDX_ASMIK);
                                cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(model.IDX_ASMIK_DETAIL) == true ? "0" : model.IDX_ASMIK_DETAIL);

                                cmd.Parameters.AddWithValue("@NAMA_LENGKAP_AHLI_WARIS", string.IsNullOrEmpty(model.Nama_Lengkap_AHLI_WARIS) == true ? "" : model.Nama_Lengkap_AHLI_WARIS);
                                cmd.Parameters.AddWithValue("@HUB_DGN_NASABAH_AHLI_WARIS", string.IsNullOrEmpty(model.HUB_DGN_NASABAH) == true ? "" : (model.HUB_DGN_NASABAH));
                                cmd.Parameters.AddWithValue("@ALAMAT_AHLI_WARIS", string.IsNullOrEmpty(model.Alamat_AHLI_WARIS) == true ? "" : model.Alamat_AHLI_WARIS);
                                cmd.Parameters.AddWithValue("@ReffNo_ASMIK", string.IsNullOrEmpty(model.ReffNo_ASMIK) == true ? "" : model.ReffNo_ASMIK);


                                conn.Open();
                                cmd.ExecuteNonQuery();
                                conn.Close();
                            }
                        }

                        string Redirect = "Edit_ASMIK";
                        TempData["messageRequest"] = "<script>alert('Submit Data Success');</script>";
                        return RedirectToAction(Redirect.Trim(), new { IdxHeader = model.No_KTP_Nasabah });
                    }
                    else
                    {
                        string error = "";
                        foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                        {
                            if (error == "")
                            {
                                error = state.Errors[0].ErrorMessage.ToString();
                            }
                            else
                            {
                                error = error + "; " + state.Errors[0].ErrorMessage.ToString();
                            }
                        }

                        TempData["messageRequest"] = "<script>alert('Please Check : " + error + "');</script>";
                    }
                }
                else
                {
                    TempData["messageRequest"] = "<script>alert('Please Fill Nama Tertanggung');</script>";
                    return View(model);
                }
            }
            else if (Submit == "Draft")
            {

            }
            else if (Submit == "Submits")
            {

            }
            else if (Submit == "Back")
            {
                return RedirectToAction("Search_NB_ASMIK");
            }
            model.Content = ListNB_By_KTP((model.No_KTP_Nasabah));
            return View(model);
        }
        public ActionResult DownloadPDF()
        {
           
            //Session["UserID"] = "HIMAWAN SUTANTO";

            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();


            model.Content = ListNB_By_KTP("0");

            return View(model);
        }
        public ActionResult DownloadPDF_LIST(string ReffNo_List)
        {
            string partialName = ReffNo_List;
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("\\\\wrccare03uat\\CERTIFICATE_2019\\");
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*");
            string pdfPath = "";
            foreach (FileInfo foundFile in filesInDir)
            {
                string fullName = foundFile.FullName;
                string FileName = foundFile.Name;
                pdfPath = "\\\\wrccare03uat\\CERTIFICATE_2019\\" + FileName;
            }

            // FIND FILE
            //string pdfPath = "\\\\wrccare03uat\\CERTIFICATE_2019\\" + model.ReffNo_ASMIK + ".pdf";// AP2019112100600A0000502.pdf";

            if (System.IO.File.Exists(pdfPath))
            {
                byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPath);
                //string pdfBase64 = Convert.ToBase64String(pdfBytes);
                string FileName = System.IO.Path.GetFileName(pdfPath);
                string filePath = Server.MapPath("~/Certificate/" + FileName);
                System.IO.File.WriteAllBytes(filePath, pdfBytes);

                Response.ContentType = "application/pdf";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + ReffNo_List + ".pdf");
                Response.TransmitFile(filePath);
                Response.End();
            }
            else
            {
                TempData["messageRequest"] = "<script>alert('File Not Found');</script>";
                return RedirectToAction("DownloadPDF".Trim());

            }
            return View();
        }
        [HttpPost]
        public ActionResult DownloadPDF(PagedList<ASMIKModels> model)
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            

            model.Content = ListNB_By_KTP_DOWNLOAD(model.KTP_NO_SEARCHING);
            return View(model);
        }
        public ActionResult Edit_ASMIK(string idxHeader, string edit)
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            string KTPNO = "";
            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            model.Product_ASMIK = "ASMIK PA";

            string url = Request.Url.OriginalString;
            Session["urlEdit"] = url;
            model.LINK = url;
            model.Disabled = true;

            KTPNO = Request.QueryString["idxHeader"];

            model.Jenis_Kelamin_1_ddl = DDL_JENISKELAMIN();
            model.HUB_DGN_NASABAH_DDL = DDL_HUBNASABAH();
            model.Kode_Cabang_BMRI_DDL = DDL_KODE_CABANG_BMRI();
            

            if (KTPNO != "0" || KTPNO != "")
            {
                DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_DATA_HEADER_NB] '" + KTPNO + "'");
                if (dt01.Rows.Count > 0)
                {
                    model.Nama_Cabang_BMRI = dt01.Rows[0]["Nama_Cabang_BMRI"].ToString();
                    model.Product_ASMIK = dt01.Rows[0]["Product_ASMIK"].ToString();
                    model.Nama_Lengkap_Nasabah = dt01.Rows[0]["Nama_Lengkap_Nasabah"].ToString();
                    model.No_Rekening_Nasabah = dt01.Rows[0]["No_Rekening_Nasabah"].ToString();
                    model.Nama_Lengkap_Nasabah = dt01.Rows[0]["Nama_Lengkap_Nasabah"].ToString();
                    model.Kode_MO_BMRI = dt01.Rows[0]["Kode_MO_BMRI"].ToString();
                    model.Kode_Cabang_BMRI = dt01.Rows[0]["Kode_Cabang_BMRI"].ToString();
                    model.No_KTP_Nasabah = dt01.Rows[0]["No_KTP_Nasabah"].ToString();
                    model.Nama_Lengkap_AHLI_WARIS = dt01.Rows[0]["NAMA_LENGKAP_AHLI_WARIS"].ToString();
                    model.HUB_DGN_NASABAH = dt01.Rows[0]["HUB_DGN_NASABAH_AHLI_WARIS"].ToString();
                    model.Alamat_AHLI_WARIS = dt01.Rows[0]["ALAMAT_AHLI_WARIS"].ToString();
                    model.ReffNo_ASMIK  = dt01.Rows[0]["ReffNo_ASMIK"].ToString(); 
                }
            }
            model.Disabled = true;
            model.DisabledSubmit = false;
            model.DisabledDraft = false;
            model.Content = ListNB_By_KTP(KTPNO);
            //for (int i = 0; i < model.Content.Count; i++)
            //{
            //    if (model.Content[i].Status == "SUBMIT")
            //    {
            //        edit.disable
            //    }
            //    else
            //    {
            //        GridViewRowEventArgs e = null;
            //        e.Row.Cells[6].Enabled = true;
            //    }

            //}
            //if (model.Status.ToUpper() == "SUBMIT")
            //{
            //    model.Disabled = false;
            //    model.DisabledSubmit = true;
            //    model.DisabledDraft = true;
            //}
            //else
            //{
            //    if (model.Status.ToUpper() == "DRAFT")
            //    {
            //        model.Disabled = true;
            //        model.DisabledSubmit = false;
            //        model.DisabledDraft = false;
            //    }
            //    else
            //    {
            //        model.Disabled = true;
            //        model.DisabledSubmit = false;
            //        model.DisabledDraft = false;
            //    }
            //}

            return View(model);
        }
        [HttpPost]
        public ActionResult Edit_ASMIK(PagedList<ASMIKModels> model = null, string Submit ="", string idxHeader = "")
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            Session["url"] = model.LINK;
            model.Product_ASMIK = "ASMIK PA";
            model.Jenis_Kelamin_1_ddl = DDL_JENISKELAMIN();
            model.HUB_DGN_NASABAH_DDL = DDL_HUBNASABAH();
            model.Kode_Cabang_BMRI_DDL = DDL_KODE_CABANG_BMRI();

        

            if (model.No_KTP_Nasabah != "0" || model.No_KTP_Nasabah != "")
            {
                if (Submit == "Add")
                {
                    if (model.Nama_Lengkap_Tertanggung_1 != null && model.Nama_Lengkap_Tertanggung_1 != "")
                    {
                        if (ModelState.IsValid)
                        {
                            DataTable dt01X = Common.ExecuteQuery("dbo.[sp_GET_COUNT_NB_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                            if (dt01X.Rows.Count > 0)
                            {
                                model.JumlahPolis = Convert.ToInt32(dt01X.Rows[0]["COUNTING"].ToString());
                            }

                            //if (model.JumlahPolis >= 5)
                            //{

                            //    return Content("<script language='javascript' type='text/javascript'>alert('Jumlah Polis tertanggung melebihi batas Maksimum pertanggungan.');window.location.href = '" + Session["urlEdit"].ToString() + "' ;</script>");
                            //}

                            if (model.Tanggal_Lahir_1 == "" || model.Tanggal_Lahir_1 == null)
                            {
                                model.Tanggal_Lahir_1 = "1900-01-01";
                            }
                            DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_REFF_NO_ASMIK_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                            if (dt01.Rows.Count > 0)
                            {
                                model.ReffNo_ASMIK = dt01.Rows[0]["REFFNO"].ToString();
                            }

                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                            {
                                using (SqlCommand cmd = new SqlCommand("sp_INSERT_TRN_ASMIK_NB", conn))
                                {
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.AddWithValue("@BUTTON", string.IsNullOrEmpty(Submit) == true ? "" : Submit);
                                    cmd.Parameters.AddWithValue("@Kode_MO_BMRI", string.IsNullOrEmpty(model.Kode_MO_BMRI) == true ? "" : model.Kode_MO_BMRI);
                                    cmd.Parameters.AddWithValue("@Kode_Cabang_BMRI", string.IsNullOrEmpty(model.Kode_Cabang_BMRI) == true ? "" : model.Kode_Cabang_BMRI);
                                    cmd.Parameters.AddWithValue("@Nama_Cabang_BMRI", string.IsNullOrEmpty(model.Nama_Cabang_BMRI) == true ? "" : model.Nama_Cabang_BMRI);
                                    cmd.Parameters.AddWithValue("@Product_ASMIK", string.IsNullOrEmpty(model.Product_ASMIK) == true ? "" : model.Product_ASMIK);
                                    cmd.Parameters.AddWithValue("@Nama_Lengkap_Nasabah", string.IsNullOrEmpty(model.Nama_Lengkap_Nasabah) == true ? "" : model.Nama_Lengkap_Nasabah);
                                    cmd.Parameters.AddWithValue("@No_KTP_Nasabah", string.IsNullOrEmpty(model.No_KTP_Nasabah) == true ? "" : model.No_KTP_Nasabah);

                                    cmd.Parameters.AddWithValue("@No_Rekening_Nasabah", string.IsNullOrEmpty(model.No_Rekening_Nasabah) == true ? "" : model.No_Rekening_Nasabah);
                                    cmd.Parameters.AddWithValue("@Status", "");
                                    cmd.Parameters.AddWithValue("@Nama_Lengkap_Tertanggung", string.IsNullOrEmpty(model.Nama_Lengkap_Tertanggung_1) == true ? "" : model.Nama_Lengkap_Tertanggung_1);
                                    cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(model.Alamat_1) == true ? "" : model.Alamat_1);
                                    cmd.Parameters.AddWithValue("@Kode_Pos", string.IsNullOrEmpty(model.Kode_Pos_1) == true ? "" : model.Kode_Pos_1);

                                    cmd.Parameters.AddWithValue("@Tempat_Lahir", string.IsNullOrEmpty(model.Tempat_Lahir_1) == true ? "" : model.Tempat_Lahir_1);
                                    cmd.Parameters.AddWithValue("@Tanggal_Lahir", string.IsNullOrEmpty(model.Tanggal_Lahir_1) == true ? "1900-01-01" : GetDateInYYYYMMDD(model.Tanggal_Lahir_1));
                                    cmd.Parameters.AddWithValue("@Jenis_Kelamin", string.IsNullOrEmpty(model.Jenis_Kelamin_1) == true ? "" : model.Jenis_Kelamin_1);
                                    cmd.Parameters.AddWithValue("@No_KTP", string.IsNullOrEmpty(model.No_KTP_1) == true ? "" : model.No_KTP_1);
                                    cmd.Parameters.AddWithValue("@No_HP", string.IsNullOrEmpty(model.No_HP_1) == true ? "" : model.No_HP_1);

                                    cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(model.Email_1) == true ? "" : model.Email_1);
                                    cmd.Parameters.AddWithValue("@NPWP", string.IsNullOrEmpty(model.NPWP_1) == true ? "" : model.NPWP_1);
                                    cmd.Parameters.AddWithValue("@Pekerjaan", string.IsNullOrEmpty(model.Pekerjaan_1) == true ? "" : model.Pekerjaan_1);
                                    cmd.Parameters.AddWithValue("@Penghasilan_Perbulan", string.IsNullOrEmpty(model.Penghasilan_Perbulan_1) == true ? 0 : Convert.ToDecimal(model.Penghasilan_Perbulan_1));
                                    cmd.Parameters.AddWithValue("@Sumber_penghasilan", string.IsNullOrEmpty(model.Sumber_penghasilan_1) == true ? "" : model.Sumber_penghasilan_1);

                                    cmd.Parameters.AddWithValue("@Created_By", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                                    cmd.Parameters.AddWithValue("@IDX_ASMIK", string.IsNullOrEmpty(model.IDX_ASMIK) == true ? "0" : model.IDX_ASMIK);
                                    cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(model.IDX_ASMIK_DETAIL) == true ? "0" : model.IDX_ASMIK_DETAIL);

                                    cmd.Parameters.AddWithValue("@NAMA_LENGKAP_AHLI_WARIS", string.IsNullOrEmpty(model.Nama_Lengkap_AHLI_WARIS) == true ? "" : model.Nama_Lengkap_AHLI_WARIS);
                                    cmd.Parameters.AddWithValue("@HUB_DGN_NASABAH_AHLI_WARIS", string.IsNullOrEmpty(model.HUB_DGN_NASABAH) == true ? "" : (model.HUB_DGN_NASABAH));
                                    cmd.Parameters.AddWithValue("@ALAMAT_AHLI_WARIS", string.IsNullOrEmpty(model.Alamat_AHLI_WARIS) == true ? "" : model.Alamat_AHLI_WARIS);
                                    cmd.Parameters.AddWithValue("@ReffNo_ASMIK", string.IsNullOrEmpty(model.ReffNo_ASMIK) == true ? "" : model.ReffNo_ASMIK);


                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                    conn.Close();
                                }
                            }

                            string Redirect = "Edit_ASMIK";
                            TempData["messageRequest"] = "<script>alert('Submit Data Success');</script>";
                            return RedirectToAction(Redirect.Trim(), new { IdxHeader = model.No_KTP_Nasabah });
                        }
                        else
                        {
                            string error = "";
                            foreach (ModelState state in ViewData.ModelState.Values.Where(x => x.Errors.Count > 0))
                            {
                                if (error == "")
                                {
                                    error = state.Errors[0].ErrorMessage.ToString();
                                }
                                else
                                {
                                    error = error + "; " + state.Errors[0].ErrorMessage.ToString();
                                }
                            }

                            TempData["messageRequest"] = "<script>alert('Please Check : " + error + "');</script>";
                        }
                    }
                    else
                    {
                        TempData["messageRequest"] = "<script>alert('Please Fill Nama Tertanggung');</script>";
                        return RedirectToAction("Edit_ASMIK".Trim(), new { IdxHeader = model.No_KTP_Nasabah });
                    }
                }
                else if (Submit == "Draft")
                {
                    DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_REFF_NO_ASMIK_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                    if (dt01.Rows.Count > 0)
                    {
                        model.ReffNo_ASMIK = dt01.Rows[0]["REFFNO"].ToString();
                    }
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_INSERT_TRN_ASMIK_NB", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@BUTTON", string.IsNullOrEmpty(Submit) == true ? "" : Submit);
                            cmd.Parameters.AddWithValue("@Kode_MO_BMRI", string.IsNullOrEmpty(model.Kode_MO_BMRI) == true ? "" : model.Kode_MO_BMRI);
                            cmd.Parameters.AddWithValue("@Kode_Cabang_BMRI", string.IsNullOrEmpty(model.Kode_Cabang_BMRI) == true ? "" : model.Kode_Cabang_BMRI);
                            cmd.Parameters.AddWithValue("@Nama_Cabang_BMRI", string.IsNullOrEmpty(model.Nama_Cabang_BMRI) == true ? "" : model.Nama_Cabang_BMRI);
                            cmd.Parameters.AddWithValue("@Product_ASMIK", string.IsNullOrEmpty(model.Product_ASMIK) == true ? "" : model.Product_ASMIK);
                            cmd.Parameters.AddWithValue("@Nama_Lengkap_Nasabah", string.IsNullOrEmpty(model.Nama_Lengkap_Nasabah) == true ? "" : model.Nama_Lengkap_Nasabah);
                            cmd.Parameters.AddWithValue("@No_KTP_Nasabah", string.IsNullOrEmpty(model.No_KTP_Nasabah) == true ? "" : model.No_KTP_Nasabah);

                            cmd.Parameters.AddWithValue("@No_Rekening_Nasabah", string.IsNullOrEmpty(model.No_Rekening_Nasabah) == true ? "" : model.No_Rekening_Nasabah);
                            cmd.Parameters.AddWithValue("@Status", "");
                            cmd.Parameters.AddWithValue("@Nama_Lengkap_Tertanggung", string.IsNullOrEmpty(model.Nama_Lengkap_Tertanggung_1) == true ? "" : model.Nama_Lengkap_Tertanggung_1);
                            cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(model.Alamat_1) == true ? "" : model.Alamat_1);
                            cmd.Parameters.AddWithValue("@Kode_Pos", string.IsNullOrEmpty(model.Kode_Pos_1) == true ? "" : model.Kode_Pos_1);

                            cmd.Parameters.AddWithValue("@Tempat_Lahir", string.IsNullOrEmpty(model.Tempat_Lahir_1) == true ? "" : model.Tempat_Lahir_1);
                            cmd.Parameters.AddWithValue("@Tanggal_Lahir", string.IsNullOrEmpty(model.Tanggal_Lahir_1) == true ? "1900-01-01" : GetDateInYYYYMMDD(model.Tanggal_Lahir_1));
                            cmd.Parameters.AddWithValue("@Jenis_Kelamin", string.IsNullOrEmpty(model.Jenis_Kelamin_1) == true ? "" : model.Jenis_Kelamin_1);
                            cmd.Parameters.AddWithValue("@No_KTP", string.IsNullOrEmpty(model.No_KTP_1) == true ? "" : model.No_KTP_1);
                            cmd.Parameters.AddWithValue("@No_HP", string.IsNullOrEmpty(model.No_HP_1) == true ? "" : model.No_HP_1);

                            cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(model.Email_1) == true ? "" : model.Email_1);
                            cmd.Parameters.AddWithValue("@NPWP", string.IsNullOrEmpty(model.NPWP_1) == true ? "" : model.NPWP_1);
                            cmd.Parameters.AddWithValue("@Pekerjaan", string.IsNullOrEmpty(model.Pekerjaan_1) == true ? "" : model.Pekerjaan_1);
                            cmd.Parameters.AddWithValue("@Penghasilan_Perbulan", string.IsNullOrEmpty(model.Penghasilan_Perbulan_1) == true ? 0 : Convert.ToDecimal(model.Penghasilan_Perbulan_1));
                            cmd.Parameters.AddWithValue("@Sumber_penghasilan", string.IsNullOrEmpty(model.Sumber_penghasilan_1) == true ? "" : model.Sumber_penghasilan_1);

                            cmd.Parameters.AddWithValue("@Created_By", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@IDX_ASMIK", string.IsNullOrEmpty(model.IDX_ASMIK) == true ? "0" : model.IDX_ASMIK);
                            cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(model.IDX_ASMIK_DETAIL) == true ? "0" : model.IDX_ASMIK_DETAIL);

                            cmd.Parameters.AddWithValue("@NAMA_LENGKAP_AHLI_WARIS", string.IsNullOrEmpty(model.Nama_Lengkap_AHLI_WARIS) == true ? "" : model.Nama_Lengkap_AHLI_WARIS);
                            cmd.Parameters.AddWithValue("@HUB_DGN_NASABAH_AHLI_WARIS", string.IsNullOrEmpty(model.HUB_DGN_NASABAH) == true ? "" : (model.HUB_DGN_NASABAH));
                            cmd.Parameters.AddWithValue("@ALAMAT_AHLI_WARIS", string.IsNullOrEmpty(model.Alamat_AHLI_WARIS) == true ? "" : model.Alamat_AHLI_WARIS);
                            cmd.Parameters.AddWithValue("@ReffNo_ASMIK", string.IsNullOrEmpty(model.ReffNo_ASMIK) == true ? "" : model.ReffNo_ASMIK);


                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }

                    Session.Remove("url");
                    Session.Remove("urlEdit");

                    string Redirect = "Search_NB_ASMIK";
                    TempData["messageRequest"] = "<script>alert('Submit Draft Success');</script>";
                    return RedirectToAction(Redirect.Trim());
                }
                else if (Submit == "Submits")
                {
                    DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_REFF_NO_ASMIK_BY_KTP] '" + model.No_KTP_Nasabah + "'");
                    if (dt01.Rows.Count > 0)
                    {
                        model.ReffNo_ASMIK = dt01.Rows[0]["REFFNO"].ToString();
                    }
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_INSERT_TRN_ASMIK_NB", conn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@BUTTON", string.IsNullOrEmpty(Submit) == true ? "" : Submit);
                            cmd.Parameters.AddWithValue("@Kode_MO_BMRI", string.IsNullOrEmpty(model.Kode_MO_BMRI) == true ? "" : model.Kode_MO_BMRI);
                            cmd.Parameters.AddWithValue("@Kode_Cabang_BMRI", string.IsNullOrEmpty(model.Kode_Cabang_BMRI) == true ? "" : model.Kode_Cabang_BMRI);
                            cmd.Parameters.AddWithValue("@Nama_Cabang_BMRI", string.IsNullOrEmpty(model.Nama_Cabang_BMRI) == true ? "" : model.Nama_Cabang_BMRI);
                            cmd.Parameters.AddWithValue("@Product_ASMIK", string.IsNullOrEmpty(model.Product_ASMIK) == true ? "" : model.Product_ASMIK);
                            cmd.Parameters.AddWithValue("@Nama_Lengkap_Nasabah", string.IsNullOrEmpty(model.Nama_Lengkap_Nasabah) == true ? "" : model.Nama_Lengkap_Nasabah);
                            cmd.Parameters.AddWithValue("@No_KTP_Nasabah", string.IsNullOrEmpty(model.No_KTP_Nasabah) == true ? "" : model.No_KTP_Nasabah);

                            cmd.Parameters.AddWithValue("@No_Rekening_Nasabah", string.IsNullOrEmpty(model.No_Rekening_Nasabah) == true ? "" : model.No_Rekening_Nasabah);
                            cmd.Parameters.AddWithValue("@Status", "");
                            cmd.Parameters.AddWithValue("@Nama_Lengkap_Tertanggung", string.IsNullOrEmpty(model.Nama_Lengkap_Tertanggung_1) == true ? "" : model.Nama_Lengkap_Tertanggung_1);
                            cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(model.Alamat_1) == true ? "" : model.Alamat_1);
                            cmd.Parameters.AddWithValue("@Kode_Pos", string.IsNullOrEmpty(model.Kode_Pos_1) == true ? "" : model.Kode_Pos_1);

                            cmd.Parameters.AddWithValue("@Tempat_Lahir", string.IsNullOrEmpty(model.Tempat_Lahir_1) == true ? "" : model.Tempat_Lahir_1);
                            cmd.Parameters.AddWithValue("@Tanggal_Lahir", string.IsNullOrEmpty(model.Tanggal_Lahir_1) == true ? "1900-01-01" : GetDateInYYYYMMDD(model.Tanggal_Lahir_1));
                            cmd.Parameters.AddWithValue("@Jenis_Kelamin", string.IsNullOrEmpty(model.Jenis_Kelamin_1) == true ? "" : model.Jenis_Kelamin_1);
                            cmd.Parameters.AddWithValue("@No_KTP", string.IsNullOrEmpty(model.No_KTP_1) == true ? "" : model.No_KTP_1);
                            cmd.Parameters.AddWithValue("@No_HP", string.IsNullOrEmpty(model.No_HP_1) == true ? "" : model.No_HP_1);

                            cmd.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(model.Email_1) == true ? "" : model.Email_1);
                            cmd.Parameters.AddWithValue("@NPWP", string.IsNullOrEmpty(model.NPWP_1) == true ? "" : model.NPWP_1);
                            cmd.Parameters.AddWithValue("@Pekerjaan", string.IsNullOrEmpty(model.Pekerjaan_1) == true ? "" : model.Pekerjaan_1);
                            cmd.Parameters.AddWithValue("@Penghasilan_Perbulan", string.IsNullOrEmpty(model.Penghasilan_Perbulan_1) == true ? 0 : Convert.ToDecimal(model.Penghasilan_Perbulan_1));
                            cmd.Parameters.AddWithValue("@Sumber_penghasilan", string.IsNullOrEmpty(model.Sumber_penghasilan_1) == true ? "" : model.Sumber_penghasilan_1);

                            cmd.Parameters.AddWithValue("@Created_By", string.IsNullOrEmpty(Session["UserID"].ToString()) == true ? "" : Session["UserID"].ToString());
                            cmd.Parameters.AddWithValue("@IDX_ASMIK", string.IsNullOrEmpty(model.IDX_ASMIK) == true ? "0" : model.IDX_ASMIK);
                            cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(model.IDX_ASMIK_DETAIL) == true ? "0" : model.IDX_ASMIK_DETAIL);

                            cmd.Parameters.AddWithValue("@NAMA_LENGKAP_AHLI_WARIS", string.IsNullOrEmpty(model.Nama_Lengkap_AHLI_WARIS) == true ? "" : model.Nama_Lengkap_AHLI_WARIS);
                            cmd.Parameters.AddWithValue("@HUB_DGN_NASABAH_AHLI_WARIS", string.IsNullOrEmpty(model.HUB_DGN_NASABAH) == true ? "" : (model.HUB_DGN_NASABAH));
                            cmd.Parameters.AddWithValue("@ALAMAT_AHLI_WARIS", string.IsNullOrEmpty(model.Alamat_AHLI_WARIS) == true ? "" : model.Alamat_AHLI_WARIS);
                            cmd.Parameters.AddWithValue("@ReffNo_ASMIK", string.IsNullOrEmpty(model.ReffNo_ASMIK) == true ? "" : model.ReffNo_ASMIK);

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }

                    
                    Session.Remove("url");
                    Session.Remove("urlEdit");
                            
                    string Redirect = "Search_NB_ASMIK";
                    TempData["messageRequest"] = "<script>alert('Submit Data Success');</script>";
                    return RedirectToAction(Redirect.Trim());
                }
                else if (Submit == "Back")
                {
                    Session.Remove("url");
                    Session.Remove("urlEdit");

                    return RedirectToAction("Search_NB_ASMIK");
                }
                else if (Submit == "Excel")
                {
                    // EXCEL
                    DataTable dtExp = Common.ExecuteQuery("dbo.[sp_GET_EXCEL_REPORT] '" + model.No_KTP_Nasabah + "'");
                    if (dtExp.Rows.Count > 0)
                    {
                        DataTable dtReport = new DataTable();

                        #region Kolom
                        dtReport.Columns.Add("ASOURCE");
                        dtReport.Columns.Add("DISCOUNT");
                        dtReport.Columns.Add("GRACE");
                        dtReport.Columns.Add("PAYORID");
                        dtReport.Columns.Add("MASTERF");
                        dtReport.Columns.Add("TOPRO");
                        dtReport.Columns.Add("TOPRO_CURRENCY");
                        dtReport.Columns.Add("TOPRO_TSI");
                        dtReport.Columns.Add("TOPRO_ATSI");
                        dtReport.Columns.Add("STYPE");
                        dtReport.Columns.Add("TOC");
                        dtReport.Columns.Add("MO");
                        dtReport.Columns.Add("SEGMENT");
                        dtReport.Columns.Add("BRANCH");
                        dtReport.Columns.Add("HOLDER_ID");
                        dtReport.Columns.Add("INSURED_ID");
                        dtReport.Columns.Add("INSURED");
                        dtReport.Columns.Add("REGNO");
                        dtReport.Columns.Add("REFNO");
                        dtReport.Columns.Add("INCEPTION");
                        dtReport.Columns.Add("EXPIRY");
                        dtReport.Columns.Add("OBJ_INFO_01");
                        dtReport.Columns.Add("OBJ_INFO_02");
                        dtReport.Columns.Add("DESCRIPTION2");
                        dtReport.Columns.Add("OBJ_INFO_03");
                        dtReport.Columns.Add("OBJ_INFO_04");
                        dtReport.Columns.Add("RISKLOCATIONREMARK");
                        dtReport.Columns.Add("RFLDID");
                        dtReport.Columns.Add("RVALUEID");
                        dtReport.Columns.Add("OBJ_INFO_05");
                        dtReport.Columns.Add("OBJ_INFO_06");
                        dtReport.Columns.Add("OBJ_INFO_07");
                        dtReport.Columns.Add("OBJ_INFO_08");
                        dtReport.Columns.Add("OBJ_INFO_09");
                        dtReport.Columns.Add("OBJ_INFO_10");
                        dtReport.Columns.Add("OBJ_INFO_11");
                        dtReport.Columns.Add("OBJ_INFO_12");
                        dtReport.Columns.Add("OBJ_INFO_13");
                        dtReport.Columns.Add("OBJ_INFO_14");
                        dtReport.Columns.Add("OBJ_INFO_15");
                        dtReport.Columns.Add("OBJ_INFO_16");
                        dtReport.Columns.Add("OBJ_INFO_17");
                        dtReport.Columns.Add("OBJ_INFO_18");
                        dtReport.Columns.Add("OBJ_INFO_19");
                        dtReport.Columns.Add("OBJ_INFO_20");
                        dtReport.Columns.Add("OBJ_INFO_21");
                        dtReport.Columns.Add("OBJ_INFO_22");
                        dtReport.Columns.Add("vessel");
                        dtReport.Columns.Add("CONVEYANCE");
                        dtReport.Columns.Add("TRANSTO");
                        dtReport.Columns.Add("CONSIGNEE");
                        dtReport.Columns.Add("TRANSHIPMENT");
                        dtReport.Columns.Add("ATANDFROM");
                        dtReport.Columns.Add("VOYAGEFROM");
                        dtReport.Columns.Add("VOYAGETO");
                        dtReport.Columns.Add("CADDRESS");
                        dtReport.Columns.Add("TRANSDATE");
                        dtReport.Columns.Add("EXCLUSIVEF");
                        dtReport.Columns.Add("MAINCOVERAGE");
                        dtReport.Columns.Add("MAINCOVERAGEREMARK");
                        dtReport.Columns.Add("RATE");
                        dtReport.Columns.Add("PCALC");
                        dtReport.Columns.Add("MAINUNIT");
                        dtReport.Columns.Add("PPRORATA");
                        dtReport.Columns.Add("MAININTERESTCODE");
                        dtReport.Columns.Add("MAINCURRENCY ");
                        dtReport.Columns.Add("SUM_INSURED ");
                        dtReport.Columns.Add("MAININTERESTREMARK");
                        dtReport.Columns.Add("ADDCOVERAGE1");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK1");
                        dtReport.Columns.Add("ADDRATE1");
                        dtReport.Columns.Add("ADDUNIT1");
                        dtReport.Columns.Add("ADDPPRORATA1");
                        dtReport.Columns.Add("ADDINTERESTCODE1");
                        dtReport.Columns.Add("ADDCURRENCY1 ");
                        dtReport.Columns.Add("ADDSUMINSURED1 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK1");
                        dtReport.Columns.Add("ADDCOVERAGE2");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK2");
                        dtReport.Columns.Add("ADDRATE2");
                        dtReport.Columns.Add("ADDUNIT2");
                        dtReport.Columns.Add("ADDPPRORATA2");
                        dtReport.Columns.Add("ADDINTERESTCODE2");
                        dtReport.Columns.Add("ADDCURRENCY2 ");
                        dtReport.Columns.Add("ADDSUMINSURED2 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK2");
                        dtReport.Columns.Add("ADDCOVERAGE3");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK3");
                        dtReport.Columns.Add("ADDRATE3");
                        dtReport.Columns.Add("ADDUNIT3");
                        dtReport.Columns.Add("ADDPPRORATA3");
                        dtReport.Columns.Add("ADDINTERESTCODE3");
                        dtReport.Columns.Add("ADDCURRENCY3 ");
                        dtReport.Columns.Add("ADDSUMINSURED3 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK3");
                        dtReport.Columns.Add("ADDCOVERAGE4");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK4");
                        dtReport.Columns.Add("ADDRATE4");
                        dtReport.Columns.Add("ADDUNIT4");
                        dtReport.Columns.Add("ADDINTERESTCODE4");
                        dtReport.Columns.Add("ADDCURRENCY4");
                        dtReport.Columns.Add("ADDSUMINSURED4");
                        dtReport.Columns.Add("ADDINTERESTREMARK4");
                        dtReport.Columns.Add("ADDCOVERAGE5");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK5");
                        dtReport.Columns.Add("ADDRATE5");
                        dtReport.Columns.Add("ADDUNIT5");
                        dtReport.Columns.Add("ADDCOVERAGE6");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK6");
                        dtReport.Columns.Add("ADDRATE6");
                        dtReport.Columns.Add("ADDUNIT6");
                        dtReport.Columns.Add("ADDCOVERAGE7");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK7");
                        dtReport.Columns.Add("ADDRATE7");
                        dtReport.Columns.Add("ADDUNIT7");
                        dtReport.Columns.Add("ADDINTERESTCODE4 ");
                        dtReport.Columns.Add("ADDCURRENCY4 ");
                        dtReport.Columns.Add("ADDSUMINSURED4 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK4 ");
                        dtReport.Columns.Add("ADDINTERESTCODE5");
                        dtReport.Columns.Add("ADDCURRENCY5 ");
                        dtReport.Columns.Add("ADDSUMINSURED5 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK5");
                        dtReport.Columns.Add("ADDINTERESTCODE6");
                        dtReport.Columns.Add("ADDCURRENCY6 ");
                        dtReport.Columns.Add("ADDSUMINSURED6 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK6");
                        dtReport.Columns.Add("ADDINTERESTCODE7");
                        dtReport.Columns.Add("ADDCURRENCY7 ");
                        dtReport.Columns.Add("ADDSUMINSURED7 ");
                        dtReport.Columns.Add("ADDINTERESTREMARK7");
                        dtReport.Columns.Add("ADDCOVERAGE8");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK8");
                        dtReport.Columns.Add("ADDRATE8");
                        dtReport.Columns.Add("ADDUNIT8");
                        dtReport.Columns.Add("ADDINTERESTCODE8");
                        dtReport.Columns.Add("ADDSUMINSURED8");
                        dtReport.Columns.Add("ADDCURRENCY8");
                        dtReport.Columns.Add("ADDINTERESTREMARK8");
                        dtReport.Columns.Add("ADDCOVERAGE9");
                        dtReport.Columns.Add("ADDCOVERAGEREMARK9");
                        dtReport.Columns.Add("ADDRATE9");
                        dtReport.Columns.Add("ADDUNIT9");
                        dtReport.Columns.Add("ADDINTERESTCODE9");
                        dtReport.Columns.Add("ADDSUMINSURED9");
                        dtReport.Columns.Add("ADDCURRENCY9");
                        dtReport.Columns.Add("ADDINTERESTREMARK9");
                        dtReport.Columns.Add("CLAORDERNO_01");
                        dtReport.Columns.Add("CLAORDERNO_02");
                        dtReport.Columns.Add("CLAORDERNO_03");
                        dtReport.Columns.Add("CLAORDERNO_04");
                        dtReport.Columns.Add("CLAORDERNO_05");
                        dtReport.Columns.Add("CLAORDERNO_06");
                        dtReport.Columns.Add("CLAORDERNO_07");
                        dtReport.Columns.Add("CLAORDERNO_08");
                        dtReport.Columns.Add("CLAORDERNO_09");
                        dtReport.Columns.Add("CLAORDERNO_10");
                        dtReport.Columns.Add("FEE_ID");
                        dtReport.Columns.Add("FEE_REMARK");
                        dtReport.Columns.Add("FEE_CURRENCY");
                        dtReport.Columns.Add("FEE_AMOUNT");
                        dtReport.Columns.Add("DUTY_ID");
                        dtReport.Columns.Add("DUTY_REMARK");
                        dtReport.Columns.Add("DUTY_CURRENCY");
                        dtReport.Columns.Add("DUTY_AMOUNT");
                        dtReport.Columns.Add("DCODE1");
                        dtReport.Columns.Add("REMARKS1");
                        dtReport.Columns.Add("PCTTSI1");
                        dtReport.Columns.Add("PCTCL1");
                        dtReport.Columns.Add("FIXEDMIN1");
                        dtReport.Columns.Add("FIXEDMAX1");
                        dtReport.Columns.Add("CURRENCY1");
                        dtReport.Columns.Add("DCODE2");
                        dtReport.Columns.Add("REMARKS2");
                        dtReport.Columns.Add("PCTTSI2");
                        dtReport.Columns.Add("PCTCL2");
                        dtReport.Columns.Add("FIXEDMIN2");
                        dtReport.Columns.Add("FIXEDMAX2");
                        dtReport.Columns.Add("CURRENCY2");
                        dtReport.Columns.Add("DCODE3");
                        dtReport.Columns.Add("REMARKS3");
                        dtReport.Columns.Add("PCTTSI3");
                        dtReport.Columns.Add("PCTCL3");
                        dtReport.Columns.Add("FIXEDMIN3");
                        dtReport.Columns.Add("FIXEDMAX3");
                        dtReport.Columns.Add("CURRENCY3");
                        dtReport.Columns.Add("DCODE4");
                        dtReport.Columns.Add("REMARKS4");
                        dtReport.Columns.Add("PCTTSI4");
                        dtReport.Columns.Add("PCTCL4");
                        dtReport.Columns.Add("FIXEDMIN4");
                        dtReport.Columns.Add("FIXEDMAX4");
                        dtReport.Columns.Add("CURRENCY4");
                        dtReport.Columns.Add("DCODE5");
                        dtReport.Columns.Add("REMARKS5");
                        dtReport.Columns.Add("PCTTSI5");
                        dtReport.Columns.Add("PCTCL5");
                        dtReport.Columns.Add("FIXEDMIN5");
                        dtReport.Columns.Add("FIXEDMAX5");
                        dtReport.Columns.Add("CURRENCY5");
                        dtReport.Columns.Add("BSID");
                        dtReport.Columns.Add("BSTYPE");
                        dtReport.Columns.Add("BSFEE");
                        dtReport.Columns.Add("INSTALLMENT_FREQ");
                        dtReport.Columns.Add("INSTALLMENT_DURATION");
                        dtReport.Columns.Add("INSTALLMENT_FIRSTDUE");
                        dtReport.Columns.Add("TOPRO_INTEREST_A");
                        dtReport.Columns.Add("TOPRO_COVERAGE_A");
                        dtReport.Columns.Add("ISTYPE");
                        dtReport.Columns.Add("LEADERFEE");
                        dtReport.Columns.Add("LEADERHF");
                        dtReport.Columns.Add("LEADERID");
                        dtReport.Columns.Add("LEADERSHARE");
                        #endregion
                        foreach (DataRow item in dtExp.Rows)
                        {
                            var row = dtReport.NewRow();

                            row["INSURED"] = item["Nama_Lengkap_Nasabah"].ToString();
                            row["REFNO"] = item["REFFNO_ASMIK_DOWNLOAD"].ToString();
                            row["INCEPTION"] = item["Inception_Date"].ToString();
                            row["EXPIRY"] = item["Expiry_Date"].ToString();
                            row["OBJ_INFO_01"] = (item["Kode_Cabang_BMRI"].ToString());
                            row["OBJ_INFO_02"] = item["Kode_MO_BMRI"].ToString();
                            row["OBJ_INFO_04"] = "ASMIK PA";
                            row["OBJ_INFO_05"] = item["Nama_Lengkap_Tertanggung"].ToString();
                            row["OBJ_INFO_06"] = item["Alamat"].ToString();
                            row["OBJ_INFO_07"] = item["Kode_Pos"].ToString();
                            row["OBJ_INFO_08"] = item["Tempat_Lahir"].ToString();
                            row["OBJ_INFO_09"] = item["Tanggal_Lahir"].ToString();
                            row["OBJ_INFO_10"] = item["Jenis_Kelamin"].ToString();
                            row["OBJ_INFO_11"] = item["No_KTP"].ToString();
                            row["OBJ_INFO_12"] = item["No_HP"].ToString();
                            row["OBJ_INFO_13"] = "";
                            row["OBJ_INFO_14"] = item["Email"].ToString();
                            row["OBJ_INFO_15"] = item["No_Rekening_Nasabah"].ToString();
                            row["OBJ_INFO_16"] = item["NPWP"].ToString();
                            row["OBJ_INFO_17"] = item["Pekerjaan"].ToString();
                            row["OBJ_INFO_18"] = item["Penghasilan_Perbulan"].ToString();
                            row["OBJ_INFO_19"] = item["Sumber_penghasilan"].ToString();
                            row["OBJ_INFO_20"] = item["NAMA_LENGKAP_AHLI_WARIS"].ToString();
                            row["OBJ_INFO_21"] = item["HUB_DGN_NASABAH_AHLI_WARIS"].ToString();
                            row["OBJ_INFO_22"] = item["ALAMAT_AHLI_WARIS"].ToString();
                            row["MAINCOVERAGE"] = "ASMIK-01";
                            row["MAINCOVERAGEREMARK"] = "ASURANSI MIKRO/KECELAKAAN DIRI";
                            row["RATE"] = "50000";
                            row["PCALC"] = "F";
                            row["MAINUNIT"] = "F";
                            row["MAININTERESTCODE"] = "M27";
                            row["MAINCURRENCY "] = "IDR";
                            row["SUM_INSURED "] = "50000000";
                            row["MAININTERESTREMARK"] = "Meninggal Dunia Akibat Kecelakaan, atau Cacat Tetap Total Akibat Kecelakaan";

                            dtReport.Rows.Add(row);
                        }

                        if (dtReport.Rows.Count > 0)
                        {
                            var grid = new GridView();
                            grid.DataSource = dtReport;
                            grid.DataBind();

                            Response.ClearContent();
                            Response.Buffer = true;
                            string filename = "Report ASMIK";
                            Response.AddHeader("content-disposition", "attachment; filename=" + filename + ".xls");
                            Response.ContentType = "application/ms-excel";

                            Response.Charset = "";
                            StringWriter sw = new StringWriter();
                            HtmlTextWriter htw = new HtmlTextWriter(sw);

                            foreach (GridViewRow r in grid.Rows)
                            {
                                if ((r.RowType == DataControlRowType.DataRow))
                                {
                                    for (int columnIndex = 0; (columnIndex
                                                <= (r.Cells.Count - 1)); columnIndex++)
                                    {
                                        r.Cells[columnIndex].Attributes.Add("class", "text");
                                    }

                                }

                            }

                            grid.RenderControl(htw);
                            string style = "<style> .text { mso-number-format:\\@; } </style> ";
                            Response.Write(style);

                            Response.Write(sw.ToString());
                            Response.End();
                            ModelState.Clear();

                        }
                    }

                    // END


                }
                else if (Submit == "Download")
                {
                    string partialName = model.ReffNo_ASMIK;
                    DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo("\\\\wrccare03uat\\CERTIFICATE_2019\\");
                    FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*");
                    string pdfPath = "";
                    foreach (FileInfo foundFile in filesInDir)
                    {
                        string fullName = foundFile.FullName;
                        string FileName = foundFile.Name;
                        pdfPath = "\\\\wrccare03uat\\CERTIFICATE_2019\\" + FileName;
                    }

                    // FIND FILE
                    //string pdfPath = "\\\\wrccare03uat\\CERTIFICATE_2019\\" + model.ReffNo_ASMIK + ".pdf";// AP2019112100600A0000502.pdf";

                    if(System.IO.File.Exists(pdfPath))
                    { 
                        byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPath);
                        //string pdfBase64 = Convert.ToBase64String(pdfBytes);
                        string FileName = System.IO.Path.GetFileName(pdfPath);
                        string filePath = Server.MapPath("~/Certificate/"+ FileName);
                        System.IO.File.WriteAllBytes(filePath, pdfBytes);

                        Response.ContentType = "application/pdf";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + model.ReffNo_ASMIK + ".pdf");
                        Response.TransmitFile(filePath);
                        Response.End();
                    }
                    else
                    {
                        TempData["messageRequest"] = "<script>alert('File Not Found');window.close();</script>";
                        return RedirectToAction("Edit_ASMIK".Trim(), new { IdxHeader = model.No_KTP_Nasabah });
                    
                    }
                    //Response.ContentType = "Application/pdf";
                    //Response.BinaryWrite(pdfBytes);
                    ////Response.WriteFile(filePath);
                    //Response.End();

                    //string url = "/Certificate/ASMIK.pdf";
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append(" <Script type = 'text/javascript'>");
                    //sb.Append("window.open('");
                    //sb.Append(url);
                    //sb.Append("');");
                    //sb.Append("</script>");
                    //ScriptManager.RegisterStartupScript(this.GetType(), "script", sb.ToString());
                    //ScriptManager.RegisterStartupScript(this.GetType(), "script", sb.ToString());
                    //ScriptManager.RegisterStartupScript(this.GetType(), "ShowStatus", "javascript:alert('Record is not updated');", true);

                }
            }

            model.Content = ListNB_By_KTP(model.No_KTP_Nasabah);

            if (model.No_KTP_Nasabah != "0" || model.No_KTP_Nasabah != "")
            {
                DataTable dt01 = Common.ExecuteQuery("dbo.[sp_GET_DATA_DETAIL_NB] '" + model.No_KTP_Nasabah + "'");
                if (dt01.Rows.Count > 0)
                {
                    model.Status = dt01.Rows[0]["Status"].ToString();
                }
            }
            
            if (model.Status.ToUpper() == "SUBMIT")
            {
                model.Disabled = false;
                model.DisabledSubmit = true;
                model.DisabledDraft = true;
            }
            else
            {
                if (model.Status.ToUpper() == "DRAFT")
                { 
                    model.Disabled = true;
                    model.DisabledSubmit = false;
                    model.DisabledDraft = false;
                }
                else
                {
                    model.Disabled = true;
                    model.DisabledSubmit = false;
                    model.DisabledDraft = false;
                }
            }

            return View(model);
        }

        public ActionResult Search_NB_Asmik()
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            string cek = Session["Rolez"].ToString();

            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            
            model.Content = ListNB_By_KTP_Header("1");
            
            return View(model);
        }
        [HttpPost]
        public ActionResult Search_NB_Asmik(PagedList<ASMIKModels> model = null,string Submit ="")
        {
            //Session["UserID"] = "HIMAWAN SUTANTO";
            string cek = Session["Rolez"].ToString();
            model.Content = ListNB_By_KTP("1");
            if (Submit =="Search")
            {
                if(model.KTP_NO_SEARCHING == "" || model.KTP_NO_SEARCHING == null)
                {
                    TempData["messageRequest"] = "<script>alert('Please Fill No. KTP Nasabah');</script>";
                    return View(model);
                }


                model.Content = ListNB_By_KTP_Header(model.KTP_NO_SEARCHING);
            }
            else if(Submit== "Download")
            {
                return RedirectToAction("DownloadPDF");
            }
            else
            {
                return RedirectToAction("Add_ASMIK");
            }
            return View(model);
        }
        public ActionResult Delete(string idxDetail,string idxHeader, PagedList<ASMIKModels> model, string Menu, string KTP)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConSql"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_DELETE_TRN_ASMIK_NB_DETAIL", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IDX", string.IsNullOrEmpty(idxDetail) == true ? "" : idxDetail);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                
                string Redirect = "Edit_ASMIK";
                TempData["messageRequest"] = "<script>alert('Success for Delete Nama Tertanggung');</script>";
                return RedirectToAction(Redirect.Trim(), new { IdxHeader = KTP });

            }
            catch (Exception ex)
            {
                Common.SaveError(ex, "Add_ASMIK", "Home");
            }
            return RedirectToAction("Add_ASMIK", new { msg = "Failed" });
        }
      
        public JsonResult AjaxGetDetailData(string id)
        {
            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            DataTable dt = Common.ExecuteQuery("sp_GET_DETAIL_DATA_ASMIK_DETAIL_NB '" +id + "'");
            if (dt.Rows.Count > 0)
            {
                var record = dt;
                var result = new
                {
                    Nama_Lengkap_Tertanggung_List = dt.Rows[0]["Nama_Lengkap_Tertanggung"].ToString(),
                    Alamat_List = dt.Rows[0]["Alamat"].ToString(),
                    Kode_Pos_List = dt.Rows[0]["Kode_Pos"].ToString(),
                    Tempat_Lahir_List = dt.Rows[0]["Tempat_Lahir"].ToString(),
                    Tanggal_Lahir_List = dt.Rows[0]["Tanggal_Lahir_2"].ToString(),
                    Jenis_Kelamin_List = dt.Rows[0]["Jenis_Kelamin"].ToString(),
                    No_KTP_List = dt.Rows[0]["No_KTP"].ToString(),
                    No_HP_List = dt.Rows[0]["No_HP"].ToString(),
                    Email_List = dt.Rows[0]["Email"].ToString(),
                    NPWP_List = dt.Rows[0]["NPWP"].ToString(),
                    Pekerjaan_List = dt.Rows[0]["Pekerjaan"].ToString(),
                    Penghasilan_Perbulan_List = dt.Rows[0]["Penghasilan_Perbulan"].ToString(),
                    Sumber_penghasilan_List = dt.Rows[0]["Sumber_penghasilan"].ToString(),
                    IDX_ASMIK_DETAIL = dt.Rows[0]["IDX_DETAIL"].ToString(),
                    IDX_HEADER = dt.Rows[0]["IDX_HEADER"].ToString(),
                    Kode_MO_BMRI = dt.Rows[0]["Kode_MO_BMRI"].ToString(),
                    Kode_Cabang_BMRI = dt.Rows[0]["Kode_Cabang_BMRI"].ToString(),
                    Nama_Cabang_BMRI = dt.Rows[0]["Nama_Cabang_BMRI"].ToString(),
                    Product_ASMIK = dt.Rows[0]["Product_ASMIK"].ToString(),
                    Nama_Lengkap_Nasabah = dt.Rows[0]["Nama_Lengkap_Nasabah"].ToString(),
                    No_Rekening_Nasabah = dt.Rows[0]["No_Rekening_Nasabah"].ToString(),

                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public JsonResult AjaxGetDetailData_By_KTP(string id)
        {
            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            ASMIKModels models = new ASMIKModels();

            SqlConnection conn = Common.GetConnection();
            DataTable dt = Common.ExecuteQuery("dbo.[sp_GET_DETAIL_DATA_ASMIK_DETAIL_NB_BY_KTP] '" + id + "'");

            if (dt == null)
            {

            }
            else
            {
                model.IDX_ASMIK = dt.Rows[0]["id_ASMIK_DETAIL"].ToString();
                model.IDX_ASMIK_DETAIL = dt.Rows[0]["IDX_DETAIL"].ToString();
                model.IDX_HEADER = dt.Rows[0]["IDX_HEADER"].ToString();
                model.Kode_MO_BMRI = dt.Rows[0]["Kode_MO_BMRI"].ToString();
                model.Kode_Cabang_BMRI = dt.Rows[0]["Kode_Cabang_BMRI"].ToString();
                model.Nama_Cabang_BMRI = dt.Rows[0]["Nama_Cabang_BMRI"].ToString();
                model.Product_ASMIK = dt.Rows[0]["Product_ASMIK"].ToString();
                model.Nama_Lengkap_Nasabah = dt.Rows[0]["Nama_Lengkap_Nasabah"].ToString();
                model.No_Rekening_Nasabah = dt.Rows[0]["No_Rekening_Nasabah"].ToString();
                model.No_KTP_Nasabah = dt.Rows[0]["No_KTP_Nasabah"].ToString();
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        
        public static List<ASMIKModels> ListNB(int idx)
        {
            SqlConnection conn = Common.GetConnection();
            List<ASMIKModels> model = new List<ASMIKModels>();
            DataTable tbl = Common.ExecuteQuery("dbo.[sp_GET_ASMIK_NB] '" + idx + "'");

            if (tbl == null)
            {
                model.Add(new ASMIKModels
                {
                    Nama_Lengkap_Tertanggung_List = "",
                    Alamat_List = "",
                    Kode_Pos_List = "",
                    Tempat_Lahir_List = "",
                    Tanggal_Lahir_List = "",
                    Jenis_Kelamin_List = "",
                    No_KTP_List = "",
                    No_HP_List = "",
                    Email_List = "",
                    NPWP_List = "",
                    Pekerjaan_List = "",
                    Penghasilan_Perbulan_List = "",
                    Sumber_penghasilan_List = "",
                    IDX_ASMIK = "0",
                    IDX_ASMIK_DETAIL = "0"
                });
            }
            else
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    model.Add(new ASMIKModels
                    {
                        Nama_Lengkap_Tertanggung_List = dr["Nama_Lengkap_Tertanggung"].ToString(),
                        Alamat_List = dr["Alamat"].ToString(),
                        Kode_Pos_List = dr["Kode_Pos"].ToString(),
                        Tempat_Lahir_List = dr["Tempat_Lahir"].ToString(),
                        Tanggal_Lahir_List = dr["Tanggal_Lahir_2"].ToString(),
                        Jenis_Kelamin_List = dr["Jenis_Kelamin"].ToString(),
                        No_KTP_List = dr["No_KTP"].ToString(),
                        No_HP_List = dr["No_HP"].ToString(),
                        Email_List = dr["Email"].ToString(),
                        NPWP_List = dr["NPWP"].ToString(),
                        Pekerjaan_List = dr["Pekerjaan"].ToString(),
                        Penghasilan_Perbulan_List = dr["Penghasilan_Perbulan"].ToString(),
                        Sumber_penghasilan_List = dr["Sumber_penghasilan"].ToString(),
                        IDX_ASMIK = dr["id_ASMIK"].ToString(),
                        IDX_ASMIK_DETAIL = dr["IDX"].ToString()
                    });
                }
            }
            return model;
        }

        public static List<ASMIKModels> ListNB_By_KTP(string KTP)
        {
            SqlConnection conn = Common.GetConnection();
            List<ASMIKModels> model = new List<ASMIKModels>();
            DataTable tbl = Common.ExecuteQuery("dbo.[sp_GET_DETAIL_DATA_ASMIK_DETAIL_NB_BY_KTP] '" + KTP + "'");

            if (tbl == null)
            {
                model.Add(new ASMIKModels
                {
                    Nama_Lengkap_Tertanggung_List = "",
                    Alamat_List = "",
                    Kode_Pos_List = "",
                    Tempat_Lahir_List = "",
                    Tanggal_Lahir_List = "",
                    Jenis_Kelamin_List = "",
                    No_KTP_List = "",
                    No_HP_List = "",
                    Email_List = "",
                    NPWP_List = "",
                    Pekerjaan_List = "",
                    Penghasilan_Perbulan_List = "",
                    Sumber_penghasilan_List = "",
                    IDX_ASMIK = "0",
                    IDX_ASMIK_DETAIL = "0",
                    NomerUrut = "0",
                    Status = "",
                    Visible = ""
                });
            }
            else
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    model.Add(new ASMIKModels
                    {
                        Nama_Lengkap_Tertanggung_List = dr["Nama_Lengkap_Tertanggung"].ToString(),
                        Alamat_List = dr["Alamat"].ToString(),
                        Kode_Pos_List = dr["Kode_Pos"].ToString(),
                        Tempat_Lahir_List = dr["Tempat_Lahir"].ToString(),
                        Tanggal_Lahir_List = dr["Tanggal_Lahir_2"].ToString(),
                        Jenis_Kelamin_List = dr["Jenis_Kelamin"].ToString(),
                        No_KTP_List = dr["No_KTP"].ToString(),
                        No_HP_List = dr["No_HP"].ToString(),
                        Email_List = dr["Email"].ToString(),
                        NPWP_List = dr["NPWP"].ToString(),
                        Pekerjaan_List = dr["Pekerjaan"].ToString(),
                        Penghasilan_Perbulan_List = dr["Penghasilan_Perbulan"].ToString(),
                        Sumber_penghasilan_List = dr["Sumber_penghasilan"].ToString(),
                        IDX_ASMIK_DETAIL = dr["IDX"].ToString(),
                        NomerUrut = dr["Nomer"].ToString(),
                        Status = dr["Status"].ToString(),
                        Visible = dr["Visible"].ToString()
                    });
                }
            }
            return model;
        }
        public static List<ASMIKModels> ListNB_By_KTP_DOWNLOAD(string KTP)
        {
            SqlConnection conn = Common.GetConnection();
            List<ASMIKModels> model = new List<ASMIKModels>();
            DataTable tbl = Common.ExecuteQuery("dbo.[sp_GET_DETAIL_DATA_ASMIK_DETAIL_NB_BY_KTP] '" + KTP + "'");

            if (tbl == null)
            {
                model.Add(new ASMIKModels
                {
                    Nama_Lengkap_Tertanggung_List = "",
                    Alamat_List = "",
                    Kode_Pos_List = "",
                    Tempat_Lahir_List = "",
                    Tanggal_Lahir_List = "",
                    Jenis_Kelamin_List = "",
                    No_KTP_List = "",
                    No_HP_List = "",
                    Email_List = "",
                    NPWP_List = "",
                    Pekerjaan_List = "",
                    Penghasilan_Perbulan_List = "",
                    Sumber_penghasilan_List = "",
                    IDX_ASMIK = "0",
                    IDX_ASMIK_DETAIL = "0",
                    NomerUrut = "0",
                    ReffNo_List = "0"
                });
            }
            else
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    model.Add(new ASMIKModels
                    {
                        Nama_Lengkap_Tertanggung_List = dr["Nama_Lengkap_Tertanggung"].ToString(),
                        Alamat_List = dr["Alamat"].ToString(),
                        Kode_Pos_List = dr["Kode_Pos"].ToString(),
                        Tempat_Lahir_List = dr["Tempat_Lahir"].ToString(),
                        Tanggal_Lahir_List = dr["Tanggal_Lahir_2"].ToString(),
                        Jenis_Kelamin_List = dr["Jenis_Kelamin"].ToString(),
                        No_KTP_List = dr["No_KTP"].ToString(),
                        No_HP_List = dr["No_HP"].ToString(),
                        Email_List = dr["Email"].ToString(),
                        NPWP_List = dr["NPWP"].ToString(),
                        Pekerjaan_List = dr["Pekerjaan"].ToString(),
                        Penghasilan_Perbulan_List = dr["Penghasilan_Perbulan"].ToString(),
                        Sumber_penghasilan_List = dr["Sumber_penghasilan"].ToString(),
                        IDX_ASMIK_DETAIL = dr["IDX"].ToString(),
                        NomerUrut = dr["Nomer"].ToString(),
                        ReffNo_List = dr["REFFNO_ASMIK"].ToString()
                    });
                }
            }
            return model;
        }
        public static List<ASMIKModels> ListNB_By_KTP_Header(string KTP)
        {
            SqlConnection conn = Common.GetConnection();
            List<ASMIKModels> model = new List<ASMIKModels>();
            DataTable tbl = Common.ExecuteQuery("dbo.[sp_GET_DETAIL_DATA_ASMIK_DETAIL_NB_BY_KTP_HEADER] '" + KTP + "'");

            if (tbl == null)
            {
                model.Add(new ASMIKModels
                {
                    No_KTP_List = "",
                    Kode_MO_BMRI = "",
                    Kode_Cabang_BMRI = "",
                    Nama_Cabang_BMRI = "",
                    Product_ASMIK = "",
                    Nama_Lengkap_Nasabah = "",
                    No_Rekening_Nasabah = "",
                });
            }
            else
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    model.Add(new ASMIKModels
                    {
                        No_KTP_List = dr["NO_KTP_NASABAH"].ToString(),
                        Kode_MO_BMRI = dr["Kode_MO_BMRI"].ToString(),
                        Kode_Cabang_BMRI = dr["Kode_Cabang_BMRI"].ToString(),
                        Nama_Cabang_BMRI = dr["Nama_Cabang_BMRI"].ToString(),
                        Product_ASMIK = dr["Product_ASMIK"].ToString(),
                        Nama_Lengkap_Nasabah = dr["Nama_Lengkap_Nasabah"].ToString(),
                        No_Rekening_Nasabah = dr["No_Rekening_Nasabah"].ToString(),
                    });
                }
            }
            return model;
        }
        public static List<ASMIKModels> List_Login(string rolez)
        {
            SqlConnection conn = Common.GetConnection();
            List<ASMIKModels> model = new List<ASMIKModels>();
            DataTable tbl = Common.ExecuteQuery("dbo.[SP_LOGIN] '', '', '"+ rolez +"'");

            if (tbl == null)
            {
                model.Add(new ASMIKModels
                {
                    No = "",
                    Username = "",
                    CreatedBy = "",
                    CreatedDate = "",
                    UpdatedBy = "",
                    UpdatedDate = "",
                    Role = "",
                    Idx_login = ""
                });
            }
            else
            {
                foreach (DataRow dr in tbl.Rows)
                {
                    model.Add(new ASMIKModels
                    {
                        No = dr["No"].ToString(),
                        Username = dr["Username"].ToString(),
                        CreatedBy = dr["CreatedBy"].ToString(),
                        CreatedDate = dr["CreatedDate"].ToString(),
                        UpdatedBy = dr["UpdatedBy"].ToString(),
                        UpdatedDate = dr["UpdatedDate"].ToString(),
                        Role = dr["Role"].ToString(),
                        Idx_login = dr["idx"].ToString()
                    });
                }
            }
            return model;
        }
        private static List<SelectListItem> DDL_JENISKELAMIN()
        {
            SqlConnection con = Common.GetConnection();
            List<SelectListItem> item = new List<SelectListItem>();
            string query = "EXEC [dbo].[SP_JENISKELAMIN] ";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        item.Add(new SelectListItem
                        {
                            Text = " --Choose-- ",
                            Value = ""
                        });
                    }
                    while (dr.Read())
                    {
                        item.Add(new SelectListItem
                        {
                            Text = dr["JENISKELAMIN"].ToString(),
                            Value = dr["JENISKELAMIN"].ToString()
                        });
                    }
                }

                con.Close();
            }
            return item;
        }
        
        private static List<SelectListItem> DDL_HUBNASABAH()
        {
            SqlConnection con = Common.GetConnection();
            List<SelectListItem> item = new List<SelectListItem>();
            string query = "EXEC [dbo].[SP_HUBNASABAH] ";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        item.Add(new SelectListItem
                        {
                            Text = " --Choose-- ",
                            Value = ""
                        });
                    }
                    while (dr.Read())
                    {
                        item.Add(new SelectListItem
                        {
                            Text = dr["DESCRIPTIONS"].ToString(),
                            Value = dr["DESCRIPTIONS"].ToString()
                        });
                    }
                }

                con.Close();
            }
            return item;
        }
        private static List<SelectListItem> DDL_KODE_CABANG_BMRI()
        {
            SqlConnection con = Common.GetConnection();
            List<SelectListItem> item = new List<SelectListItem>();
            string query = "EXEC [dbo].[sp_GET_KODE_CABANG_BMRI] ";

            using (SqlCommand cmd = new SqlCommand(query))
            {
                cmd.Connection = con;
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.HasRows)
                    {
                        item.Add(new SelectListItem
                        {
                            Text = " --Choose-- ",
                            Value = ""
                        });
                    }
                    while (dr.Read())
                    {
                        item.Add(new SelectListItem
                        {
                            Text = dr["REFID"].ToString(),
                            Value = dr["REFID"].ToString()
                        });
                    }
                }

                con.Close();
            }
            return item;
        }
        public string GetDateInYYYYMMDD(string dt)
        {
            if (dt == "1900-01-01")
            {
                return dt;
            }
            string[] stringSeparators = new string[] { "/" };
            string[] str = dt.Split('/');

            string tempdt = string.Empty;
            for (int i = 2; i >= 0; i += -1)
                tempdt += str[i] + "-";
            tempdt = tempdt.Substring(0, 10);
            return tempdt;
        }

        public JsonResult getMST_MAPPING_BMRI(string Kode)
        {
            PagedList<ASMIKModels> model = new PagedList<ASMIKModels>();
            DataTable dt = Common.ExecuteQuery("SP_MAPPING_BMRI '" + Kode + "'");
            if (dt.Rows.Count > 0)
            {
                var record = dt;
                var result = new
                {
                    Nama_Cabang_BMRI = dt.Rows[0]["name"].ToString(),
                };
                

                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public FileResult GetReport()
        {
            string pdfPath = "\\\\wrccare03uat\\CERTIFICATE_2019\\AP2019112100600A0000502.pdf";
            byte[] FileBytes = System.IO.File.ReadAllBytes(pdfPath);
            return File(FileBytes, "application/pdf");
        }
    }
}