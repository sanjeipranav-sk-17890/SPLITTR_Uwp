using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.ViewModel.Models.ExpenseListObject
{
    public class ExpenseGroupingList :ObservableCollection<ExpenseBobj>
    {
        private readonly ExpenseStatus _status;

        public ExpenseGroupHeader GroupHeader { get; }
       
        
        public ExpenseGroupingList(ExpenseStatus status, IEnumerable<ExpenseBobj> expenses)
        {
            _status = status;

            GroupHeader = new ExpenseGroupHeader(this,_status.ToString());

            foreach (var expense in expenses)
            {
                if (expense == null)
                {
                    continue;
                }
                Add(new ExpenseVobj(expense));
            }
            base.CollectionChanged += ExpenseGroupingList_CollectionChanged;

        }

        private void ExpenseGroupingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
              base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExpenseGroupHeader)));  
        }

        public override bool Equals(object obj)
        {
            if (obj is not ExpenseGroupingList expenseGroup)
            {
                return false;
            }
            return expenseGroup._status == _status;

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
    public class ExpenseGroupHeader : INotifyPropertyChanged
    {
        private readonly ObservableCollection<ExpenseBobj> _expense;

        public string GroupName { get; }   

        public ExpenseGroupHeader(ObservableCollection<ExpenseBobj> expense,string groupNAme)
        {
            GroupName = groupNAme;
            _expense = expense;
            expense.CollectionChanged += Expense_CollectionChanged;

        }

        private void Expense_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(NoOfExpenses));
        }


        public int NoOfExpenses
        {
            get => _expense.Count;
        }









        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

}
