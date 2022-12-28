using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.ViewModel.Models.ExpenseListObject
{
    internal class ExpenseGroupingList :ObservableCollection<ExpenseBobj>
    {
        public readonly ExpenseStatus Status;


        public string ExpenseGroupHeader
        {
            get => $"{Status} Expenses ({Count})" ;
           
        }
        
        public ExpenseGroupingList(ExpenseStatus status, IEnumerable<ExpenseBobj> expenses)
        {
            Status = status;

            foreach (var expense in expenses)
            {
                if (expense == null)
                {
                    continue;
                }
                Add(new ExpenseViewModel(expense));
            }
            base.CollectionChanged += ExpenseGroupingList_CollectionChanged;

        }

        private void ExpenseGroupingList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
              base.OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExpenseGroupHeader)));  
        }

        public override bool Equals(object obj)
        {
            if (obj is not ExpenseGroupingList expenseGroup)
            {
                return false;
            }
            return expenseGroup.Status == this.Status;

        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


    }
}
