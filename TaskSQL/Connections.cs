using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskSQL
{
    class Connections
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public Connections()
        {
            Init();
        }

        private void Init()
        {
            server = "localhost";
            database = "petshop";
            uid = "root";
            password = "***";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

#region Pets Connection Sector
        //Load all Data from DB & Search
        public List<Pets> LoadSqlPets(string chosenParameter, string searchText)
        {
            List<Pets> pets = new List<Pets>();

            string query = "SELECT pets.PetID, pettype.PetType, pettype.PetDescription, pets.PetName, " +
                           "pets.PetStatus, staff.FirstName, staff.LastName, " +
                           "clients.ClFirstName, clients.ClLastName " +
                           "FROM pets " +
                           "INNER JOIN pettype " +
                           "ON pets.PetTypeID = pettype.PetTypeID " +
                           "LEFT JOIN staff " +
                           "ON pets.StaffID = staff.StaffID " +
                           "LEFT JOIN clients " +
                           "ON pets.ClientID = clients.ClientID ";

            switch (chosenParameter)
            {
                case "Buyer":
                    query += "WHERE clients.ClFirstName LIKE '" + searchText + "%'";
                    break;
                case "Seller":
                    query += "WHERE staff.FirstName LIKE '" + searchText + "%'";
                    break;
                case "Status":
                    query += "WHERE pets.PetStatus LIKE '" + searchText + "%'";
                    break;
                case "Pet Type":
                    query += "WHERE pettype.PetType LIKE '" + searchText + "%'";
                    break;
                case "Pet Id":
                    query += "WHERE pets.PetID LIKE '" + searchText + "%'";
                    break;
                default:
                    break;
            }

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    pets.Add(new Pets(reader["PetID"].ToString(),
                                      reader["PetType"].ToString(),
                                      reader["PetDescription"].ToString(),
                                      reader["PetName"].ToString(),
                                      reader["PetStatus"].ToString(),
                                      reader["FirstName"].ToString(),
                                      reader["LastName"].ToString(),
                                      reader["ClFirstName"].ToString(),
                                      reader["ClLastName"].ToString()));
                }
                reader.Close();
                this.Close();
            }
            else
            {
                pets = null;
            }

            return pets;
        }
      
        public void AddNewPet(List<Pets> newPets, string petTypeId, Label label)
        {
            string query = "INSERT INTO pets (PetName, PetTypeID, PetStatus) " +
                "VALUES ('" + newPets[newPets.Count - 1].name + "'," + 
                              Convert.ToInt32(petTypeId) + ",'" +
                              newPets[newPets.Count - 1].status + "');";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();   
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }

        public void SellPet(int petId, int sellerId, int clientId, Label label)
        {
            string query = "UPDATE pets " +
                           "SET pets.PetStatus = 'Sold', " +
                           "pets.StaffID = " + sellerId + ", " +
                           "pets.ClientID = " + clientId +
                           " WHERE petId = " + petId;               

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "The pet is sold. Thank you.";
            }
            else
            {
                label.Text = "Error";
            }
        }
#endregion

#region Staff Connection Sector
        //Load Staff Data from DB & Search
        public List<Staff> LoadSqlStaff(string chosenParameter, string searchText)
        {
            List<Staff> staff = new List<Staff>();

            string query = "SELECT staff.StaffID, staff.FirstName, staff.LastName, staff.Phone " +
                           "FROM staff";
            switch (chosenParameter)
            {
                case "Id":
                    query += " WHERE staff.StaffID LIKE '" + searchText + "%'";
                    break;
                case "First Name":
                    query += " WHERE staff.FirstName LIKE '" + searchText + "%'";
                    break;
                case "Last Name":
                    query += " WHERE staff.LastName LIKE '" + searchText + "%'";
                    break;
                default:
                    break;
            }

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    staff.Add(new Staff(reader["StaffID"].ToString(),
                                      reader["FirstName"].ToString(),
                                      reader["LastName"].ToString(),
                                      reader["Phone"].ToString()));
                }

                reader.Close();
                this.Close();
            }
            else
            {
                staff = null;
            }

            return staff;
        }

        public void AddNewStaff(List<Staff> staff, Label label)
        {
            string query = "INSERT INTO staff (FirstName, LastName, Phone) " +
                "VALUES ('" + staff[staff.Count - 1].staffFirstName + "','" +
                              staff[staff.Count - 1].staffLastName + "'," +
                              Convert.ToInt32(staff[staff.Count - 1].staffPhone) + ");";                             

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }

        public void EditStaff(List<Staff> staff, int index, Label label)
        {
            string query = "UPDATE staff " +
                           "SET staff.FirstName = '" + staff[index].staffFirstName + "', " +
                           "staff.LastName = '" + staff[index].staffLastName + "', " +
                           "staff.Phone = " + Convert.ToInt32(staff[index].staffPhone) +
                           " WHERE staff.StaffID = " + (index + 1) + ";";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }
#endregion

#region Clients Connection Sector
        //Load Client Data from DB & Search
        public List<Clients> LoadSqlClients(string chosenParameter, string searchText)
        {
            List<Clients> clients = new List<Clients>();

            string query = "SELECT clients.ClientID, clients.ClFirstName, clients.ClLastName, clients.ClPhone " +
                           "FROM clients";
            switch (chosenParameter)
            {
                case "Id":
                    query += " WHERE clients.ClientID LIKE '" + searchText + "%'";
                    break;
                case "First Name":
                    query += " WHERE clients.ClFirstName LIKE '" + searchText + "%'";
                    break;
                case "Last Name":
                    query += " WHERE clients.ClLastName LIKE '" + searchText + "%'";
                    break;
                default:
                    break;
            }

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    clients.Add(new Clients(reader["ClientID"].ToString(),
                                      reader["ClFirstName"].ToString(),
                                      reader["ClLastName"].ToString(),
                                      reader["ClPhone"].ToString()));
                }

                reader.Close();
                this.Close();
            }
            else
            {
                clients = null;
            }

            return clients;
        }

        public void AddNewClient(List<Clients> clients, Label label)
        {
            string query = "INSERT INTO clients (ClFirstName, ClLastName, ClPhone) " +
                "VALUES ('" + clients[clients.Count - 1].clientFirstName + "','" +
                              clients[clients.Count - 1].clientLastName + "'," +
                              Convert.ToInt32(clients[clients.Count - 1].clientPhone) + ");";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }

        public void EditClient(List<Clients> clients, int index, Label label)
        {
            string query = "UPDATE clients " +
                           "SET clients.ClFirstName = '" + clients[index].clientFirstName + "', " +
                           "clients.ClLastName = '" + clients[index].clientLastName + "', " +
                           "clients.ClPhone = " + Convert.ToInt32(clients[index].clientPhone) +
                           " WHERE clients.ClientID = " + (index + 1) + ";";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }

#endregion

#region PetType Connection Sector
        //Load Pet-Type Data from DB
        public List<PetType> LoadSqlPetType(string chosenParameter, string searchText)
        {
            List<PetType> petType = new List<PetType>();

            string query = "SELECT pettype.PetTypeID, pettype.PetType, pettype.PetDescription " +
                           "FROM pettype";
            switch (chosenParameter)
            {
                case "Pet Id":
                    query += " WHERE pettype.PetTypeID LIKE '" + searchText + "%'";
                    break;
                case "Pet Type":
                    query += " WHERE pettype.PetType LIKE '" + searchText + "%'";
                    break;
                default:
                    break;
            }

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    petType.Add(new PetType(reader["PetTypeID"].ToString(),
                                      reader["PetType"].ToString(),
                                      reader["PetDescription"].ToString()));
                }

                reader.Close();
                this.Close();
            }
            else
            {
                petType = null;
            }

            return petType;
        }

        public void AddNewPetType(List<PetType> type, Label label)
        {
            string query = "INSERT INTO pettype (PetType, PetDescription) " +
                "VALUES ('" + type[type.Count - 1].type + "','" +
                              type[type.Count - 1].description + "');";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }

        public void EditPetType(List<PetType> type, int index, Label label)
        {
            string query = "UPDATE pettype " +
                           "SET pettype.PetType = '" + type[index].type + "', " +
                           "pettype.PetDescription = '" + type[index].description + "'" +
                           " WHERE pettype.PetTypeID = " + (index + 1) + ";";

            if (this.Open() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.Close();
                label.Text = "Data Stored Successfully";
            }
            else
            {
                label.Text = "Error";
            }
        }
#endregion

        public bool Open()
        {
            try
            {
                connection.Open();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Close()
        {
            try
            {
                connection.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
