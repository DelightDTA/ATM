using System.Text;
using SHA256 = System.Security.Cryptography.SHA256;

namespace ATM
{
	internal class Program
	{
		// Global variables
		static int AccountChoice = new Random().Next(0, 3);
		static string[] AccountNames = { "Ivan Petrov", "Maria Sidorova", "Alexey Kozlov" } ;
		static decimal[] Balances = { 15000m, 8500m, 32000m };
		static string[] History = new string[2_000_000_000];
		static bool AdminPanelCanShow = true;
		static int ActionChoice = -1;
		static int ExitConfirm = -1;

		static void Main()
		{
			// Window settings
			Console.Title = "ATM";
			Console.ForegroundColor = ConsoleColor.White;

			string TimeOfDay = "";
			if (DateTime.Now.Hour >= 5 && DateTime.Now.Hour < 11) TimeOfDay = "morning";
			else if (DateTime.Now.Hour >= 11 && DateTime.Now.Hour < 17) TimeOfDay = "afternoon";
			else if (DateTime.Now.Hour >= 17 && DateTime.Now.Hour < 23) TimeOfDay = "evening";
			else TimeOfDay = "nigt";

			// Main program loop
			while (ActionChoice != 0 && ExitConfirm != 1)
			{
				// Start program
				Console.Clear();
				Console.WriteLine($"===== Good {TimeOfDay}, {AccountNames[AccountChoice]}! =====");
				Console.WriteLine($"Your balance: {Balances[AccountChoice]}");
				Console.WriteLine();
				Console.WriteLine("Choose the action:");
				Console.WriteLine("1. Withdrawn");
				Console.WriteLine("2. Top up");
				Console.WriteLine("3. Transfer between accounts");
				Console.WriteLine("4. Operation hystory");
				Console.WriteLine("0. Exit");
				Console.WriteLine();
				Console.WriteLine("====================");
				Console.Write("Your choice: ");

				// Value validity check
				if (!int.TryParse(Console.ReadLine(), out ActionChoice))
				{
					ActionChoice = -1;
					ErrorShow("Invalid input.");
					continue;
				}
				if ((ActionChoice < 0 || ActionChoice > 4) && ActionChoice != 1488) ErrorShow("Invalid input.");
				Console.Clear();
				if (ActionChoice == 1) Withdrawn();
				else if (ActionChoice == 2) TopUp();
				else if (ActionChoice == 3) TransferBetweenAccouts();
				else if (ActionChoice == 4) OperationHistory();
				else if (ActionChoice == 0) Exit();
				else if (ActionChoice == 1488)
				{
					if (AdminPanelCanShow) AdminPanel();
					else ErrorShow("Invalid input.");
				}
			}
		}

		static void Withdrawn()
		{
            Console.WriteLine("===== WITHDRAWN =====");
			Console.WriteLine($"Balance: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.Write("Enter the withdrawal amount (the amount must be a multiple of 100): ");

            // Value validity check
            if (!decimal.TryParse(Console.ReadLine(), out decimal WithdrawnAmount))
			{
				ErrorShow("Invalid input.");
				return;
			}
			if (WithdrawnAmount < 0)
			{
				ErrorShow("You cannot withdraw a negative amount..");
				return;
			}
			if (WithdrawnAmount % 100 != 0)
			{
				ErrorShow("The amount must be a multiple of 100.");
				return;
			}
			if (WithdrawnAmount > Balances[AccountChoice])
			{
				ErrorShow("Insufficient funds.");
				return;
			}

            // Successful cash withdrawal
            Balances[AccountChoice] -= WithdrawnAmount;
			Console.Clear();
            Console.WriteLine("===== WITHDRAWN =====");
            Console.WriteLine($"Balance: {Balances[AccountChoice]}");
            Console.WriteLine();
            Console.WriteLine($"Enter the withdrawal amount (the amount must be a multiple of 100): {WithdrawnAmount}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"You have successfully withdrawn {WithdrawnAmount}.");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Press Enter to continue...");
			Console.ReadKey();

			// Record operation in history
			for (int i = 0; i < History.Length; i++)
			{
				if (History[i] == null)
				{
                    History[i] = $"[{DateTime.Now.ToLongTimeString()}] Withdrawn -{WithdrawnAmount},00. (balance of funds: {Balances[AccountChoice]})";
                    break;
				}
			}
		}

		static void TopUp()
		{
            Console.WriteLine("===== TOP UP =====");
			Console.WriteLine($"Balance: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.Write("Enter the amount to be topped up: ");

            // Value validity check
            if (!decimal.TryParse(Console.ReadLine(), out decimal TopUpAmount))
			{
				ErrorShow("Invalid input.");
				return;
			}
			if (TopUpAmount < 0)
			{
				ErrorShow("You can't top up the account for an amount less than 0.");
				return;
			}

            // Successful account top up
            Balances[AccountChoice] += TopUpAmount;
			Console.Clear();
			Console.WriteLine("===== TOP UP =====");
			Console.WriteLine($"Balance: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine($"Enter the amount to be topped up: {TopUpAmount}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"You have successfully topped up your account by {TopUpAmount}.");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Press Enter to continue...");
			Console.ReadKey();

            // Record operation in history
            for (int i = 0; i < History.Length; i++)
			{
				if (History[i] == null)
				{
					History[i] = $"[{DateTime.Now.ToLongTimeString()}] Top Up +{TopUpAmount},00. (balance of funds: {Balances[AccountChoice]})";
					break;
				}
			}
		}

		static void TransferBetweenAccouts()
		{
			Console.WriteLine("===== TRANSFER BETWEEN ACCOUNTS =====");
			Console.WriteLine($"Balance: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine("Select the account for the transfer:");

            // Display of all accounts except the current one.
            for (int i = 0; i < AccountNames.Length; i++) if (i != AccountChoice) Console.WriteLine($"{i + 1}. {AccountNames[i]}");
			Console.WriteLine("0. Exit");
			Console.WriteLine();

            // Entering values and checking their validity
            Console.Write("Your choice: ");
			if (!int.TryParse(Console.ReadLine(), out int TransferAccount))
			{
				ErrorShow("Invalid input.");
				return;
			}
			TransferAccount--;
			if (TransferAccount < -1 || TransferAccount > 2)
			{
				ErrorShow("Such an account doesn't exist.");
				return;
			}
			if (TransferAccount == AccountChoice)
			{
				ErrorShow("You can't transfer funds to the same account.");
				return;
			}
			Console.Write("Enter the transfer amount: ");
			if (!decimal.TryParse(Console.ReadLine(), out decimal TransferSum))
			{
				ErrorShow("Invalid input.");
				return;
			}
			if (TransferSum < 0)
			{
				ErrorShow("You can't transfer a negative amount.");
				return;
			}
			if (TransferSum > Balances[AccountChoice])
			{
				ErrorShow("Insufficient funds.");
				return;
			}

            // Successful transfer between accounts
            Balances[AccountChoice] -= TransferSum;
			Balances[TransferAccount] += TransferSum;
			Console.Clear();
			Console.WriteLine("===== TRANSFER BETWEEN ACCOUNTS =====");
			Console.WriteLine($"Balance: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine("Select the account for the transfer:");
			for (int i = 0; i < AccountNames.Length; i++) if (i != AccountChoice) Console.WriteLine($"{i + 1}. {AccountNames[i]}");
			Console.WriteLine();
			Console.WriteLine($"Your choice: {TransferAccount + 1}");
			Console.WriteLine($"Enter the transfer amount: {TransferSum}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"You have successfully transferred {TransferSum} to the account \"{AccountNames[TransferAccount]}\".");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Press Enter to continue...");
			Console.ReadKey();

            // Record operation in history
            for (int i = 0; i < 256; i++)
			{
				if (History[i] == null)
				{
					History[i] = $"[{DateTime.Now.ToLongTimeString()}] Transfer -{TransferSum},00. --> {AccountNames[TransferAccount]} (balance of funds: {Balances[AccountChoice]})";
					break;
				}
			}
		}

		static void OperationHistory() // исправить
		{
			Console.WriteLine("===== OPERATION HISTORY =====");
			if (History[0] == null) ErrorShow("There are no operations yet..");

            /*{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine();
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.Write("Press Enter to continue...");
				Console.ReadKey();
				return;
			}*/
            else
            {
				for (int i = 0; i < History.Length; i++)
				{
					if (History[i] == null) break;
					Console.WriteLine($"{i + 1}. {History[i]}");
				}
			}
			Console.WriteLine();
			Console.Write("Press Enter to continue...");
			Console.ReadKey();
		}

        static void ErrorShow(string ErrorMessage) // добавить параметр действия
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Error! ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(ErrorMessage);
            Console.WriteLine();
            Console.Write("Press Enter to start over...");
            Console.ReadKey();
        }

        static void AdminPanel()
		{
			bool AdminPanelIsEisabled = false;
			string AdminPanelLabel = "===== ADMIN PANEL ===== ";
			ConsoleColor[] Pattern = [ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue];
			int PasswordAttempts = 3;
			int AdminChoice = -1;
			string CorrectPasswordHash = "4eb1b555c4a8e71516179ea34a0828272a95944411c4109f51cf7d621ba57044";

			while (!AdminPanelIsEisabled)
			{
				for (int i = 0; i < AdminPanelLabel.Length; i++)
				{
					Console.ForegroundColor = Pattern[i % Pattern.Length];
					Console.Write(AdminPanelLabel[i]);
				}
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine("Enter 0 to exit.");
				Console.Write("Enter the password to log in: ");
				string Password = Console.ReadLine();
				Console.WriteLine();

				SHA256 sha256 = SHA256.Create();
				byte[] Bytes = Encoding.UTF8.GetBytes(Password);
				byte[] Hash = sha256.ComputeHash(Bytes);
				sha256.Dispose();
				string PasswordHash = BitConverter.ToString(Hash).ToLower().Replace("-", "");

				if (PasswordHash == CorrectPasswordHash) AdminPanelIsEisabled = true;
				else if (Password == "0")
				{
					Console.ForegroundColor = ConsoleColor.White;
					return;
				}
				else
				{
					PasswordAttempts--;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("The password is incorrect!");
					Console.ForegroundColor = ConsoleColor.Cyan;
					if (PasswordAttempts == 2) Console.Write("You have 2 attempts left.");
					else if (PasswordAttempts == 1) Console.Write("You have 1 attempts left.");
					else if (PasswordAttempts == 0)
					{
						AdminPanelCanShow = false;
						Console.ForegroundColor = ConsoleColor.White;
						return;
					}
					Console.ReadKey();
					Console.Clear();
				}
			}

			while (AdminChoice != 0)
			{
				Console.Clear();
				for (int i = 0; i < AdminPanelLabel.Length; i++)
				{
					Console.ForegroundColor = Pattern[i % Pattern.Length];
					Console.Write(AdminPanelLabel[i]);
				}
				Console.WriteLine();
				Console.WriteLine();
                Console.WriteLine("Choose the action:");
                Console.WriteLine("1. Clearing the operation history");
				Console.WriteLine("2. Balance change");
				Console.WriteLine("0. Exit");
				Console.WriteLine();
				Console.WriteLine("====================");
				Console.Write("Your choice: ");
				AdminChoice = Convert.ToInt32(Console.ReadLine());
				Console.Clear();
				for (int i = 0; i < AdminPanelLabel.Length; i++)
				{
					Console.ForegroundColor = Pattern[i % Pattern.Length];
					Console.Write(AdminPanelLabel[i]);
				}
				Console.WriteLine();
				Console.WriteLine();

                // Clearing the operation history
                if (AdminChoice == 1)
				{
					int OperationHistory;
					Console.WriteLine("Clearing the operation history:");
					Console.WriteLine();
					Console.WriteLine("Are you sure you want to clear the operation history?");
					Console.WriteLine("1 - yes");
					Console.WriteLine("0 - no");
					Console.WriteLine();
					Console.WriteLine("====================");
					Console.Write("Your choice: ");
					OperationHistory = Convert.ToInt32(Console.ReadLine());
					if (OperationHistory == 0) continue;
					else
					{
						if (History[0] == null)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write("Error! ");
							Console.ForegroundColor = ConsoleColor.White;
							Console.Write("There are no operations.");
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else
						{
							for (int i = 0; i < History.Length; i++)
							{
								if (History[i] != null) History[i] = null;
								else break;
							}
						}
					}
					Console.ReadKey();
				}

                // Balance change
                if (AdminChoice == 2)
				{
					int BalanceChangeAccount, NewBalance;

					Console.WriteLine("Balance change:");
					Console.WriteLine();
					Console.WriteLine("Select an account for balance change:");
					for (int i = 0; i < AccountNames.Length; i++)
					{
						if (i == 0) Console.WriteLine($"{i + 1}. {AccountNames[i]} \t\t {Balances[i]}");
						else Console.WriteLine($"{i + 1}. {AccountNames[i]} \t {Balances[i]}");
					}
					Console.WriteLine("0. Exit");
					Console.WriteLine();
					Console.WriteLine("====================");
					Console.Write("Your choice: ");
					BalanceChangeAccount = Convert.ToInt32(Console.ReadLine());
					if (BalanceChangeAccount == 0) continue;
					Console.WriteLine();
					BalanceChangeAccount--;
					Console.Write($"Enter the new balance for the account. \"{AccountNames[BalanceChangeAccount]}\": ");
					NewBalance = Convert.ToInt32(Console.ReadLine());
					Console.WriteLine();
					Balances[BalanceChangeAccount] = NewBalance;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($"You have successfully set up the balance in {NewBalance} for the account. \"{AccountNames[BalanceChangeAccount]}\".");
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.ReadKey();
				}
			}
			Console.ForegroundColor = ConsoleColor.White;
			return;
		}

		static void Exit()
		{
			Console.WriteLine("Are you sure you want to leave?");
			Console.WriteLine("1 - yes");
			Console.WriteLine("0 - no");
			Console.WriteLine();
			Console.WriteLine("====================");
			Console.Write("Your choice: ");
			if (!int.TryParse(Console.ReadLine(), out ExitConfirm) || ExitConfirm < 0 || ExitConfirm > 1) ErrorShow("Invalid input.");
			else if (ExitConfirm == 1) return;
			else
			{
				ActionChoice = -1;
				ExitConfirm = -1;
			}
		}
	}
}