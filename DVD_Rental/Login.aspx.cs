using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DVD_Rental
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Cookies["login"] != null)
            {
                if (Request.Cookies["login"].Value != "")
                {
                    if (Session[Request.Cookies["login"].Value] != null)
                    {
                        if (Session[Request.Cookies["login"].Value].ToString() == "0")
                        {
                            Response.Redirect("RentalForm.aspx");
                        }
                        else if(Session[Request.Cookies["login"].Value].ToString() == "1")
                        {
                            Response.Redirect("./sasaki_masayuki/100_regression_management/regression_management.aspx");
                        }
                        else
                        {
                            Session[Request.Cookies["login"].Value] = null;
                            Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                        }
                    }
                    else
                    {
                        Session[Request.Cookies["login"].Value] = null;
                        Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                    }
                }
                else
                {
                    Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                }
            }
            else
            {
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
            }

            if (!IsPostBack)
            {
                Session["id_err_flag"] = 0;
                Session["pw_err_flag"] = 0;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //ID : 15文字 英数字
            //PW : 25文字 英数字

            //user_idの英数字判定
            if(new Regex("^[0-9a-zA-Z]+$").IsMatch(user_id.Text))
            {
                Session["id_err_flag"] = 0;

                //passwdの英数字判定
                if (new Regex("^[0-9a-zA-Z]+$").IsMatch(passwd.Text))
                {
                    //認証処理


                    //データベース
                    ConnectDB db = new ConnectDB();
                    string[] status = db.auth(user_id.Text,passwd.Text);

                    try
                    {
                        if (Convert.ToBoolean(status[0]) == true)
                        {
                            //管理者フラグ成立
                            Session.Remove("id_err_flag");
                            Session.Remove("pw_err_flag");

                            Session[status[1]] = "1";
                            Response.Cookies["login"].Value = status[1];
                            Response.Cookies["login"].Expires = DateTime.Now.AddDays(1);
                            Response.Redirect("./sasaki_masayuki/100_regression_management/regression_management.aspx");
                        }
                        else if (Convert.ToBoolean(status[0]) == false)
                        {
                            //管理者フラグ非成立
                            Session.Remove("id_err_flag");
                            Session.Remove("pw_err_flag");

                            Session[status[1]] = "0";
                            Response.Cookies["login"].Value = status[1];
                            Response.Cookies["login"].Expires = DateTime.Now.AddDays(1);
                            Response.Redirect("RentalForm.aspx");
                        }
                    }
                    catch(Exception err)
                    {
                        if (status[0] == "-1")
                        {
                            //ログインできない
                        }
                    }

                }
                else
                {
                    //パスワードのエラー
                    Session["pw_err_flag"] = 1;
                }
            }
            else
            {

                //passwdの英数字判定
                if (new Regex("^[0-9a-zA-Z]+$").IsMatch(passwd.Text))
                {
                    //IDのエラー
                    Session["pw_err_flag"] = 0;
                    Session["id_err_flag"] = 1;
                }
                else
                {
                    //IDとパスワードのエラー
                    Session["id_err_flag"] = 1;
                    Session["pw_err_flag"] = 1;
                }
            }
        }

    }
}