using Microsoft.EntityFrameworkCore;
using Service_Center.Data;
using Service_Center.Models;
using Service_Store.Models;
using System;
using System.Linq;
using System.Windows;

namespace Service_Store
{
    public partial class EmployeeShiftsWindow : Window
    {
        private Employee _currentEmployee;

        /// <summary>
        /// Ініціалізує вікно графіка змін працівників.
        /// </summary>
        public EmployeeShiftsWindow(Employee currentEmployee)
        {
            InitializeComponent();
            _currentEmployee = currentEmployee;

            CheckAdminPermissions();
            LoadEmployees();
            LoadShifts();
        }

        /// <summary>
        /// Перевіряє права адміністратора для редагування змін.
        /// </summary>
        private void CheckAdminPermissions()
        {
            // Перевірка RBAC: Адміністратор має PositionId == 1
            if (_currentEmployee == null || _currentEmployee.PositionId != 1)
            {
                pnlAdminControls.IsEnabled = false;
                pnlAdminControls.Visibility = Visibility.Collapsed;
                this.Title = "Графік роботи працівників (Тільки перегляд)";
            }
        }

        /// <summary>
        /// Завантажує працівників до списку вибору.
        /// </summary>
        private void LoadEmployees()
        {
            using (var db = new AppDbContext())
            {
                cbEmployees.ItemsSource = db.Employees.ToList();
            }
        }

        /// <summary>
        /// Завантажує графік змін до таблиці.
        /// </summary>
        private void LoadShifts()
        {
            using (var db = new AppDbContext())
            {
                var shifts = db.EmployeeShifts
                    .Include(s => s.Employee)
                    .OrderByDescending(s => s.ShiftDate)
                    .Select(s => new
                    {
                        ShiftId = s.ShiftId,
                        ShiftDate = s.ShiftDate,
                        EmployeeName = s.Employee != null ? s.Employee.FirstName + " " + s.Employee.LastName : "Невідомо",
                        TimeRange = $"{s.StartTime:hh\\:mm} - {s.EndTime:hh\\:mm}",
                        StatusText = "Робоча зміна"
                    })
                    .ToList();

                dgShifts.ItemsSource = shifts;
            }
        }

        /// <summary>
        /// Перевіряє та зберігає зміну працівника.
        /// </summary>
        private void SaveShift_Click(object sender, RoutedEventArgs e)
        {
            if (cbEmployees.SelectedValue == null)
            {
                MessageBox.Show("Оберіть працівника!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (dpShiftDate.SelectedDate == null)
            {
                MessageBox.Show("Оберіть дату зміни!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!TimeSpan.TryParse(txtStartTime.Text, out TimeSpan startTime) || !TimeSpan.TryParse(txtEndTime.Text, out TimeSpan endTime))
            {
                MessageBox.Show("Введіть коректний формат часу (наприклад, 09:00)!", "Попередження", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int employeeId = Convert.ToInt32(cbEmployees.SelectedValue);
            DateTime shiftDate = dpShiftDate.SelectedDate.Value.Date;

            using (var db = new AppDbContext())
            {
                // Перевіряємо, чи вже існує зміна для цього працівника на цю дату
                var existingShift = db.EmployeeShifts
                    .FirstOrDefault(s => s.EmployeeId == employeeId && s.ShiftDate.Date == shiftDate);

                if (existingShift != null)
                {
                    // Оновлюємо існуючу
                    existingShift.StartTime = startTime;
                    existingShift.EndTime = endTime;
                }
                else
                {
                    // Створюємо нову
                    var newShift = new EmployeeShift
                    {
                        EmployeeId = employeeId,
                        ShiftDate = shiftDate,
                        StartTime = startTime,
                        EndTime = endTime
                    };
                    db.EmployeeShifts.Add(newShift);
                }

                db.SaveChanges();
            }

            MessageBox.Show("Графік успішно збережено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
            LoadShifts();
        }
    }
}