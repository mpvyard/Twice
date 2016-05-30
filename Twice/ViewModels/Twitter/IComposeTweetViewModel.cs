using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Input;
using Twice.ViewModels.Main;

namespace Twice.ViewModels.Twitter
{
	internal interface IComposeTweetViewModel : ILoadCallback, IDialogViewModel
	{
		void PreSelectAccounts( IEnumerable<ulong> accounts );

		ICollection<AccountEntry> Accounts { get; }
		IList<MediaItem> AttachedMedias { get; }
		ICommand AttachImageCommand { get; }
		bool ConfirmationRequired { get; }
		bool ConfirmationSet { get; set; }
		ICommand DeleteMediaCommand { get; }
		bool IsSending { get; }
		ICollection<string> KnownHashtags { get; }
		ICollection<string> KnownUserNames { get; }
		bool LowCharsLeft { get; set; }
		bool MediumCharsLeft { get; set; }
		StatusViewModel QuotedTweet { [DebuggerStepThrough] get; set; }
		ICommand RemoveQuoteCommand { get; }
		ICommand SendTweetCommand { get; }
		bool StayOpen { get; set; }
		string Text { get; set; }
		int TextLength { get; set; }
	}
}