using Service_Center.Data;
using Service_Store.Models;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class EditRepairWindow : Window
    {
        private int _repairId;

        public EditRepairWindow(int repairId)
        {
            InitializeComponent();
            _repairId = repairId;
            LoadStatuses();
            LoadRepairData();
        }

        private void LoadStatuses()
        {
            using (var db = new AppDbContext())
            {
                cbStatuses.ItemsSource = db.RepairStatuses.ToList();
            }
        }

        private void LoadRepairData()
        {
            using (var db = new AppDbContext())
            {
                var repair = db.Repairs.FirstOrDefault(r => r.RepairId == _repairId);
                if (repair != null)
                {
                    txtRepairNumber.Text = repair.RepairNumber;
                    cbStatuses.SelectedValue = repair.StatusId;
                    txtInternalNotes.Text = repair.InternalNotes;
                    txtFinalPrice.Text = repair.FinalPrice?.ToString() ?? repair.EstimatedPrice.ToString();
                }
            }
        }

        private void SaveRepair_Click(object sender, RoutedEventArgs e)
        {
            if (cbStatuses.SelectedValue == null)
            {
                MessageBox.Show("Оберіть статус ремонту!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new AppDbContext())
            {
                var repair = db.Repairs.FirstOrDefault(r => r.RepairId == _repairId);
                if (repair != null)
                {
                    repair.StatusId = (int)cbStatuses.SelectedValue;
                    repair.InternalNotes = txtInternalNotes.Text.Trim();

                    if (decimal.TryParse(txtFinalPrice.Text, out decimal price))
                    {
                        repair.FinalPrice = price;
                    }

                    db.SaveChanges();
                }
            }

            MessageBox.Show("Зміни успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            this.DialogResult = true;
            this.Close();
        }
    }
}