using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Graphics.DirectX.Direct3D11;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel.Models
{
    public class ExpenseViewModel : ExpenseBobj,INotifyPropertyChanged
    {
        private readonly ExpenseBobj _expense;
        private bool _visibility = true;

        [System.Diagnostics.DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override  string Note
        {
            get => _expense.Note;
        }

        [System.Diagnostics.DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override double ExpenseAmount
        {
            get => _expense.StrExpenseAmount;
           
        }

        [System.Diagnostics.DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override  ExpenseStatus ExpenseStatus
        {
            get => _expense.ExpenseStatus;
        }

        public bool Visibility
        {
            get => _visibility ;
            set
            {
                if (value == _visibility)
                    return;
                _visibility = value;
                OnPropertyChanged();
            }
        }


        public ExpenseViewModel(ExpenseBobj expense) : base(expense)
        {
            _expense = expense;
            _expense.ValueChanged += InnerObjValueChanged;
        }



        public void InnerObjValueChanged(string property)
        {
           OnPropertyChanged(property);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected async virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
          await UiService.RunOnUiThread((() =>
            {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }));
        }


    }
}
