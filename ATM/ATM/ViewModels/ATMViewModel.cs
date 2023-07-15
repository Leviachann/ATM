using System;
using System.Threading;
using System.Windows.Input;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using ATM.ViewModels;

namespace ATMApp.ViewModels
{
    public class ATMViewModel : BaseViewModel
    {
        private string cardNumber;
        private string balanceText;
        private string transferAmountText;
        private double balance;
        private double transferAmount;
        private static Mutex mutex = new Mutex(false, "ATMAppMutex");

        public ATMViewModel()
        {
            LoadCommand = new RelayCommand(Load, CanLoad);
            TransferCommand = new RelayCommand(Transfer, CanTransfer);
        }

        public string CardNumber
        {
            get { return cardNumber; }
            set
            {
                cardNumber = value;
                OnPropertyChanged(nameof(CardNumber));
            }
        }

        public string BalanceText
        {
            get { return balanceText; }
            set
            {
                balanceText = value;
                OnPropertyChanged(nameof(BalanceText));
            }
        }

        public string TransferAmountText
        {
            get { return transferAmountText; }
            set
            {
                transferAmountText = value;
                OnPropertyChanged(nameof(TransferAmountText));
            }
        }

        public ICommand LoadCommand { get; }
        public ICommand TransferCommand { get; }

        private bool CanLoad()
        {
            return mutex.WaitOne(0);
        }

        private bool CanTransfer()
        {
            return mutex.WaitOne(0);
        }

        private void Load()
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ATMConnectionString"].ConnectionString;
                string query = "SELECT UserName, Balance FROM Users WHERE CardNumber = @CardNumber";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@CardNumber", CardNumber);

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string userName = reader.GetString(0);
                        double userBalance = reader.GetDouble(1);

                        BalanceText = $"User: {userName}\nBalance: {userBalance:C}";
                    }
                    else
                    {
                        BalanceText = "Invalid Card Number";
                    }
                }
            }
            catch (Exception ex)
            {
                BalanceText = $"An error occurred while loading the data: {ex.Message}";
            }
        }




        private void Transfer()
        {
            if (CanTransfer())
            {
                try
                {
                    mutex.WaitOne();

                    if (double.TryParse(TransferAmountText, out double transferAmount))
                    {
                        balance += transferAmount;
                        BalanceText = $"User: {CardNumber}\nBalance: {balance:C}";
                    }

                    string scriptFilePath = @"Path\to\script.sql";
                    string script = File.ReadAllText(scriptFilePath);

                    string connectionString = ConfigurationManager.ConnectionStrings["ATMConnectionString"].ConnectionString;

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand createTableCommand = new SqlCommand(script, connection);
                        createTableCommand.ExecuteNonQuery();
                    }
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }
    }
}
