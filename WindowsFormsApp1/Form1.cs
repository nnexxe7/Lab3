using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Linq;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private const string baseUrl = "https://localhost:44391";

        public Form1()
        {
            InitializeComponent();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
        }

        // Przycisk Get menu //
        private void button2_Click(object sender, EventArgs e)
        {
            button6.Show();
            button1.Hide();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            button7.Hide();
            button8.Hide();
            label4.Show();
            textBox1.Show();
        }

        // Przycisk Post menu //
        private void button3_Click(object sender, EventArgs e)
        {
            button6.Hide();
            textBox2.Show();
            textBox3.Show();
            textBox4.Show();
            label1.Show();
            label2.Show();
            label3.Show();
            button7.Show();
            button6.Hide();
            button8.Hide();
            label4.Hide();
            textBox1.Hide();
        }

        // Przycisk Delete menu //
        private void button5_Click(object sender, EventArgs e)
        {
            button6.Hide();
            button1.Show();
            textBox2.Hide();
            textBox3.Hide();
            textBox4.Hide();
            label1.Hide();
            label2.Hide();
            label3.Hide();
            button7.Hide();
            button8.Hide();
            label4.Show();
            textBox1.Show();
        }

        // Przycisk Put menu // 
        private void button4_Click(object sender, EventArgs e)
        {
            label1.Show();
            label2.Show();
            label3.Show();
            label4.Show();
            textBox1.Show();
            textBox2.Show();
            textBox3.Show();
            textBox4.Show();
            button1.Hide();
            button7.Hide();
            button6.Hide();
            button8.Show();
            label4.Show();
            textBox1.Show();
        }


        // Przycisk DELETE // 
        private async void button1_Click_1(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int desiredId))
            {
                bool isDeleted = await DeleteOsobaByIdFromApi(desiredId);

                if (isDeleted)
                {
                    MessageBox.Show($"Osoba o ID {desiredId} została pomyślnie usunięta.");
                }
                else
                {
                    MessageBox.Show("Nie udało się usunąć osoby o podanym ID.");
                }
            }
            else
            {
                MessageBox.Show("Wprowadzono niepoprawną wartość.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // Przycisk POST //
        private async void button7_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && int.TryParse(textBox4.Text, out int rokUrodzenia))
            {
                Osoba nowaOsoba = new Osoba
                {
                    Imie = textBox2.Text,
                    Miasto = textBox3.Text,
                    RokUrodzenia = rokUrodzenia
                };

                bool isAdded = await AddOsobaToApi(nowaOsoba);

                if (isAdded)
                {
                    MessageBox.Show("Nowa osoba została dodana pomyślnie.");
                }
                else
                {
                    MessageBox.Show("Nie udało się dodać nowej osoby.");
                }
            }
            else
            {
                MessageBox.Show("Wprowadzono niepoprawne wartości.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // Przycisk GET //
        private async void button6_Click(object sender, EventArgs e) // Get
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) // Jeśli pole jest puste
            {
                List<Osoba> osoby = await GetOsobyFromApi();

                if (osoby != null && osoby.Any())
                {
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = osoby;
                }
                else
                {
                    MessageBox.Show("Brak danych do wyświetlenia.");
                }
            }
            else if (int.TryParse(textBox1.Text, out int desiredId)) // Jeśli wpisano liczbę
            {
                List<Osoba> osoby = await GetOsobaByIdFromApi(desiredId);

                if (osoby != null && osoby.Any())
                {
                    dataGridView1.AutoGenerateColumns = true;
                    dataGridView1.DataSource = osoby;
                }
                else
                {
                    MessageBox.Show("Nie znaleziono osoby o podanym ID.");
                }
            }
            else
            {
                MessageBox.Show("Wprowadzono niepoprawną wartość.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // Przycisk Put //
        private async void button8_Click(object sender, EventArgs e)
        {
            if (int.TryParse(textBox1.Text, out int id) && !string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && int.TryParse(textBox4.Text, out int rokUrodzenia))
            {
                Osoba updatedOsoba = new Osoba
                {
                    Id = id,
                    Imie = textBox2.Text,
                    Miasto = textBox3.Text,
                    RokUrodzenia = rokUrodzenia
                };

                bool isUpdated = await UpdateOsobaInApi(updatedOsoba);

                if (isUpdated)
                {
                    MessageBox.Show($"Osoba o ID {id} została zaktualizowana pomyślnie.");
                }
                else
                {
                    MessageBox.Show("Nie udało się zaktualizować osoby o podanym ID.");
                }
            }
            else
            {
                MessageBox.Show("Wprowadzono niepoprawne wartości.");
            }

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
        }

        // GET //
        private async Task<List<Osoba>> GetOsobaByIdFromApi(int id) // Get po ID
        {
            List<Osoba> osoba = new List<Osoba>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync($"api/osoby/{id}");

                if (response.IsSuccessStatusCode)
                {
                    osoba.Add(await response.Content.ReadAsAsync<Osoba>());
                }
                else
                {
                    osoba = null;
                }
            }

            return osoba;
        }

        private async Task<List<Osoba>> GetOsobyFromApi() // Get wszystkich
        {
            List<Osoba> osoby = new List<Osoba>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.GetAsync("api/osoby");

                if (response.IsSuccessStatusCode)
                {
                    osoby = await response.Content.ReadAsAsync<List<Osoba>>();
                }
                else
                {
                    MessageBox.Show("Error: " + response.StatusCode);
                }
            }

            return osoby;
        }

        // POST //
        private async Task<bool> AddOsobaToApi(Osoba nowaOsoba)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PostAsJsonAsync("api/osoby", nowaOsoba);

                return response.IsSuccessStatusCode;
            }
        }

        // PUT //
        private async Task<bool> UpdateOsobaInApi(Osoba updatedOsoba)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.PutAsJsonAsync($"api/osoby/{updatedOsoba.Id}", updatedOsoba);

                return response.IsSuccessStatusCode;
            }
        }

        // DELETE //
        private async Task<bool> DeleteOsobaByIdFromApi(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await client.DeleteAsync($"api/osoby/{id}");

                return response.IsSuccessStatusCode;
            }
        }
    }
    public class Osoba
    {
        public int Id { get; set; }
        public string Imie { get; set; }
        public string Miasto { get; set; }
        public int RokUrodzenia { get; set; }
    }
}
