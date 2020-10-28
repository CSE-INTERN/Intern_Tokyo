<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="DVD_Rental.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="Label1" runat="server" Text="ユーザーID"></asp:Label><br />
            <asp:TextBox ID="user_id" runat="server" MaxLength="15"></asp:TextBox><br />
            <asp:Label ID="Label2" runat="server" Text="パスワード"></asp:Label><br />
            <asp:TextBox ID="passwd" runat="server" TextMode="Password" MaxLength="25"></asp:TextBox><br />
            <asp:Button ID="Button1" runat="server" Text="ログイン" OnClick="Button1_Click" />
        </div>
    </form>

    <script>
        if (<%=Session["id_err_flag"] %>) {
            document.getElementById("Label1").style.color = "#ff0000";
        }
        else {
            document.getElementById("Label1").style.color = "#000000";
        }

        if (<%=Session["pw_err_flag"] %>) {
            document.getElementById("Label2").style.color = "#ff0000";
        }
        else {
            document.getElementById("Label2").style.color = "#000000";
        }
    </script>
</body>
</html>
