
namespace TimeTracker.ViewModels.ChangeTask
{
	public class ChangeTaskViewModel : ClosableViewModel
	{
		private string originalMemo;
		private string memo;

		public string Memo
		{
			get { return memo; }
			set
			{
				if (memo == value)
				{
					return;
				}

				memo = value;
				RaisePropertyChanged(() => Memo);
			}
		}

		public string ProjectName
		{
			get { return "Default"; }
		}

		protected override bool CanExecuteOk()
		{
			return base.CanExecuteOk() && !string.IsNullOrEmpty(Memo) && !string.Equals(originalMemo, Memo);
		}

		public void SetDefaultValues(string currentMemo, string projectName)
		{
			originalMemo = currentMemo;
		}
	}
}