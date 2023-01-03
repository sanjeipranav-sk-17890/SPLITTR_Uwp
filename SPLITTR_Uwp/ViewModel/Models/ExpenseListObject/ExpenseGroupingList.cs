﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;

namespace SPLITTR_Uwp.ViewModel.Models.ExpenseListObject
{
    internal class ExpenseGroupingList :ObservableCollection<ExpenseBobj>
    {
         readonly ExpenseStatus _status;

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
            return expenseGroup._status == this._status;

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

        private void Expense_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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