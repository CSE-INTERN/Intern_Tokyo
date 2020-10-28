using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Drawing;
using DVD_Rental.sasaki_masayuki._0_common;
using System.Windows.Forms;

namespace Rental_Form
{
    public partial class RentalForm : System.Web.UI.Page
    {

        private static SqlConnection sqlConnection;
        private static SqlDataReader sqlDataReader;

        protected void Page_Load(object sender, EventArgs e)
        {
            // ログイン関連処理
            if (Request.Cookies["login"] != null)
            {
                if (Request.Cookies["login"].Value != "")
                {
                    if (Session[Request.Cookies["login"].Value] != null)
                    {
                        if (Session[Request.Cookies["login"].Value].ToString() == "1")
                        {
                            Response.Redirect("/sasaki_masayuki/100_regression_management/regression_management.aspx");
                        }
                    }
                    else
                    {
                        Session[Request.Cookies["login"].Value] = null;
                        Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("./login.aspx");
                    }
                }
                else
                {
                    Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                    Response.Redirect("./login.aspx");
                }
            }
            else
            {
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("./login.aspx");
            }

            Label1.Text = "";
            //確認画面からのエラーがあれば表示する
            if (Session["confirmation_error"] != null)
            {
                Label1.Text = Session["confirmation_error"].ToString();
            }

            string connectionString = null;
            C_Sasaki_Common.Generate_A_Strin_To_Connect_To_The_SQL(ref connectionString);
            sqlConnection = new SqlConnection(connectionString);

            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();

            sqlCommand.CommandText = "Select * From [DVD]";

            sqlDataReader = sqlCommand.ExecuteReader();


            sqlCommand.Dispose();
            List<string> Stock_Quantity_str = new List<string>();
            List<int> Stock_Quantity = new List<int>();

            C_Sasaki_Common.Select_SQL("Select * From [dbo].[Stock] ", "Quantity", Stock_Quantity_str);

            // ストックの在庫をint型に変換
            for (int i = 0; i < Stock_Quantity_str.Count; i++)
            {
                Stock_Quantity.Add(int.Parse(Stock_Quantity_str[i].ToString()));

            }

            if (!IsPostBack)
            {

                CheckBoxList1.Items.Clear();
                // DVDを表示
                while (sqlDataReader.Read())
                {

                    string str = sqlDataReader["Name"].ToString();
                    CheckBoxList1.Items.Add(str);

                }

            }

            // 在庫の数が０より少なかったらチェックできなくする
            for (int i = 0; i < Stock_Quantity.Count; i++)
            {
                if (Stock_Quantity[i] <= 0)
                {
                    CheckBoxList1.Items[i].Enabled = false;
                }
            }

            // Stockにそもそもないからチェック不可にする
            CheckBoxList1.Items[9].Enabled = false;
            CheckBoxList1.Items[11].Enabled = false;


            sqlDataReader.Close();
            sqlConnection.Close();
            sqlConnection.Dispose();

        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            // テキストボックスに入力されているメンバーIDを取得
            string MemberIDText = GetMemberId_TextBox();
            List<string> MemberID_Check = new List<string>();
            bool ID_Check = false;
            C_Sasaki_Common.Select_SQL("Select * From [dbo].[Member] ", "Id", MemberID_Check)
                ;
            for(int i = 0;i < MemberID_Check.Count;i++)
            {            
                // 入力されたメンバーIDとDB内のメンバーIDを比較
                if (MemberID_Check[i] == MemberIDText)
                {
                    ID_Check = true;
                    break;
                }

            }

            // テキストボックス内に入力されているかつDB内IDと一致していれば次ページに進む
            if (MemberIDText != "" && ID_Check == true)
            {
                Label1.Text = "";
                Label2.ForeColor = Color.Black;
                Session.Remove("confirmation_error");

                int count = 1;                // DVDIDカウント

                // チェックが入っているものをSessionに入れる（力技）
                foreach (ListItem li in CheckBoxList1.Items)
                {
                    // チェックが付いているものにデータを入れる
                    if (li.Selected)
                    {
                        switch (count)
                        {

                            case 1:
                                Session["DVD1"] = li.Text;
                                break;
                            case 2:
                                Session["DVD2"] = li.Text;
                                break;
                            case 3:
                                Session["DVD3"] = li.Text;
                                break;
                            case 4:
                                Session["DVD4"] = li.Text;
                                break;
                            case 5:
                                Session["DVD5"] = li.Text;
                                break;
                            case 6:
                                Session["DVD6"] = li.Text;
                                break;
                            case 7:
                                Session["DVD7"] = li.Text;
                                break;
                            case 8:
                                Session["DVD8"] = li.Text;
                                break;
                            case 9:
                                Session["DVD9"] = li.Text;
                                break;
                            case 10:
                                Session["DVD10"] = li.Text;
                                break;
                            case 11:
                                Session["DVD11"] = li.Text;
                                break;
                            case 12:
                                Session["DVD12"] = li.Text;
                                break;

                        }
                    }
                    // チェックがついていないものにはなにもいれない
                    else
                    {
                        switch (count)
                        {
                            case 1:
                                Session.Remove("DVD1");
                                break;
                            case 2:
                                Session.Remove("DVD2");
                                break;
                            case 3:
                                Session.Remove("DVD3");
                                break;
                            case 4:
                                Session.Remove("DVD4");
                                break;
                            case 5:
                                Session.Remove("DVD5");
                                break;
                            case 6:
                                Session.Remove("DVD6");
                                break;
                            case 7:
                                Session.Remove("DVD7");
                                break;
                            case 8:
                                Session.Remove("DVD8");
                                break;
                            case 9:
                                Session.Remove("DVD9");
                                break;
                            case 10:
                                Session.Remove("DVD10");
                                break;
                            case 11:
                                Session.Remove("DVD11");
                                break;
                            case 12:
                                Session.Remove("DVD12");
                                break;

                        }
                    }

                    count++;
                }


                List<string> Dvdid = new List<string>();
                for (int i = 1; i <= 12; i++)
                {
                    if (Session["DVD" + i.ToString()] != null)
                    {
                        if (Session["DVD" + i.ToString()].ToString() != "")
                        {

                            Dvdid.Add(C_Sasaki_Common.Convert_DVD_Name_To_ID(Session["DVD" + i.ToString()].ToString()));

                        }
                    }

                }
                Session["dvdid"] = string.Join(",", Dvdid);

                List<string> Stock_UpdateDateTime;
                Stock_UpdateDateTime = C_Sasaki_Common.Get_Stock_Update_Date_Time_For_DVD_Id(Dvdid);
                Session["RentalForm_Stock_UpdateDateTime"] = string.Join(",", Stock_UpdateDateTime);

                if (Dvdid.Count != 0)
                {
                    Session["MemberID"] = MemberIDText;
                    Response.Redirect("RentalConfirm.aspx");
                }
                else
                {
                    Label3.ForeColor = Color.Red;
                }


            }
            else
            {
                Label2.ForeColor = Color.Red;
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            // ログアウト処理
            Session[Request.Cookies["login"].Value] = null;
            Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("Login.aspx");
        }


        // テキストボックスに入力されているテキストを取得
        // ※テキストが入力されていない場合nullを返します。
        private string GetMemberId_TextBox()
        {
            try
            {
                return TextBox1.Text;
            }
            catch (Exception ex)
            {
                C_Sasaki_Common.Catch_An_Exception(ex);
            }
            return null;
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}