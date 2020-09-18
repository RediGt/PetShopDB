using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TaskSQL
{
    public partial class PetShopForm : Form
    {
        private List<Pets> pets = new List<Pets>();
        private List<PetType> petType = new List<PetType>();
        private List<Staff> staff = new List<Staff>();
        private List<Clients> clients = new List<Clients>();
        Connections con = new Connections();

        public PetShopForm()
        {
            InitializeComponent();
            InitializeBanner();
            LoadAllDataFromDB();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabStaff);
            tabControl1.TabPages.Remove(tabClients);
            tabControl1.TabPages.Remove(tabPetType);
            HideBoxesInMain();
        }

        private void InitializeBanner()
        {
            PictureBox banner = new PictureBox();
            banner.BackColor = Color.Transparent;
            banner.Size = new Size(350, 200);
            banner.Location = new Point(50, 30);
            banner.Image = Properties.Resources.LogoSavana;
            banner.SizeMode = PictureBoxSizeMode.StretchImage;

            tabMain.Controls.Add(banner);
        }

        private void LoadAllDataFromDB()
        {
            pets = con.LoadSqlPets("Show all", "");
            petType = con.LoadSqlPetType("Show all", "");
            staff = con.LoadSqlStaff("Show all", "");
            clients = con.LoadSqlClients("Show all", "");

            if (pets != null)
            {
                PetDataToGrid();
            }
            if (staff != null)
            {
                InitializeComboSellerInMain();
            }
            if (clients != null)
            {
                InitializeComboClientsInMain();
            }
            if (petType != null)
            {
                InitializeComboPetTypeInMain();
            }
        }
        
        private void PetDataToGrid()
        {
            dataGridMain.Rows.Clear();
            foreach (var pet in pets)
            {
                int n = dataGridMain.Rows.Add();
                dataGridMain.Rows[n].Cells[0].Value = pet.id;
                dataGridMain.Rows[n].Cells[1].Value = pet.type;
                dataGridMain.Rows[n].Cells[2].Value = pet.description;
                dataGridMain.Rows[n].Cells[3].Value = pet.name;
                dataGridMain.Rows[n].Cells[4].Value = pet.status;
                dataGridMain.Rows[n].Cells[5].Value = pet.staffFirstName + " " + pet.staffLastName;
                dataGridMain.Rows[n].Cells[6].Value = pet.clientFirstName + " " + pet.clientLastName;
            }
        }

        private void InitializeComboSellerInMain()
        {           
            comBoxMainSeller.Items.Clear();           
            foreach (var seller in staff)
            {
                comBoxMainSeller.Items.Add(seller.staffFirstName + " " + seller.staffLastName);
            }
        }

        private void InitializeComboClientsInMain()
        {
            comBoxMainClient.Items.Clear();
            foreach (var client in clients)
            {
                comBoxMainClient.Items.Add(client.clientFirstName + " " + client.clientLastName);
            }
        }

        private void InitializeComboPetTypeInMain()
        {
            comBoxMainPetType.Items.Clear();
            foreach (var type in petType)
            {
                comBoxMainPetType.Items.Add(type.type);
            }
        }

        private void btnMainPets_Click(object sender, EventArgs e)
        {
            dataGridMain.Visible = true;
            label6.Visible = true;
            comBoxMainSearch.Visible = true;
            tBoxMainSearch.Visible = true;
            btnMainNew.Visible = true;
            btnMainSell.Visible = true;
        }

        private void bttNew_Click(object sender, EventArgs e)
        {
            if (comBoxMainPetType.Text != "" && tBoxMainPetName.Text != "" && comBoxMainStatus.Text != "")
            {
                Pets newPet = new Pets((pets.Count + 1).ToString(), comBoxMainPetType.Text, tBoxMainDescription.Text, tBoxMainPetName.Text,
                    comBoxMainStatus.Text, "", "", "", "");
                pets.Add(newPet);
                string petTypeId = SearchForPetTypeId();
                MessageBox.Show("petTypeID = " + petTypeId);

                PetDataToGrid();
                ClearBoxesInMain();
                con.AddNewPet(pets, petTypeId);
            }
        }

        private string SearchForPetTypeId()
        {
            string petTypeId = "";
            for (int i = 0; i < petType.Count; i++)
            {
                if (comBoxMainPetType.Text == petType[i].type)
                {
                    petTypeId = petType[i].typeId;
                    break;
                }
            }
            return petTypeId;
        }

        private void FromGrigToBoxesInMain()
        {
            comBoxMainPetType.Text = dataGridMain.SelectedRows[0].Cells[1].Value.ToString();
            tBoxMainDescription.Text = dataGridMain.SelectedRows[0].Cells[2].Value.ToString();
            tBoxMainPetName.Text = dataGridMain.SelectedRows[0].Cells[3].Value.ToString();
            comBoxMainStatus.Text = dataGridMain.SelectedRows[0].Cells[4].Value.ToString();
            comBoxMainSeller.Text = dataGridMain.SelectedRows[0].Cells[5].Value.ToString();
            comBoxMainClient.Text = dataGridMain.SelectedRows[0].Cells[6].Value.ToString();
        }

        private void dataGridMain_MouseClick(object sender, MouseEventArgs e)
        {
            ClearBoxesInMain();
            FromGrigToBoxesInMain();
        }

        private void bttSell_Click(object sender, EventArgs e)
        {
            if (comBoxMainStatus.Text == "Sold")
            {
                MessageBox.Show("The Pet has been already Sold!");
                return;
            }

            if (comBoxMainStatus.Text == "Available" && comBoxMainSeller.Text != "" &&
                comBoxMainClient.Text != "")
            {
                comBoxMainStatus.Text = "Sold";
                int selectedPetIndex = dataGridMain.SelectedRows[0].Index;
                int petId = Convert.ToInt32(pets[selectedPetIndex].id);
                int sellerId = Convert.ToInt32(comBoxMainSeller.SelectedIndex + 1);
                int clientId = Convert.ToInt32(comBoxMainClient.SelectedIndex + 1);

                pets[selectedPetIndex].status = comBoxMainStatus.Text;
                pets[selectedPetIndex].staffFirstName = staff[sellerId - 1].staffFirstName;
                pets[selectedPetIndex].staffLastName = staff[sellerId - 1].staffLastName;
                pets[selectedPetIndex].clientFirstName = clients[clientId - 1].clientFirstName;
                pets[selectedPetIndex].clientLastName = clients[clientId - 1].clientLastName;

                ClearBoxesInMain();
                PetDataToGrid();
                con.SellPet(petId, sellerId, clientId);
            }
        }

        private void ClearBoxesInMain()
        {
            comBoxMainPetType.Text = "";
            tBoxMainDescription.Text = "";
            tBoxMainPetName.Text = "";
            comBoxMainStatus.SelectedIndex = -1;
            comBoxMainSeller.SelectedIndex = -1;
            comBoxMainClient.SelectedIndex = -1;
        }

        private void HideBoxesInMain()
        {
            lblMainPetType.Visible = false;
            lblMainDescription.Visible = false;
            lblMainName.Visible = false;
            lblMainStatus.Visible = false;
            lblMainSeller.Visible = false;
            lblMainClient.Visible = false;
            comBoxMainPetType.Visible = false;
            tBoxMainDescription.Visible = false;
            tBoxMainPetName.Visible = false;
            comBoxMainStatus.Visible = false;
            comBoxMainSeller.Visible = false;
            comBoxMainClient.Visible = false;
        }

        private void bttClear_Click(object sender, EventArgs e)
        {
            ClearBoxesInMain();
        }

        private void tBoxMainSearch_TextChanged(object sender, EventArgs e)
        {
            pets = con.LoadSqlPets(comBoxMainSearch.Text, tBoxMainSearch.Text);

            if (pets != null)
            {
                PetDataToGrid();
            }
        }

        private void comBoxMainSearch_TextChanged(object sender, EventArgs e)
        {
            tBoxMainSearch.Text = "";
            pets = con.LoadSqlPets("Show all", "");
        }

        private void bttExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabMain);

            HideStaffBoxes();
            lblOperationWithStaff.Text = "";

            tabControl1.TabPages.Add(tabStaff);
            tabControl1.SelectedTab = tabStaff;

            StaffDataToGrid();
        }

        private void btnClients_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabMain);

            HideClientsBoxes();
            lblOperationWithClients.Text = "";

            tabControl1.TabPages.Add(tabClients);
            tabControl1.SelectedTab = tabClients;

            ClientsDataToGrid();
        }

        private void btnPetType_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabMain);

            HidePetTypeBoxes();
            lblOperationWithPetType.Text = "";

            tabControl1.TabPages.Add(tabPetType);
            tabControl1.SelectedTab = tabPetType;

            PetTypeDataToGrid();
        }

#region Staff TabControl Sector
        private void btnStaffBack_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabStaff);
            tabControl1.TabPages.Add(tabMain);
            tabControl1.SelectedTab = tabMain;
        }

        private void StaffDataToGrid()
        {
            dataGridStaff.Rows.Clear();
            foreach (var employee in staff)
            {
                int n = dataGridStaff.Rows.Add();
                dataGridStaff.Rows[n].Cells[0].Value = employee.id;
                dataGridStaff.Rows[n].Cells[1].Value = employee.staffFirstName;
                dataGridStaff.Rows[n].Cells[2].Value = employee.staffLastName;
                dataGridStaff.Rows[n].Cells[3].Value = employee.staffPhone;
            }
        }

        private void btnStaffNew_Click(object sender, EventArgs e)
        {
            ClearStaffBoxes();
            lblOperationWithStaff.Visible = true;
            lblOperationWithStaff.Text = "Insert new employee data :";
            VisiblizeStaffBoxes();
        }

        private void tBoxStaffFirstName_TextChanged(object sender, EventArgs e)
        {
            StaffDataFilledCheck();
        }

        private void tBoxStaffLastName_TextChanged(object sender, EventArgs e)
        {
            StaffDataFilledCheck();
        }

        private void tBoxStaffPhone_TextChanged(object sender, EventArgs e)
        {
            StaffDataFilledCheck();
        }

        private void StaffDataFilledCheck()
        {
            if (tBoxStaffFirstName.Text != "" &&
                tBoxStaffLastName.Text != "" &&
                tBoxStaffPhone.Text != "")
            {
                btnStaffOK.Visible = true;
                btnStaffCalcel.Visible = true;
            }
            else
            {
                btnStaffOK.Visible = false;
                btnStaffCalcel.Visible = false;
            }
        }

        private void btnStaffOK_Click(object sender, EventArgs e)
        {
            if (lblOperationWithStaff.Text == "Insert new employee data :")
                InsertNewStaff();
            if (lblOperationWithStaff.Text == "Edit employee data :")
                EditStaff();
        }

        private void InsertNewStaff()
        {
            Staff newStaff = new Staff((staff.Count + 1).ToString(), tBoxStaffFirstName.Text,
                                        tBoxStaffLastName.Text, tBoxStaffPhone.Text);
            staff.Add(newStaff);

            StaffDataToGrid();
            ClearStaffBoxes();
            HideStaffBoxes();
            con.AddNewStaff(staff, lblOperationWithStaff);
            InitializeComboSellerInMain();
        }

        private void EditStaff()
        {
            int selectedStaffIndex = dataGridStaff.SelectedRows[0].Index;

            staff[selectedStaffIndex].staffFirstName = tBoxStaffFirstName.Text;
            staff[selectedStaffIndex].staffLastName = tBoxStaffLastName.Text;
            staff[selectedStaffIndex].staffPhone = tBoxStaffPhone.Text;

            ClearStaffBoxes();
            StaffDataToGrid();
            HideStaffBoxes();
            con.EditStaff(staff, selectedStaffIndex, lblOperationWithStaff);

            pets = con.LoadSqlPets("Show all", "");
            InitializeComboSellerInMain();
            PetDataToGrid();
        }

        private void ClearStaffBoxes()
        {
            tBoxStaffFirstName.Text = "";
            tBoxStaffLastName.Text = "";
            tBoxStaffPhone.Text = "";
        }

        private void HideStaffBoxes()
        {
            lblStaffFirstName.Visible = false;
            lblStaffLastName.Visible = false;
            lblStaffPhone.Visible = false;
            tBoxStaffFirstName.Visible = false;
            tBoxStaffLastName.Visible = false;
            tBoxStaffPhone.Visible = false;
            btnStaffOK.Visible = false;
            btnStaffCalcel.Visible = false;
        }

        private void VisiblizeStaffBoxes()
        {
            lblStaffFirstName.Visible = true;
            lblStaffLastName.Visible = true;
            lblStaffPhone.Visible = true;
            tBoxStaffFirstName.Visible = true;
            tBoxStaffLastName.Visible = true;
            tBoxStaffPhone.Visible = true;
        }

        private void btnStaffCalcel_Click(object sender, EventArgs e)
        {
            HideStaffBoxes();
            lblOperationWithStaff.Visible = false;
        }

        private void btnStaffEdit_Click(object sender, EventArgs e)
        {
            ClearStaffBoxes();
            lblOperationWithStaff.Visible = true;
            lblOperationWithStaff.Text = "Click on the List for necessary employee :";
            VisiblizeStaffBoxes();
        }

        private void dataGridStaff_MouseClick(object sender, MouseEventArgs e)
        {
            ClearStaffBoxes();
            lblOperationWithStaff.Text = "Edit employee data :";
            FromStaffGrigToBoxes();
        }

        private void FromStaffGrigToBoxes()
        {
            tBoxStaffFirstName.Text = dataGridStaff.SelectedRows[0].Cells[1].Value.ToString();
            tBoxStaffLastName.Text = dataGridStaff.SelectedRows[0].Cells[2].Value.ToString();
            tBoxStaffPhone.Text = dataGridStaff.SelectedRows[0].Cells[3].Value.ToString();
        }

        private void tBoxStaffSearch_TextChanged(object sender, EventArgs e)
        {
            staff = con.LoadSqlStaff(comBoxStaffSearch.Text, tBoxStaffSearch.Text);

            if (staff != null)
            {
                StaffDataToGrid();
            }
        }

        private void comBoxStaffSearch_TextChanged(object sender, EventArgs e)
        {
            tBoxStaffSearch.Text = "";
            staff = con.LoadSqlStaff("Show all", "");
        }
#endregion

#region Clients TabControl Sector
        private void btnClientsBack_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabClients);
            tabControl1.TabPages.Add(tabMain);
            tabControl1.SelectedTab = tabMain;
        }
       
        private void ClientsDataToGrid()
        {
            dataGridClients.Rows.Clear();
            foreach (var client in clients)
            {
                int n = dataGridClients.Rows.Add();
                dataGridClients.Rows[n].Cells[0].Value = client.clientId;
                dataGridClients.Rows[n].Cells[1].Value = client.clientFirstName;
                dataGridClients.Rows[n].Cells[2].Value = client.clientLastName;
                dataGridClients.Rows[n].Cells[3].Value = client.clientPhone;
            }
        }

        private void btnClientsNew_Click(object sender, EventArgs e)
        {
            ClearClientsBoxes();
            lblOperationWithClients.Visible = true;
            lblOperationWithClients.Text = "Insert new client data :";
            VisiblizeClientsBoxes();
        }

        private void tBoxClientsFirstName_TextChanged(object sender, EventArgs e)
        {
            ClientsDataFilledCheck();
        }

        private void tBoxClientsLastName_TextChanged(object sender, EventArgs e)
        {
            ClientsDataFilledCheck();
        }

        private void tBoxClientsPhone_TextChanged(object sender, EventArgs e)
        {
            ClientsDataFilledCheck();
        }

        private void ClientsDataFilledCheck()
        {
            if (tBoxClientsFirstName.Text != "" &&
                tBoxClientsLastName.Text != "" &&
                tBoxClientsPhone.Text != "")
            {
                btnClientsOK.Visible = true;
                btnClientsCancel.Visible = true;
            }
            else
            {
                btnClientsOK.Visible = false;
                btnClientsCancel.Visible = false;
            }
        }

        private void btnClientsOK_Click(object sender, EventArgs e)
        {
            if (lblOperationWithClients.Text == "Insert new client data :")
                InsertNewClient();
            if (lblOperationWithClients.Text == "Edit client data :")
                EditClient();
        }

        private void InsertNewClient()
        {
            Clients newClient = new Clients((clients.Count + 1).ToString(), tBoxClientsFirstName.Text,
                                        tBoxClientsLastName.Text, tBoxClientsPhone.Text);
            clients.Add(newClient);

            ClientsDataToGrid();
            ClearClientsBoxes();
            HideClientsBoxes();
            con.AddNewClient(clients, lblOperationWithClients);
            InitializeComboClientsInMain();
        }

        private void EditClient()
        {
            int selectedClientIndex = dataGridClients.SelectedRows[0].Index;

            clients[selectedClientIndex].clientFirstName = tBoxClientsFirstName.Text;
            clients[selectedClientIndex].clientLastName = tBoxClientsLastName.Text;
            clients[selectedClientIndex].clientPhone = tBoxClientsPhone.Text;

            ClearClientsBoxes();
            ClientsDataToGrid();
            HideClientsBoxes();
            con.EditClient(clients, selectedClientIndex, lblOperationWithClients);

            pets = con.LoadSqlPets("Show all", "");
            InitializeComboClientsInMain();
            PetDataToGrid();
        }

        private void ClearClientsBoxes()
        {
            tBoxClientsFirstName.Text = "";
            tBoxClientsLastName.Text = "";
            tBoxClientsPhone.Text = "";
        }

        private void HideClientsBoxes()
        {
            lblClientsFirstName.Visible = false;
            lblClientsLastName.Visible = false;
            lblClientsPhone.Visible = false;
            tBoxClientsFirstName.Visible = false;
            tBoxClientsLastName.Visible = false;
            tBoxClientsPhone.Visible = false;
            btnClientsOK.Visible = false;
            btnClientsCancel.Visible = false;
        }

        private void VisiblizeClientsBoxes()
        {
            lblClientsFirstName.Visible = true;
            lblClientsLastName.Visible = true;
            lblClientsPhone.Visible = true;
            tBoxClientsFirstName.Visible = true;
            tBoxClientsLastName.Visible = true;
            tBoxClientsPhone.Visible = true;
        }

        private void btnClientsCancel_Click(object sender, EventArgs e)
        {
            HideClientsBoxes();
            lblOperationWithClients.Visible = false;
        }

        private void btnClientsEdit_Click(object sender, EventArgs e)
        {
            ClearClientsBoxes();
            lblOperationWithClients.Visible = true;
            lblOperationWithClients.Text = "Click on the List for necessary client :";
            VisiblizeClientsBoxes();
        }

        private void dataGridClients_MouseClick(object sender, MouseEventArgs e)
        { 
            ClearClientsBoxes();
            lblOperationWithClients.Text = "Edit client data :";
            FromClientsGrigToBoxes();
        }

        private void FromClientsGrigToBoxes()
        {
            tBoxClientsFirstName.Text = dataGridClients.SelectedRows[0].Cells[1].Value.ToString();
            tBoxClientsLastName.Text = dataGridClients.SelectedRows[0].Cells[2].Value.ToString();
            tBoxClientsPhone.Text = dataGridClients.SelectedRows[0].Cells[3].Value.ToString();
        }

        private void tBoxClientsSearch_TextChanged(object sender, EventArgs e)
        {
            clients = con.LoadSqlClients(comBoxClientsSearch.Text, tBoxClientsSearch.Text);

            if (clients != null)
            {
                ClientsDataToGrid();
            }
        }

        private void comBoxClientsSearch_TextChanged(object sender, EventArgs e)
        {
            tBoxClientsSearch.Text = "";
            clients = con.LoadSqlClients("Show all", "");
        }
#endregion

#region Pets TabControl Sector

        private void btnPetTypeBack_Click(object sender, EventArgs e)
        {
            tabControl1.TabPages.Remove(tabPetType);
            tabControl1.TabPages.Add(tabMain);
            tabControl1.SelectedTab = tabMain;
        }

        private void PetTypeDataToGrid()
        {
            dataGridPetType.Rows.Clear();
            foreach (var type in petType)
            {
                int n = dataGridPetType.Rows.Add();
                dataGridPetType.Rows[n].Cells[0].Value = type.typeId;
                dataGridPetType.Rows[n].Cells[1].Value = type.type;
                dataGridPetType.Rows[n].Cells[2].Value = type.description;
            }
        }

        private void btnPetTypeNew_Click(object sender, EventArgs e)
        {
            ClearPetTypeBoxes();
            lblOperationWithPetType.Visible = true;
            lblOperationWithPetType.Text = "Insert new pet type data :";
            VisiblizePetTypeBoxes();
        }

        private void tBoxPetTypeType_TextChanged(object sender, EventArgs e)
        {
            PetTypeDataFilledCheck();
        }

        private void tBoxPetTypeDescription_TextChanged(object sender, EventArgs e)
        {
            PetTypeDataFilledCheck();
        }

        private void PetTypeDataFilledCheck()
        {
            if (tBoxPetTypeType.Text != "" &&
                tBoxPetTypeDescription.Text != "")
            {
                btnPetTypeOK.Visible = true;
                btnPetTypeCancel.Visible = true;
            }
            else
            {
                btnPetTypeOK.Visible = false;
                btnPetTypeCancel.Visible = false;
            }
        }

        private void btnPetTypeOK_Click(object sender, EventArgs e)
        {
            if (lblOperationWithPetType.Text == "Insert new pet type data :")
                InsertNewPetType();
            if (lblOperationWithPetType.Text == "Edit pet type data :")
                EditPetType();
        }

        private void InsertNewPetType()
        {
            PetType newType = new PetType((petType.Count + 1).ToString(), tBoxPetTypeType.Text,
                                        tBoxPetTypeDescription.Text);
            petType.Add(newType);

            PetTypeDataToGrid();
            ClearPetTypeBoxes();
            HidePetTypeBoxes();
            con.AddNewPetType(petType, lblOperationWithPetType);
            InitializeComboPetTypeInMain();
        }

        private void EditPetType()
        {
            int selectedPetTypeIndex = dataGridPetType.SelectedRows[0].Index;

            petType[selectedPetTypeIndex].type = tBoxPetTypeType.Text;
            petType[selectedPetTypeIndex].description = tBoxPetTypeDescription.Text;

            ClearPetTypeBoxes();
            PetTypeDataToGrid();
            HidePetTypeBoxes();
            con.EditPetType(petType, selectedPetTypeIndex, lblOperationWithPetType);

            pets = con.LoadSqlPets("Show all", "");
            InitializeComboPetTypeInMain();
            PetDataToGrid();
        }

        private void ClearPetTypeBoxes()
        {
            tBoxPetTypeType.Text = "";
            tBoxMainDescription.Text = "";
        }

        private void HidePetTypeBoxes()
        {
            lblPetTypeType.Visible = false;
            lblPetTypeDescription.Visible = false;
            tBoxPetTypeType.Visible = false;
            tBoxPetTypeDescription.Visible = false;
            btnPetTypeOK.Visible = false;
            btnPetTypeCancel.Visible = false;
        }

        private void VisiblizePetTypeBoxes()
        {
            lblPetTypeType.Visible = true;
            lblPetTypeDescription.Visible = true;
            tBoxPetTypeType.Visible = true;
            tBoxPetTypeDescription.Visible = true;
        }

        private void btnPetTypeCancel_Click(object sender, EventArgs e)
        {
            HidePetTypeBoxes();
            lblOperationWithPetType.Visible = false;
        }

        private void btnPetTypeEdit_Click(object sender, EventArgs e)
        {
            ClearPetTypeBoxes();
            lblOperationWithPetType.Visible = true;
            lblOperationWithPetType.Text = "Click on the List for necessary pet type :";
            VisiblizePetTypeBoxes();
        }

        private void dataGridPetType_MouseClick(object sender, MouseEventArgs e)
        {
            ClearPetTypeBoxes();
            lblOperationWithPetType.Text = "Edit pet type data :";
            FromPetTypeGrigToBoxes();
        }

        private void FromPetTypeGrigToBoxes()
        {
            tBoxPetTypeType.Text = dataGridPetType.SelectedRows[0].Cells[1].Value.ToString();
            tBoxPetTypeDescription.Text = dataGridPetType.SelectedRows[0].Cells[2].Value.ToString();
        }

        private void tBoxPetTypeSearch_TextChanged(object sender, EventArgs e)
        {
            petType = con.LoadSqlPetType(comBoxPetTypeSearch.Text, tBoxPetTypeSearch.Text);

            if (petType != null)
            {
                PetTypeDataToGrid();
            }
        }

        private void comBoxPetTypeSearch_TextChanged(object sender, EventArgs e)
        {
            tBoxPetTypeSearch.Text = "";
            petType = con.LoadSqlPetType("Show all", "");
        }
        #endregion

    }
}
