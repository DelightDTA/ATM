using System.Text;
using System;
using System.Text;
using SHA256 = System.Security.Cryptography.SHA256;

namespace ATM
{
	internal class Program
	{
		// Глобальные переменные
		static int AccountChoice = new Random().Next(0, 3);
		static string[] AccountNames = { "Иван Петров", "Мария Сидорова", "Алексей Козлов" } ;
		static decimal[] Balances = { 15000m, 8500m, 32000m };
		static string[] History = new string[256];
		static bool AdminPanelCanShow = true;
		static int ActionChoice = -1;
		static int ExitConfirm = -1;

		static void Main()
		{
			// Настройки окна
			Console.Title = "МОД СБЕРБАНК МНОГО ДЕНЕГ";
			Console.ForegroundColor = ConsoleColor.White;

			// Основной цикл программы
			while (ActionChoice != 0 && ExitConfirm != 1)
			{
				// Начало программы
				Console.Clear();
				Console.WriteLine($"===== Здавствуйте, {AccountNames[AccountChoice]} =====");
				Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
				Console.WriteLine();
				Console.WriteLine("Выберите действие:");
				Console.WriteLine("1. Снять наличные");
				Console.WriteLine("2. Пополнить счет");
				Console.WriteLine("3. Перевод между счетами");
				Console.WriteLine("4. История операций");
				Console.WriteLine("0. Выход");
				Console.WriteLine();
				Console.WriteLine("====================");
				Console.Write("Ваш выбор: ");

				// Проверка валидности значения
				if (!int.TryParse(Console.ReadLine(), out ActionChoice))
				{
					ActionChoice = -1;
					ErrorShow("Неверный ввод.");
					continue;
				}
				if ((ActionChoice < 0 || ActionChoice > 4) && ActionChoice != 1488) ErrorShow("Неверный ввод.");
				Console.Clear();
				if (ActionChoice == 1) GetCash();
				else if (ActionChoice == 2) TopUp();
				else if (ActionChoice == 3) TransferBetweenAccouts();
				else if (ActionChoice == 4) OperationHistory();
				else if (ActionChoice == 0) Exit();
				else if (ActionChoice == 1488)
				{
					if (AdminPanelCanShow) AdminPanel();
					else ErrorShow("Неверный ввод.");
				}
			}
		}

		static void ErrorShow(string ErrorMessage)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("Ошибка! ");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine(ErrorMessage);
			Console.WriteLine();
			Console.Write("Нажмите Enter, чтобы начать сначала...");
			Console.ReadKey();
		}

		static void GetCash()
		{
			Console.WriteLine("===== СНЯТИЕ НАЛИЧНЫХ =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.Write("Введите сумму для снятия (сумма должна быть кратна 100): ");

			// Проверка валидности значения
			if (!decimal.TryParse(Console.ReadLine(), out decimal CashSum))
			{
				ErrorShow("Неверный ввод.");
				return;
			}
			if (CashSum < 0)
			{
				ErrorShow("Вы не можете снять отрицательную сумму.");
				return;
			}
			if (CashSum % 100 != 0)
			{
				ErrorShow("Сумма должна быть кратна 100.");
				return;
			}
			if (CashSum > Balances[AccountChoice])
			{
				ErrorShow("Недостаточно средств.");
				return;
			}

			// Успешное снятие наличных
			Balances[AccountChoice] -= CashSum;
			Console.Clear();
			Console.WriteLine("===== СНЯТИЕ НАЛИЧНЫХ =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine($"Введите сумму для снятия (сумма должна быть кратна 100): {CashSum}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Вы успешно сняли {CashSum} рублей.");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Нажмите Enter для продолжения...");
			Console.ReadKey();

			// Запись операции в историю
			for (int i = 0; i < 256; i++)
			{
				if (History[i] == null)
				{
					History[i] = $"[{DateTime.Now.ToLongTimeString()}] Снятие -{CashSum},00 Р. (остаток: {Balances[AccountChoice]})";
					break;
				}
			}
		}

		static void TopUp()
		{
			decimal TopUpSum;
			Console.WriteLine("===== ПОПОЛНЕНИЕ СЧЕТА =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.Write("Введите сумму для пополнения: ");

			// Проверка валидности значения
			if (!decimal.TryParse(Console.ReadLine(), out TopUpSum))
			{
				ErrorShow("Неверный ввод.");
				return;
			}
			if (TopUpSum < 0)
			{
				ErrorShow("Вы не можете пополнить счет на сумму меньше чем 0 рублей.");
				return;
			}

			// Успешное пополнение счета
			Balances[AccountChoice] += TopUpSum;
			Console.Clear();
			Console.WriteLine("===== ПОПОЛНЕНИЕ СЧЕТА =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine($"Введите сумму для пополнения: {TopUpSum}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Вы успешно пополнили счет на {TopUpSum} рублей.");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Нажмите Enter для продолжения...");
			Console.ReadKey();

			// Запись операции в историю
			for (int i = 0; i < 256; i++)
			{
				if (History[i] == null)
				{
					History[i] = $"[{DateTime.Now.ToLongTimeString()}] Пополнение +{TopUpSum},00 Р. (остаток: {Balances[AccountChoice]})";
					break;
				}
			}
		}

		static void TransferBetweenAccouts()
		{
			int TransferAccount;
			decimal TransferSum;
			Console.WriteLine("===== ПЕРЕВОД МЕЖДУ СЧЕТАМИ =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine("Выберите счет для перевода:");

			// Показ всех счетов, кроме текущего
			for (int i = 0; i < 3; i++)
			{
				if (i != AccountChoice) Console.WriteLine($"{i + 1}. {AccountNames[i]}");
			}
			Console.WriteLine("0. Выход");
			Console.WriteLine();

			// Ввод значений и проверка их валидности
			Console.Write("Ваш выбор: ");
			if (!int.TryParse(Console.ReadLine(), out TransferAccount))
			{
				ErrorShow("Неверный ввод.");
				return;
			}
			TransferAccount--;
			if (TransferAccount == AccountChoice)
			{
				ErrorShow("Вы не можете перевести средства на тот же счет.");
				return;
			}
			if ((TransferAccount < -1 || TransferAccount > 2))
			{
				ErrorShow("Такого счета не существует.");
				return;
			}
			if (TransferAccount == -1) return;
			Console.Write("Введите сумму для перевода: ");
			if (!decimal.TryParse(Console.ReadLine(), out TransferSum))
			{
				ErrorShow("Неверный ввод.");
				return;
			}
			if (TransferSum < 0)
			{
				ErrorShow("Вы не можете перевести отрицательную сумму.");
				return;
			}
			if (TransferSum > Balances[AccountChoice])
			{
				ErrorShow("Недостаточно средств.");
				return;
			}

			// Успешный перевод между счетами
			Balances[AccountChoice] -= TransferSum;
			Balances[TransferAccount] += TransferSum;
			Console.Clear();
			Console.WriteLine("===== ПЕРЕВОД МЕЖДУ СЧЕТАМИ =====");
			Console.WriteLine($"Баланс: {Balances[AccountChoice]}");
			Console.WriteLine();
			Console.WriteLine("Выберите счет для перевода:");
			for (int i = 0; i < 3; i++)
			{
				if (i != AccountChoice) Console.WriteLine($"{i + 1}. {AccountNames[i]}");
			}
			Console.WriteLine();
			Console.WriteLine($"Ваш выбор: {TransferAccount + 1}");
			Console.WriteLine($"Введите сумму для перевода: {TransferSum}");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine($"Вы успешно перевели {TransferSum} на счет \"{AccountNames[TransferAccount]}\".");
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine();
			Console.Write("Нажмите Enter для продолжения...");
			Console.ReadKey();

			// Запись операции в историю
			for (int i = 0; i < 256; i++)
			{
				if (History[i] == null)
				{
					History[i] = $"[{DateTime.Now.ToLongTimeString()}] Перевод -{TransferSum},00 Р. --> {AccountNames[TransferAccount]} (остаток: {Balances[AccountChoice]})";
					break;
				}
			}
		}

		static void OperationHistory()
		{
			Console.WriteLine("===== ИСТОРИЯ ОПЕРАЦИЙ =====");
			if (History[0] == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Операций пока нет.");
				Console.ForegroundColor = ConsoleColor.White;
				Console.WriteLine();
				Console.Write("Нажмите Enter для продолжения...");
				Console.ReadKey();
				return;
			}
			else
			{
				for (int i = 0; i < 256; i++)
				{
					if (History[i] == null) break;
					Console.WriteLine($"{i + 1}. {History[i]}");
				}
			}
			Console.WriteLine();
			Console.Write("Нажмите Enter для продолжения...");
			Console.ReadKey();
		}

		static void AdminPanel()
		{
			bool AdminPanelDisabled = true;
			string AdminPanelLabel = "===== АДМИН-ПАНЕЛЬ =====";
			ConsoleColor[] Pattern = [ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Green, ConsoleColor.Cyan, ConsoleColor.Blue];
			int PasswordAttempts = 3;
			int AdminChoice = -1;
			string CorrectPasswordHash = "4eb1b555c4a8e71516179ea34a0828272a95944411c4109f51cf7d621ba57044";

			while (AdminPanelDisabled)
			{
				for (int i = 0; i < AdminPanelLabel.Length; i++)
				{
					Console.ForegroundColor = Pattern[i % Pattern.Length];
					Console.Write(AdminPanelLabel[i]);
				}
				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine("Введите 0 для выхода.");
				Console.Write("Введите пароль для входа: ");
				string Password = Console.ReadLine();
				Console.WriteLine();

				SHA256 sha256 = SHA256.Create();
				byte[] Bytes = Encoding.UTF8.GetBytes(Password);
				byte[] Hash = sha256.ComputeHash(Bytes);
				sha256.Dispose();
				string PasswordHash = BitConverter.ToString(Hash).ToLower().Replace("-", "");

				if (PasswordHash == CorrectPasswordHash) AdminPanelDisabled = false;
				else if (Password == "0")
				{
					Console.ForegroundColor = ConsoleColor.White;
					return;
				}
				else
				{
					PasswordAttempts--;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine("Пароль неправильный!");
					Console.ForegroundColor = ConsoleColor.Cyan;
					if (PasswordAttempts == 2) Console.Write("У вас осталось 2 попытки.");
					else if (PasswordAttempts == 1) Console.Write("У вас осталось 1 попытка.");
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
				Console.WriteLine("Выберите действие:");
				Console.WriteLine("1. Очистка истории операций");
				Console.WriteLine("2. Изменение баланса");
				Console.WriteLine("0. Выход");
				Console.WriteLine();
				Console.WriteLine("====================");
				Console.Write("Ваш выбор: ");
				AdminChoice = Convert.ToInt32(Console.ReadLine());
				Console.Clear();
				for (int i = 0; i < AdminPanelLabel.Length; i++)
				{
					Console.ForegroundColor = Pattern[i % Pattern.Length];
					Console.Write(AdminPanelLabel[i]);
				}
				Console.WriteLine();
				Console.WriteLine();

				// Очистка истории операций
				if (AdminChoice == 1)
				{
					int OperationHistory;
					Console.WriteLine("Очистка истории операций:");
					Console.WriteLine();
					Console.WriteLine("Вы уверены, что хотите очистить историю операций?");
					Console.WriteLine("1 - да");
					Console.WriteLine("0 - нет");
					Console.WriteLine();
					Console.WriteLine("====================");
					Console.Write("Ваш выбор: ");
					OperationHistory = Convert.ToInt32(Console.ReadLine());
					if (OperationHistory == 0) continue;
					else
					{
						if (History[0] == null)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.Write("Ошибка! ");
							Console.ForegroundColor = ConsoleColor.White;
							Console.Write("Операций нет.");
							Console.ForegroundColor = ConsoleColor.Cyan;
						}
						else
						{
							for (int i = 0; i < 256; i++)
							{
								if (History[i] != null) History[i] = null;
								else break;
							}
						}
					}
					Console.ReadKey();
				}

				// Изменение баланса
				if (AdminChoice == 2)
				{
					int BalanceChangeAccount, NewBalance;

					Console.WriteLine("Изменение баланса:");
					Console.WriteLine();
					Console.WriteLine("Выберите счет для изменения баланса:");
					for (int i = 0; i < 3; i++)
					{
						if (i == 0) Console.WriteLine($"{i + 1}. {AccountNames[i]} \t\t {Balances[i]}");
						else Console.WriteLine($"{i + 1}. {AccountNames[i]} \t {Balances[i]}");
					}
					Console.WriteLine("0. Выход");
					Console.WriteLine();
					Console.WriteLine("====================");
					Console.Write("Ваш выбор: ");
					BalanceChangeAccount = Convert.ToInt32(Console.ReadLine());
					if (BalanceChangeAccount == 0) continue;
					Console.WriteLine();
					BalanceChangeAccount--;
					Console.Write($"Введите новый баланс для аккаута \"{AccountNames[BalanceChangeAccount]}\": ");
					NewBalance = Convert.ToInt32(Console.ReadLine());
					Console.WriteLine();
					Balances[BalanceChangeAccount] = NewBalance;
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write($"Вы успешно установили баланс в {NewBalance} для аккаута \"{AccountNames[BalanceChangeAccount]}\".");
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.ReadKey();
				}
			}
			Console.ForegroundColor = ConsoleColor.White;
			return;
		}

		static void Exit()
		{
			Console.WriteLine("Вы уверены, что хотите выйти?");
			Console.WriteLine("1 - да");
			Console.WriteLine("0 - нет");
			Console.WriteLine();
			Console.WriteLine("====================");
			Console.Write("Ваш выбор: ");
			if (!int.TryParse(Console.ReadLine(), out ExitConfirm) || ExitConfirm < 0 || ExitConfirm > 1) ErrorShow("Неверный ввод.");
			else if (ExitConfirm == 1) return;
			else
			{
				ActionChoice = -1;
				ExitConfirm = -1;
			}
		}
	}
}