using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlServerCe;
using System.Data.SqlClient;
using System.Configuration;

namespace ToGoOrders
{
    public partial class Form1 : Form
    {
        public static string id, name, phone, creditCard;
        DBAccess objDBAccess = new DBAccess();
        DataTable dtUsers = new DataTable();

        public Form1()
        {
            InitializeComponent();
        }

        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.CustomersConnectionString);

        private void Form1_Load(object sender, EventArgs e)
        {
            SqlCeCommand cmd = new SqlCeCommand("SELECT Phone FROM Test", con);
            con.Open();
            SqlCeDataReader dr = cmd.ExecuteReader();
            AutoCompleteStringCollection myCollection = new AutoCompleteStringCollection();

            while (dr.Read())
            {
                myCollection.Add(dr.GetString(0));
            }
            textBox2.AutoCompleteCustomSource = myCollection;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string custName = textBox1.Text;
            string custPhone = textBox2.Text;
            string custCC = textBox3.Text;
            bool exist = false;

            string query = "SELECT * FROM Users WHERE Phone = '" + custPhone + "'";
            objDBAccess.readDatathroughAdapter(query, dtUsers);
            if (dtUsers.Rows.Count == 1)
            {
                exist = true;
            }
            if (custName.Equals(""))
            {
                MessageBox.Show("Invalid customer name!");
            }
            else if (custPhone.Equals(""))
            {
                MessageBox.Show("Customer phone can't be blank!");
            }
            else if(exist == false)
            {
                SqlCommand insertCommand = new SqlCommand(" INSERT INTO Users(Name, Phone, CC) VALUES (@custName, @custPhone, @custCC)");

                insertCommand.Parameters.AddWithValue("@custName", custName);
                insertCommand.Parameters.AddWithValue("@custPhone", custPhone);
                insertCommand.Parameters.AddWithValue("@custCC", custCC);
                int row = objDBAccess.executeQuery(insertCommand);

                if (row == 1)
                {
                    MessageBox.Show("Account Created Succesfully");
                    Form1 form = new Form1();
                    form.Show();
                    //this.Hide();
                    //HomePage home = new HomePage();
                    //home.Show();
                }
                else
                {
                    MessageBox.Show("Error occured. Try Again");
                }
            } 
            else if(exist == true)
            {
                MessageBox.Show("Phone number already entered");

            }

        }


        private void textBox1_Click(object sender, EventArgs e)
        {

            //MessageBox.Show("Enter name");
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Verify that the pressed key isn't CTRL or any non-numeric digit
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // If you want, you can allow decimal (float) numbers
            //if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            //{
            //    e.Handled = true;
            //}

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            string sVal = textBox2.Text;
            if (!string.IsNullOrEmpty(sVal) && e.KeyCode != Keys.Back)
            {
                sVal = sVal.Replace("-", "");
                string newSt = Regex.Replace(sVal, ". {3}", "$0-");
                //newSt = Regex.Replace(sVal, ".{6}", "$0-");
                textBox2.Text = newSt;
                textBox2.SelectionStart = textBox2.Text.Length;
            }
        }


        public static string formatPhoneNumber(string phoneNum, string phoneFormat)
        {

            if (phoneFormat == "")
            {
                // If phone format is empty, code will use default format (###) ###-####
                phoneFormat = "(###) ###-####";
            }

            // First, remove everything except of numbers
            Regex regexObj = new Regex(@"[^\d]");
            phoneNum = regexObj.Replace(phoneNum, "");

            // Second, format numbers to phone string
            if (phoneNum.Length > 0)
            {
                phoneNum = Convert.ToInt64(phoneNum).ToString(phoneFormat);
            }

            return phoneNum;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //search for a customer and match it
            //in case there is not customer with the phone# entered a message needs to be displayed e.g."no customer was found"
            string userPhone = textBox2.Text;

            if (userPhone.Equals(""))
            {
                MessageBox.Show("Phone field cannot be empty!");
            }
            else
            {
                string query = "SELECT * FROM Users WHERE Phone = '" + userPhone + "'";
                objDBAccess.readDatathroughAdapter(query, dtUsers);
                if (dtUsers.Rows.Count == 1)
                {
                    //the name field should give the option to store more names for the same ph#
                    id = dtUsers.Rows[0]["ID"].ToString();
                    name = dtUsers.Rows[0]["Name"].ToString();
                    creditCard = dtUsers.Rows[0]["CC"].ToString();
                    textBox1.Text = name;
                    textBox3.Text = creditCard;
                }
                else if (dtUsers.Rows.Count == 0)
                {
                    MessageBox.Show("No customer was found");
                    textBox2.Text = "";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string newName = textBox1.Text;
            string newPhone = textBox2.Text;
            string newCreditCard = textBox3.Text;
            if (newName.Equals("") && newPhone.Equals(""))
            {
                MessageBox.Show("No customer was entered");
            }
            else if (newPhone.Equals(""))
            {
                MessageBox.Show("Phone field can not be blank");
            }
            else if (newName.Equals(""))
            {
                MessageBox.Show("Name field can not be blank");
            }
            else
            {
                string query = "Update Users SET Name = '" + @newName + "',Phone = '" + @newPhone + "' WHERE ID = '" + Form1.id + "' ";
                SqlCommand updateCommand = new SqlCommand(query);

                updateCommand.Parameters.AddWithValue("@newName", newName);
                updateCommand.Parameters.AddWithValue("@newPhone", newPhone);

                int row = objDBAccess.executeQuery(updateCommand);

                if (row == 1)
                {
                    MessageBox.Show("Account Modified Succesfully");
                }
                else
                {
                    MessageBox.Show("Error occured. Try Again");
                }

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure?", "Delete user", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if(dialog == DialogResult.Yes){
                string query = "DELETE FROM Users WHERE ID   = '" + Form1.id + "'";
                SqlCommand deleteCommand = new SqlCommand(query);
                int row = objDBAccess.executeQuery(deleteCommand);

                if (row == 1)
                {
                    MessageBox.Show("Customer account deleted succesfully");
                }
                else
                {
                    MessageBox.Show("Error occured. Try Again");
                }
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" || textBox2.Text != "")
            {
                CreditCardForm CCForm = new CreditCardForm();
                CCForm.Show();
            }
            else if (textBox1.Text == "")
            {
                MessageBox.Show("Name can not be empty");
            }
            else if (textBox2.Text == "")
            {
                MessageBox.Show("Phone can not be empty");
            }
        }

    }
}
