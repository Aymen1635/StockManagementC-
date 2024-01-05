using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GestionDeStock
{
    public partial class ManageProducts : Form
    {
        SQLiteConnection sqliteCon = Cnx.CreateConnection();
        public ManageProducts()
        {
            InitializeComponent();
        }

        private void ManageProducts_Load(object sender, EventArgs e)
        {

            //Fetching Categories
            SQLiteDataReader dr = Cnx.getData(sqliteCon, "select * from categories");
            // Determine New ID for Products
            SQLiteDataReader maxId = Cnx.getData(sqliteCon, "select MAX(id) from produit");

            // Populate ComboBox with Category Names
            while (dr.Read())
            {
                // category names are in the second column which is :1
                this.comboBox1.Items.Add(dr.GetString(1));
            }

            // Populate DataGridView with Product Data
            Cnx.populateTable(sqliteCon, "select * from produit", this.dataGridView1);
            if (maxId.Read())
            {
                int newId = maxId.GetInt32(0) + 1;
                // Display the new ID in a text control
                this.ID.Text = newId.ToString();
            }

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            // Get the selected item from the ComboBox
            string selectedItem = this.comboBox1.SelectedItem.ToString();
            // Fetch category information based on the selected item
            SQLiteDataReader dr2 = Cnx.getData(sqliteCon, "select * from categories where nom_cat='" + selectedItem + "'");

            // Check if data was retrieved
            if (dr2.Read())
            {
                // Display the category ID
                this.catID.Text = dr2.GetInt32(0).ToString();
            }
        }

        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Iterate through selected rows in the DataGridView
            foreach (DataGridViewRow dvr in dataGridView1.SelectedRows)
            {
                // Populate text controls with data from the selected row
                this.ID.Text = dvr.Cells[0].Value.ToString();
                this.nom_du_produit.Text = dvr.Cells[1].Value.ToString();
                this.prix.Text = dvr.Cells[2].Value.ToString();
                this.description.Text = dvr.Cells[4].Value.ToString();

                // Extract category ID from the selected row
                int selectedCatId = int.Parse(dvr.Cells[3].Value.ToString());

                // Fetch category information based on the extracted category ID
                SQLiteDataReader dr2 = Cnx.getData(sqliteCon, "select * from categories where id=" + selectedCatId + "");
                
                // Check if data was retrieved from the category table
                if (dr2.Read())
                {
                    this.comboBox1.SelectedItem = dr2.GetString(1);
                }
            }


        }
        //add product method
        private void button1_Click(object sender, EventArgs e)
        {
            int catId = int.Parse(catID.Text);
            int productId = int.Parse(ID.Text);
            string productName = this.nom_du_produit.Text.Trim();
            double prix = double.Parse(this.prix.Text);
            string description = this.description.Text.Trim();

            string query = "insert into produit values(" + productId + ",'" + productName + "'," + prix + " ," + catId + " ,'" + description + "')";

            Cnx.InsertData(sqliteCon, query);
            Cnx.populateTable(sqliteCon, "select * from produit", this.dataGridView1);

            SQLiteDataReader maxId = Cnx.getData(sqliteCon, "select MAX(id) from produit");
            if (maxId.Read())
            {
                int newId = maxId.GetInt32(0) + 1;
                this.ID.Text = newId.ToString();
            }


            this.ID.Text = "";
            this.nom_du_produit.Text = "";
            this.prix.Text = "";
            this.description.Text = "";

            MessageBox.Show("Product Successfully Added");

        }

        //update product method
        private void button2_Click(object sender, EventArgs e)
        {
            // Extract values from controls
            int catId = int.Parse(catID.Text);
            int productId = int.Parse(ID.Text);
            string productName = this.nom_du_produit.Text.Trim();
            double prix = double.Parse(this.prix.Text);
            string description = this.description.Text.Trim();

            //prepare the sql query
            string query = "update produit set nom_produit='" + productName + "', prix=" + prix + " ,categories=" + catId + " ,description='" + description + "' where id=" + productId + "";
           
            // Update data in the SQLite database
            Cnx.InsertData(sqliteCon, query);

            // Refresh the DataGridView with the updated data
            Cnx.populateTable(sqliteCon, "select * from produit", this.dataGridView1);

            // Get the new maximum ID after the update
            SQLiteDataReader maxId = Cnx.getData(sqliteCon, "select MAX(id) from produit");
            if (maxId.Read())
            {
                int newId = maxId.GetInt32(0) + 1;
                this.ID.Text = newId.ToString();
            }

            // Clear the input fields after the update
            this.ID.Text = "";
            this.nom_du_produit.Text = "";
            this.prix.Text = "";
            this.description.Text = "";

            MessageBox.Show("Product Successfully Updated");
        }

        //delete product method
        private void button3_Click(object sender, EventArgs e)
        {
            // Extract product ID 
            int productId = int.Parse(ID.Text);

            //prepare the sql query
            String query = "Delete from produit where id=" + productId + ";";

            // Delete the product from the SQLite database
            Cnx.InsertData(sqliteCon, query);
            // Refresh the DataGridView after deletion
            Cnx.populateTable(sqliteCon, "select * from produit", this.dataGridView1);

            // Clear the input fields after the delete
            this.ID.Text = "";
            this.nom_du_produit.Text = "";
            this.prix.Text = "";
            this.description.Text = "";

            MessageBox.Show("Product Successfully Deleted");
        }
    }
}