using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.Models;

namespace SPLITTR_Uwp.ViewModel.Models
{
    public class ExpenseViewModel : ExpenseBobj,INotifyPropertyChanged
    {
        private readonly ExpenseBobj _expense;

        public override  string Note
        {
            get => base.Note;
            set
            {
                base.Note = value;
                OnPropertyChanged();
            }
        }

        public override double ExpenseAmount
        {
            get => _expense.ExpenseAmount;
            set
            {
                _expense.ExpenseAmount = value;
                OnPropertyChanged();
            }
        }

        public override  ExpenseStatus ExpenseStatus
        {
            get => _expense.ExpenseStatus;
        }



        public ExpenseViewModel(ExpenseBobj expense) : base(expense)
        {
            _expense = expense;
            _expense.ValueChanged += InnerObjValueChanged;
        }



        public void InnerObjValueChanged()
        {
            OnPropertyChanged(nameof(ExpenseAmount));
            OnPropertyChanged(nameof(ExpenseStatus));
            OnPropertyChanged(nameof(Note));

        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

 
    }
}
