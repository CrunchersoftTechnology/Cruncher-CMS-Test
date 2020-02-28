using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using CMS.Domain.Models;
using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using CMS.Web.Helpers;
using CMS.Web.Logger;
using CMS.Web.Models;
using CMS.Web.ViewModels;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Web.Hosting;

namespace CMS.Web.Controllers
{
    public class ConfigureController : Controller
    {
        string constr = ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString;

        [HttpGet]
        public ActionResult Index()
        {
            List<Configure> Configures = new List<Configure>();


            string query = "SELECT * FROM Configuration";
            SqlConnection con = new SqlConnection(constr);
            {
                SqlCommand cmd = new SqlCommand(query);
                {
                    cmd.Connection = con;
                    con.Open();
                    SqlDataReader sdr = cmd.ExecuteReader();
                    {
                        while (sdr.Read())
                        {
                            Configures.Add(new Configure
                            {
                                Id = Convert.ToInt32(sdr["Id"]),
                                name = Convert.ToString(sdr["Name"]),
                                address = Convert.ToString(sdr["address"]),
                                aboutus = Convert.ToString(sdr["aboutus"]),
                                email_id = Convert.ToString(sdr["email_id"]),
                                sender_id = Convert.ToString(sdr["sender_id"]),
                                username = Convert.ToString(sdr["username"]),
                                password = Convert.ToString(sdr["password"]),
                               // brocherfile = Convert.ToString(sdr["brocherfile"])
                            });
                        }
                    }
                    con.Close();
                }
            }

            if (Configures.Count == 0)
            {
                Configures.Add(new Configure());
            }
            return View(Configures);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Configure());
        }


        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Configure Configure)
        {

            string query = "INSERT INTO Configuration (name,aboutus,address,email_id,sender_id,username,password) VALUES(@name,@aboutus,@address,@email_id,@sender_id,@username,@password)";
            SqlConnection con = new SqlConnection(constr);
            {
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@name", Configure.name);
                cmd.Parameters.AddWithValue("@aboutus", Configure.aboutus);
                cmd.Parameters.AddWithValue("@address", Configure.address);
                cmd.Parameters.AddWithValue("@email_id", Configure.email_id);
                cmd.Parameters.AddWithValue("@sender_id", Configure.sender_id);
                cmd.Parameters.AddWithValue("@username", Configure.username);
                cmd.Parameters.AddWithValue("@password", Configure.password);
               // cmd.Parameters.AddWithValue("@brocherfile", Configure.brocherfile);
                cmd.Connection = con;
                con.Open();
                Configure.Id = Convert.ToInt32(cmd.ExecuteScalar());
                con.Close();

            }
            return RedirectToAction("Index");
        }


        // GET: Configure/Edit/5
        //[HttpGet]
        [ValidateInput(false)]
        public ActionResult Edit(int id)
        {

            Configure conf = new Configure();
            DataTable dt = new DataTable();
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString))
            {
                sqlcon.Open();
                string query = "select * from Configuration where Id=@Id";

                SqlDataAdapter sqlda = new SqlDataAdapter(query, sqlcon);
                sqlda.SelectCommand.Parameters.AddWithValue("@Id", id);
                sqlda.Fill(dt);
            }
            if (dt.Rows.Count == 1)
            {
                conf.Id = Convert.ToInt32(dt.Rows[0][0].ToString());
                conf.name = (dt.Rows[0][1].ToString());
                conf.aboutus = (dt.Rows[0][2].ToString());
                conf.address = (dt.Rows[0][3].ToString());
                conf.email_id = (dt.Rows[0][4].ToString());
                conf.sender_id = (dt.Rows[0][5].ToString());
                conf.username = (dt.Rows[0][6].ToString());
                conf.password = (dt.Rows[0][7].ToString());
              //  conf.brocherfile = (dt.Rows[0][8].ToString());

                return View(conf);
            }
            else
            {
                return RedirectToAction("Index");
            }

            // return RedirectToAction("Index");
        }

        // POST: Configure/Edit/5
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Configure config)
        {
            try
            {
                SqlConnection sqlcon = new SqlConnection(ConfigurationManager.ConnectionStrings["CMSWebConnection"].ConnectionString);

                sqlcon.Open();
                string query = "update Configuration set name=@name, aboutus=@aboutus, address=@address,email_id=@email_id, sender_id=@sender_id, username=@username,password=@password where Id=@Id";
                SqlCommand sqlcmd = new SqlCommand(query, sqlcon);
                sqlcmd.Parameters.AddWithValue("@Id", config.Id);
                sqlcmd.Parameters.AddWithValue("@name", config.name);
                sqlcmd.Parameters.AddWithValue("@aboutus", config.aboutus);
                sqlcmd.Parameters.AddWithValue("@address", config.address);
                sqlcmd.Parameters.AddWithValue("@email_id", config.email_id);
                sqlcmd.Parameters.AddWithValue("@sender_id", config.sender_id);
                sqlcmd.Parameters.AddWithValue("@username", config.username);
                sqlcmd.Parameters.AddWithValue("@password", config.password);
              //  sqlcmd.Parameters.AddWithValue("@brocherfile", config.brocherfile);
                sqlcmd.ExecuteNonQuery();


                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return View();
            }
        }



        // GET: Configure/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }



        // POST: Configure/Delete/5
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
