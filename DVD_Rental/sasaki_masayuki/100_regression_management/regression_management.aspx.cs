using DVD_Rental.sasaki_masayuki._0_common;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;



namespace DVD_Rental.sasaki_masayuki._100_regression_management
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        int m_member_id;

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
                            Response.Redirect("./../../RentalForm.aspx");
                        }
                    }
                    else
                    {
                        Session[Request.Cookies["login"].Value] = null;
                        Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                        Response.Redirect("./../../login.aspx");
                    }
                }
                else
                {
                    Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                    Response.Redirect("./../../login.aspx");
                }
            }
            else
            {
                Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
                Response.Redirect("./../../login.aspx");
            }

            //確認画面からのエラーがあれば表示する
            if(Session["confirmation_error"] != null)
            {
                Label3.Text = Session["confirmation_error"].ToString();
            }
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        //レンタル中の商品を表示　ボタン
        protected void Button2_Click(object sender, EventArgs e)
        {
            View_Items_Currently_Being_Rented();
            
        }

        //選択した商品を返却　ボタン
        protected void Button3_Click(object sender, EventArgs e)
        {
            if (Do_You_Have_Any_Unreturned_DVDs())
            {
                //未返却のDVDがある場合

                List<int> unreturned_id = new List<int>();
                string str_unreturned_id = Session["unreturned_id"].ToString();
                //Session.Remove("unreturned_id");
                string[] arr = str_unreturned_id.Split(',');

                //選択されたdvdのIDを格納します。重複したIDが入っている可能性がありますが正常です。
                List<string> selected_dvd_id = new List<string>();
                List<string> selected_id = new List<string>();
                int dvd_num = CheckBoxList1.Items.Count;
                for (int i = 0; i < dvd_num; i++)
                {
                    if (CheckBoxList1.Items[i].Selected)
                    {
                        //選択されていたら

                        //選択されたレンタルidを抽出
                        selected_id.Add(arr[i]);

                        //選択されたdvdの名前をidに変換します
                        string name_to_id = C_Sasaki_Common.Convert_DVD_Name_To_ID(CheckBoxList1.Items[i].Value);

                        //格納。
                        selected_dvd_id.Add(name_to_id);



                    }
                }
                //1つ以上選択されたDVDがあるか
                if (selected_dvd_id.Count != 0)
                {
                    Label3.Text = "";
                    Session.Remove("confirmation_error");

                    List<string> stock_update_date_time_storehouse;
                    //dvd_idからUpdate_Date_Timeを求める
                    stock_update_date_time_storehouse = C_Sasaki_Common.Get_Stock_Update_Date_Time_For_DVD_Id(selected_dvd_id);

                    List<string> rental_update_date_time_storehouse;
                    //レンタルDBのIdからUpdate_Date_Timeを求める
                    rental_update_date_time_storehouse = C_Sasaki_Common.Get_Rental_Update_Date_Time_For_Rental_Id(selected_id);


                    //返却管理画面で返却を押した時のUpdateDateTimeを格納。
                    Session["management_time_stock_update_datetime"] = string.Join(",", stock_update_date_time_storehouse);
                    Session["management_time_rental_update_datetime"] = string.Join(",", rental_update_date_time_storehouse);

                    Session["Member_ID"] = m_member_id;
                    Session["Select_DVD_ID"] = string.Join(",", selected_dvd_id);
                    Session["Selected_ID"] = string.Join(",", selected_id);
                    Response.Redirect("/sasaki_masayuki/101_return_confirmation/return_confirmation.aspx");
                }
            }
        }
        protected void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        //テキストボックスに入力されているテキストを取得
        //※テキストが入力されていない場合nullを返します。
        private string Get_MemberId_From_TextBox()
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

        //レンタル中の商品を表示
        private void View_Items_Currently_Being_Rented()
        {
            //入力されたテキストを取得
            string member_id_storehouse = Get_MemberId_From_TextBox();
            if (member_id_storehouse != "")
            {
                //テキストが入力されていたら


                m_member_id = int.Parse(Get_MemberId_From_TextBox());
                //チェックボックスのクリア
                CheckBoxList1.Items.Clear();

                //抽出したdvdidデータの格納先
                List<string> dvdid_storehouse = C_Sasaki_Common.Extract_The_Unreturned_DVD_IDs_Of_The_Entered_Member_ID(member_id_storehouse);

                List<string> unreturned_id = new List<string>();
                //レンタル中の商品のIdを抽出
                C_Sasaki_Common.Select_SQL("Select * From [dbo].[Rental] Where " + member_id_storehouse + " = MemberId AND IsReturned = 0", "Id", unreturned_id);
                Session["unreturned_id"] = string.Join(",", unreturned_id);

                int dvd_id_num = dvdid_storehouse.Count();
                for (int i = 0; i < dvd_id_num; i++)
                {
                    //抽出したDVDの名前の格納先
                    string dvd_name_storehouse = null;

                    //dvdidから名前に変換
                    dvd_name_storehouse = C_Sasaki_Common.Extract_The_Name_Of_The_DVD_That_Matches_The_DVD_ID(dvdid_storehouse.ElementAt(i));


                    CheckBoxList1.Items.Add(dvd_name_storehouse);
                }

            }
            else
            {
                //テキストが入力されていない場合

            }
        }

        //未返却のDVDがあるか
        //true:ある false:ない
        private bool Do_You_Have_Any_Unreturned_DVDs()
        {
            if(CheckBoxList1.Items.Count != 0)
            {
                //チェックボックスの中身が0じゃなければ
                return true;
            }
            return false;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            //ログアウト処理
            Session[Request.Cookies["login"].Value] = null;
            Response.Cookies["login"].Expires = DateTime.Now.AddDays(-1);
            Response.Redirect("./../../login.aspx");
        }

        
    }
}