using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

private DataTable createBase()
{
    string connectionString = "Data Source=192.168.0.97;Initial Catalog=oktell;Persist Security Info=True;User ID=root;Password=web";
    string zapros = "SELECT id, Phone, Dolznost FROM BaseSluzbaControl ORDER BY Dolznost";
    DataTable dt = new DataTable();
    using (SqlConnection con = new SqlConnection(connectionString))
    {
        using (SqlCommand cmd = new SqlCommand(zapros))
        {
            using (SqlDataAdapter sda = new SqlDataAdapter())
            {
                cmd.Connection = con;
                sda.SelectCommand = cmd;
                sda.Fill(dt);
                GridView1.DataSource = dt;
                GridView1.DataBind();
            }
        }
    }
    return dt;
}

protected void select_OnClick_linkBtn(object sender, EventArgs e)
{
    LinkButton lnkbtn = sender as LinkButton;
    GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;

    string connectionString = "Data Source=192.168.0.97;Initial Catalog=oktell;Persist Security Info=True;User ID=root;Password=web";
    string zapros = $"SELECT Phone FROM BaseSluzbaControl WHERE id = '{GridView1.Rows[gvrow.RowIndex].Cells[0].Text}'";

    using (SqlConnection con = new SqlConnection(connectionString))
    {
        using (SqlCommand cmd = new SqlCommand(zapros, con))
        {
            con.Open();
            SqlDataReader sqlDataReader = cmd.ExecuteReader();
            if (sqlDataReader.Read())
            {
                phoneTB.Text = sqlDataReader["Phone"].ToString();
            }
            sqlDataReader.Close();
        }
    }
}

protected void Button2_Click(object sender, EventArgs e)
{
    GridViewRow row = (GridViewRow)((Button)sender).Parent.Parent;
    string forwardingNumber = row.Cells[1].Text; // Phone
    var chainid = HttpContext.Current.Request.QueryString["Chainid"];

    if (!string.IsNullOrEmpty(forwardingNumber))
        SetFil(forwardingNumber, chainid);
}

private void SetFil(string forwardingNumber, string Chainid)
{
    string connectionString = "Data Source=192.168.0.97;Initial Catalog=oktell;Persist Security Info=True;User ID=root;Password=web";
    string zapros = $"UPDATE vh_SluzhbaKontrolya SET Phone='{forwardingNumber}' WHERE id='{Chainid}'";

    using (SqlConnection con = new SqlConnection(connectionString))
    {
        using (SqlCommand cmd = new SqlCommand(zapros, con))
        {
            con.Open();
            cmd.ExecuteNonQuery();
        }
    }
}

protected void Button1_Click(object sender, EventArgs e)
{
    DataTable dt = createBase();

    if (!string.IsNullOrWhiteSpace(TextBox1.Text))
    {
        TextBox1.Text = TextBox1.Text.Trim();

        string searchString = TextBox1.Text.ToLower();

        var sortedRows = dt.AsEnumerable()
            .Where(row => row.Field<string>("Dolznost").ToLower().Contains(searchString))
            .OrderByDescending(row => row.Field<string>("Dolznost").Equals(searchString, StringComparison.OrdinalIgnoreCase));

        if (sortedRows.Any())
        {
            GridView1.DataSource = sortedRows.CopyToDataTable();
        }
        else
        {
            GridView1.DataSource = null;
        }

        GridView1.DataBind();
    }
    else
    {
        TextBox1.BorderColor = System.Drawing.Color.Red;
    }
}
