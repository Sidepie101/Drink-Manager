using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Aida_Taha_Mohamed
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly SqlConnection conDB;

        public MainWindow()
        {
            InitializeComponent();

            conDB = new SqlConnection(@"Data Source=TahaPC\SQLEXPRESS;Initial Catalog=Aida_Taha_SMohamed;Integrated Security=True");

            chargerDataGrid();
            

            tailleCombo.Items.Add("Petit");
            tailleCombo.Items.Add("Moyen");
            tailleCombo.Items.Add("Grand");

            typeCombo.Items.Add("Froid");
            typeCombo.Items.Add("Chaud");
        }

        private void chargerDataGrid()
        {
            try
            {
                DataTable dt = new DataTable();
                  string query = "select * from cafe INNER JOIN code ON cafe.IdCode = code.IdCode";

                SqlDataAdapter adapter = new SqlDataAdapter(query, @"Data Source=TahaPC\SQLEXPRESS;Initial Catalog=Aida_Taha_SMohamed;Integrated Security=True");

                adapter.Fill(dt);

                datagrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement des données: " + ex.Message);
            }
        }

        


        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string insertQuery = "INSERT INTO cafe (IdCode, boisson, prix, taille, type) VALUES (@IdCode,@boisson, @prix, @taille, @type)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conDB))
                {
                    int nextIdCode = (int)(datagrid.Items.Count) + 1;

                    cmd.Parameters.AddWithValue("@IdCode", 1);
                    cmd.Parameters.AddWithValue("@boisson", boissonTxt.Text);
                    cmd.Parameters.AddWithValue("@prix", Convert.ToDouble(prixTxt.Text));
                    cmd.Parameters.AddWithValue("@taille", tailleCombo.Text);
                    cmd.Parameters.AddWithValue("@type", typeCombo.SelectedItem.ToString());

                    conDB.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conDB.Close();

                    if (rowsAffected > 0)
                    {
                        chargerDataGrid();

                        boissonTxt.Text = "";
                        prixTxt.Text = "";
                        tailleCombo.SelectedIndex = -1;
                        typeCombo.SelectedIndex = -1;

                        MessageBox.Show("Boisson Ajoute!");
                    }
                    else
                    {
                        MessageBox.Show("Echec lors de l'ajout!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnModifier_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (datagrid.SelectedItem != null)
                {
                    DataRowView row = (DataRowView)datagrid.SelectedItem;
                    int code = Convert.ToInt32(row["Code"]);

                    string updateQuery = "UPDATE cafe SET boisson = @boisson, prix = @prix, taille = @taille, type = @type WHERE Code = @code";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, conDB))
                    {
                        cmd.Parameters.AddWithValue("@boisson", boissonTxt.Text);
                        cmd.Parameters.AddWithValue("@prix", Convert.ToDouble(prixTxt.Text));
                        cmd.Parameters.AddWithValue("@taille", tailleCombo.Text);
                        cmd.Parameters.AddWithValue("@type", typeCombo.SelectedItem.ToString());
                        cmd.Parameters.AddWithValue("@code", code);

                        conDB.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conDB.Close();

                        if (rowsAffected > 0)
                        {
                            chargerDataGrid();

                            codeTxt.Text = "";
                            boissonTxt.Text = "";
                            prixTxt.Text = "";
                            tailleCombo.SelectedIndex = -1;
                            typeCombo.SelectedIndex = -1;

                            MessageBox.Show("Boisson Ajoute!");
                        }
                        else
                        {
                            MessageBox.Show("Echec lors de l'ajout!");
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Faites une selection!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (datagrid.SelectedItem is DataRowView ligne)
                {
                    string maRequete = $"DELETE FROM cafe WHERE code = {ligne["code"]}";

                    string connectionString = @"Data Source=TahaPC\SQLEXPRESS;Initial Catalog=Aida_Taha_SMohamed;Integrated Security=True";

                    using (SqlConnection conDB = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand(maRequete, conDB))
                        {
                            conDB.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Modification faite avec succès!", "Confirmation");

                    chargerDataGrid();
                }
                else
                {
                    MessageBox.Show("Veuillez sélectionner une boisson à supprimer.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur exécution de la requête : {ex.Message}");
            }
        }

        private void searchCode(string code)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = "SELECT * FROM cafe WHERE Code = @code";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conDB);
                adapter.SelectCommand.Parameters.AddWithValue("@code", code);

                adapter.Fill(dt);

                datagrid.ItemsSource = dt.DefaultView;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la recherche: " + ex.Message);
            }
        }

        private void codeSearchBtn_Click(object sender, RoutedEventArgs e)
        {
            string code = codeSearchTxt.Text.Trim();

            if (string.IsNullOrEmpty(code))
            {
                chargerDataGrid();
            }
            else
            {
                searchCode(code);
            }
        }




    }
}
