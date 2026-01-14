using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapaPresentacion
{
    public partial class Form1 : Form
    {
        private int filas = 6;
        private int columnas = 10;
        private Label lblInfo;
        private Button btnConfirmar;
        private decimal precioPorAsiento = 10.00m;

        public Form1()
        {
            this.Text = "Cine Pro - Reserva de Entradas";
            this.Size = new Size(650, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(20, 20, 20); // Un poco más oscuro

            CrearInterfazCine();
        }

        private void CrearInterfazCine()
        {
            // [Código anterior de Pantalla y Generación de Asientos...]
            // (Asegúrate de mantener la lógica de los bucles 'for' que ya teníamos)

            // --- NUEVO: BOTÓN DE CONFIRMACIÓN ---
            btnConfirmar = new Button();
            btnConfirmar.Text = "CONFIRMAR RESERVA";
            btnConfirmar.Size = new Size(200, 45);
            btnConfirmar.Location = new Point(225, 530);
            btnConfirmar.BackColor = Color.Gold;
            btnConfirmar.FlatStyle = FlatStyle.Flat;
            btnConfirmar.Font = new Font("Arial", 10, FontStyle.Bold);
            btnConfirmar.Click += BtnConfirmar_Click;
            this.Controls.Add(btnConfirmar);
        }

        private void Asiento_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.BackColor == Color.DimGray)
            {
                btn.BackColor = Color.LimeGreen;
                btn.ForeColor = Color.Black;
            }
            else
            {
                btn.BackColor = Color.DimGray;
                btn.ForeColor = Color.White;
            }

            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            // Contamos cuántos botones están en color verde (seleccionados)
            int seleccionados = this.Controls.OfType<Button>()
                                .Count(b => b.BackColor == Color.LimeGreen);

            decimal total = seleccionados * precioPorAsiento;
            lblInfo.Text = $"Asientos: {seleccionados} | Total: ${total}";
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            // Obtenemos los nombres de los asientos seleccionados
            var asientos = this.Controls.OfType<Button>()
                            .Where(b => b.BackColor == Color.LimeGreen)
                            .Select(b => b.Text)
                            .ToList();

            if (asientos.Count > 0)
            {
                string listaAsientos = string.Join(", ", asientos);
                decimal total = asientos.Count * precioPorAsiento;

                MessageBox.Show($"¡Reserva Exitosa!\n\nAsientos: {listaAsientos}\nTotal a pagar: ${total}",
                                "Ticket de Cine", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Opcional: Bloquear los asientos tras la compra
                FinalizarCompra();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona al menos un asiento.", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FinalizarCompra()
        {
            foreach (var btn in this.Controls.OfType<Button>().Where(b => b.BackColor == Color.LimeGreen))
            {
                btn.BackColor = Color.DarkRed;
                btn.Enabled = false;
            }
            ActualizarResumen();
        }
    }
}
