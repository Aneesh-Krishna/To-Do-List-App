using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;


namespace To_Do_List_App
{
    public partial class ToDoList : Form
    {
        public ToDoList()
        {
            InitializeComponent();
        }

        DataTable todoList=new DataTable();
        bool isEditing=false;

        
        private void ToDoList_Load(object sender, EventArgs e)
        {
            todoList.Columns.Add("Title");
            todoList.Columns.Add("Description");

            //Point the datagridview to the datasource
            ToDoListView.DataSource = todoList;
            LoadDataFromDatabase();
        }
        private void NewButton_Click(object sender, EventArgs e)
        {
            TitleTextBox.Text = "";
            DescriptionTextBox.Text = "";
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            isEditing = true;

            //Fill text fields with data from the table
            TitleTextBox.Text = todoList.Rows[ToDoListView.CurrentCell.RowIndex].ItemArray[1].ToString();
            DescriptionTextBox.Text = todoList.Rows[ToDoListView.CurrentCell.RowIndex].ItemArray[2].ToString();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                int id = (int)todoList.Rows[ToDoListView.CurrentCell.RowIndex]["Id"];
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ToDoListDb"].ConnectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM ToDoItems WHERE Id=@Id";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }

            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection= new SqlConnection(ConfigurationManager.ConnectionStrings["ToDoListDb"].ConnectionString))
            {
                connection.Open();

                string query;
                
                if(isEditing)
                {
                    query = "UPDATE ToDoItems SET Title= @Title, Description=@Description where Id=@Id";
                }
                else
                {
                    query = "INSERT INTO ToDoItems (Title, Description) VALUES (@Title, @Description)";
                }

                using (SqlCommand command= new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Title", TitleTextBox.Text);
                    command.Parameters.AddWithValue("@Description", DescriptionTextBox.Text);
                    
                    if(isEditing)
                    {
                        command.Parameters.AddWithValue("@Id", todoList.Rows[ToDoListView.CurrentCell.RowIndex]["Id"]);
                    }

                    command.ExecuteNonQuery();
                }
                TitleTextBox.Text = "";
                DescriptionTextBox.Text = "";
                isEditing = false;

                LoadDataFromDatabase();
            }
        }

        private void LoadDataFromDatabase()
        {
            using (SqlConnection connection=new SqlConnection(ConfigurationManager.ConnectionStrings["ToDoListDb"].ConnectionString))
            {
                connection.Open();
                string query = "SELECT * FROM ToDoItems";
                SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                todoList = dataTable;
                ToDoListView.DataSource = dataTable;
            }
        }
    }
}
