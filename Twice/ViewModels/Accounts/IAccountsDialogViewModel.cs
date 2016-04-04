using System.Collections.Generic;
using System.Windows.Input;

namespace Twice.ViewModels.Accounts
{
	internal interface IAccountsDialogViewModel : IDialogViewModel
	{
		ICommand MakeDefaultAccountCommand { get; }
		ICommand AddAccountCommand { get; }
		ICollection<AccountEntry> AddedAccounts { get; }
	}
}