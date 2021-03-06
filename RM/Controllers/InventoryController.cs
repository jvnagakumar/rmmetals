﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RM.Models;
using System.IO;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using PagedList.Mvc;
using PagedList;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Globalization;

namespace RM.Controllers
{
    public class InventoryController : Controller
    {
        // GET: Product
        [Authorize]
        public ActionResult Index(int? page, int? Id, string sortOrder, string Pages, Inventory pro)
        {
            string submit = Request["submit"];
            ViewBag.Pages = Pages;
            ViewBag.LocSortParm = String.IsNullOrEmpty(sortOrder) ? "Loc_desc" : "";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "Type_desc" : "Type";
            ViewBag.FinishSortParm = sortOrder == "Finish" ? "Finish_desc" : "Finish";
            ViewBag.GaugeSortParm = sortOrder == "Gauge" ? "Gauge_desc" : "Gauge";
            ViewBag.WidthSortParm = sortOrder == "Width" ? "Width_desc" : "Width";
            ViewBag.WTNETSortParm = sortOrder == "WTNET" ? "WTNET_desc" : "WTNET";
            ViewBag.NOOFPCSSortParm = sortOrder == "NOOFPCS" ? "NOOFPCS_desc" : "NOOFPCS";

            pro.ProductList = pro.ProductsList().ToList();

            pro.ProductList = pro.ProductList.Where(a => a.Loc.ToUpper().Contains(string.IsNullOrEmpty(pro.Loc) ? a.Loc : pro.Loc.ToUpper())).
                Where(a => a.Type.ToUpper().Contains(string.IsNullOrEmpty(pro.Type) ? a.Type : pro.Type.ToUpper())).
                Where(a => a.Finish.ToUpper().Contains(string.IsNullOrEmpty(pro.Finish) ? a.Finish : pro.Finish.ToUpper())).
                Where(a => a.Gauge.ToUpper().Contains(string.IsNullOrEmpty(pro.Gauge) ? a.Gauge : pro.Gauge.ToUpper())).
                Where(a => a.Width.ToUpper().Contains(string.IsNullOrEmpty(pro.Width) ? a.Width : (pro.Width.ToUpper()))).
                Where(a => a.WTNET.ToUpper().Contains(string.IsNullOrEmpty(pro.WTNET) ? a.WTNET : (pro.WTNET.ToUpper()))).ToList();

            if (Id.HasValue)
            {
                RequestedQuote quote = new RequestedQuote();
                quote = quote.GetData(User.Identity.GetUserId());
                quote.User_Id = User.Identity.GetUserId();
                quote.product = new Inventory();
                quote.product = quote.product.productDetails(Id.Value);
                quote.insert(quote);

                var smtpClient = new SmtpClient();
                var message = new MailMessage("no-reply@suteki.co.uk", "admin@gmail.com")
                {
                    Subject = "Requested Quote" + quote.product.Loc,
                    Body = "User Name " + User.Identity.GetUserName() + Environment.NewLine + "Phone Nubmer :" + quote.PhoneNumber + Environment.NewLine + "Ip Adderss:" + Request.ServerVariables["REMOTE_ADDR"]

                };
                smtpClient.Send(message);

            }

            switch (sortOrder)
            {
                case "Loc_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.Loc).ToList();
                    break;
                case "Finish":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.Finish).ToList();
                    break;
                case "Finish_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.Finish).ToList();
                    break;
                case "Gauge":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.Gauge).ToList();
                    break;
                case "Gauge_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.Gauge).ToList();
                    break;
                case "Type":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.Type).ToList();
                    break;
                case "Type_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.Type).ToList();
                    break;
                case "Width":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.Width).ToList();
                    break;
                case "Width_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.Width).ToList();
                    break;

                case "WTNET":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.WTNET).ToList();
                    break;
                case "WTNET_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.WTNET).ToList();
                    break;

                case "NOOFPCS":
                    pro.ProductList = pro.ProductList.OrderBy(s => s.NOOFPCS).ToList();
                    break;
                case "NOOFPCS_desc":
                    pro.ProductList = pro.ProductList.OrderByDescending(s => s.NOOFPCS).ToList();
                    break;
                default:
                    pro.ProductList = pro.ProductList.OrderBy(s => s.Loc).ToList();
                    break;
            }

            List<SelectListItem> listitems = new List<SelectListItem>();

            listitems.Add(new SelectListItem { Text = "10 items ", Value = "10" });
            listitems.Add(new SelectListItem { Text = "20 items ", Value = "20" });
            listitems.Add(new SelectListItem { Text = "30 items ", Value = "30" });
            listitems.Add(new SelectListItem { Text = "40 items ", Value = "40" });
            ViewBag.ListItems = listitems;
            if (string.IsNullOrEmpty(Pages))
            {
                pro.IPagedProductsList = pro.ProductList.ToPagedList(page ?? 1, 10);
                return View(pro);
            }
            else
            {
                pro.IPagedProductsList = pro.ProductList.ToPagedList(page ?? 1, Convert.ToInt32(Pages));
                return View(pro);
            }
        }
        [HttpPost]
        public ActionResult Index(int? page,  string Pages, Inventory pro)
        {
            string submit = Request["submit"];
            Dictionary<int, string> searchData = new Dictionary<int, string>();

           

            if (!(string.IsNullOrEmpty(pro.Loc)))
            {
                searchData.Add(1, pro.Loc);
            }
            if (!(string.IsNullOrEmpty(pro.Type)))
            {
                searchData.Add(2, pro.Type);
            }
            if (!(string.IsNullOrEmpty(pro.Finish)))
            {
                searchData.Add(3, pro.Finish);
            }
            if (!(string.IsNullOrEmpty(pro.Gauge)))
            {
                searchData.Add(4, pro.Gauge);
            }
            if (!(string.IsNullOrEmpty(pro.Width)))
            {
                searchData.Add(5, pro.Width);
            }
            if (!(string.IsNullOrEmpty(pro.WTNET)))
            {
                searchData.Add(6, pro.WTNET);
            }
            if (!(string.IsNullOrEmpty(submit)))
            {
                SearchedData search = new SearchedData();
                search.Filerdata = new FilterData();
                search.Filerdata = search.Filerdata.GetData(User.Identity.GetUserId());
                search.Filerdata.IPAddress = Request.ServerVariables["REMOTE_ADDR"];
                search.Details = searchData;
                search.Filerdata.user_Id = User.Identity.GetUserId();
                search.insert(search);
            }

            pro.ProductList = pro.ProductsList().ToList();

            pro.ProductList = pro.ProductList.Where(a => a.Loc.ToUpper().Contains(string.IsNullOrEmpty(pro.Loc) ? a.Loc : pro.Loc.ToUpper())).
                Where(a => a.Type.ToUpper().Contains(string.IsNullOrEmpty(pro.Type) ? a.Type : pro.Type.ToUpper())).
                Where(a => a.Finish.ToUpper().Contains(string.IsNullOrEmpty(pro.Finish) ? a.Finish : pro.Finish.ToUpper())).
                Where(a => a.Gauge.ToUpper().Contains(string.IsNullOrEmpty(pro.Gauge) ? a.Gauge : pro.Gauge.ToUpper())).
                Where(a => a.Width.ToUpper().Contains(string.IsNullOrEmpty(pro.Width) ? a.Width : (pro.Width.ToUpper()))).
                Where(a => a.WTNET.ToUpper().Contains(string.IsNullOrEmpty(pro.WTNET) ? a.WTNET : (pro.WTNET.ToUpper()))).ToList();


            List<SelectListItem> listitems = new List<SelectListItem>();

            listitems.Add(new SelectListItem { Text = "10 items ", Value = "10" });
            listitems.Add(new SelectListItem { Text = "20 items ", Value = "20" });
            listitems.Add(new SelectListItem { Text = "30 items ", Value = "30" });
            listitems.Add(new SelectListItem { Text = "40 items ", Value = "40" });


            ViewBag.ListItems = listitems;


            if (string.IsNullOrEmpty(Pages))
            {

                pro.IPagedProductsList = pro.ProductList.ToPagedList(page ?? 1, 10);
                return View(pro);
            }
            else
            {
                pro.IPagedProductsList = pro.ProductList.ToPagedList(page ?? 1, Convert.ToInt32(Pages));
                return View(pro);
            }
        }
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            if (Request.Files["file"].ContentLength > 0)
            {
                string fileExtension =
                                     System.IO.Path.GetExtension(Request.Files["file"].FileName);

                if (fileExtension == ".xls" || fileExtension == ".xlsx")
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {

                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    string excelConnectionString = string.Empty;
                    excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                    fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    //connection String for xls file format.
                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" +
                        fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }

                    //                    Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    DataTable dt = new DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }

                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}] ", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }
                }
                if (fileExtension.ToString().ToLower().Equals(".xml"))
                {
                    string fileLocation = Server.MapPath("~/Content/") + Request.Files["FileUpload"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }

                    Request.Files["FileUpload"].SaveAs(fileLocation);
                    XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                    // DataSet ds = new DataSet();
                    ds.ReadXml(xmlreader);
                    xmlreader.Close();
                }
                string conn1 = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                MySqlConnection con1 = new MySqlConnection(conn1);

                string query1 = "truncate table products";
                MySqlCommand cmd1 = new MySqlCommand(query1, con1);
                con1.Open();
                cmd1.ExecuteNonQuery();

                for (int i = 0; i < (ds.Tables[0].Rows.Count) - 1; i++)
                {
                    string conn = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
                    MySqlConnection con = new MySqlConnection(conn);
                    string query = "Insert into products(Loc,Type,Finish,Gauge,Width,WTNET,NOOFPCS) Values('" +
                    ds.Tables[0].Rows[i][0].ToString() + "','" + ds.Tables[0].Rows[i][1].ToString() +
                    "','" + ds.Tables[0].Rows[i][2].ToString() + "','" + ds.Tables[0].Rows[i][3].ToString() + "','" + ds.Tables[0].Rows[i][4].ToString() + "','" + ds.Tables[0].Rows[i][5].ToString() + "','" + ds.Tables[0].Rows[i][6].ToString() + "')";
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }
            return RedirectToAction("Index");
        }



        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
