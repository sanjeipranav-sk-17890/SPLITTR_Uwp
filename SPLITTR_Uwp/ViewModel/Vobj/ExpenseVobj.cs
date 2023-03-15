using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.ModelBobj.Enum;
using SPLITTR_Uwp.Core.SplittrEventArgs;
using SPLITTR_Uwp.Core.SplittrNotifications;
using SPLITTR_Uwp.Services;

namespace SPLITTR_Uwp.ViewModel.Vobj;

public class ExpenseVobj : ExpenseBobj,INotifyPropertyChanged
{
       
    private bool _visibility = true;
    private string _iconSource;
    private string _categoryName;
   

    public string IconSource
    {
        get => _iconSource;
        set
        {
            if (value == _iconSource)
                return;
            _iconSource = value;
            OnPropertyChanged();
        }
    }

    public string CategoryName
    {
        get => _categoryName;
        set
        {
            if (value == _categoryName)
                return;
            _categoryName = value;
            OnPropertyChanged();
        }
    }

    public override string Note
    {
        get => base.Note;
        set
        {
            if (value == base.Note)
                return;
            base.Note = value;
            OnPropertyChanged();
        }
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

    public override  ExpenseStatus ExpenseStatus
    {
        get => base.ExpenseStatus;
        set
        {
            if (base.ExpenseStatus == value)
            {
                return;
            }
            base.ExpenseStatus = value;
            OnPropertyChanged();
        } 
    }

    public override DateTime DateOfExpense
    {
        get => base.DateOfExpense;
        set
        {
            if (value.Equals(base.DateOfExpense))
                return;
            base.DateOfExpense = value;
            OnPropertyChanged();
        }
    }

    public override string Description
    {
        get => base.Description;
        set
        {
            if (value == base.Description)
                return;
            base.Description = value;
            OnPropertyChanged();
        }
    }

    public ExpenseVobj(ExpenseBobj expense) : base(expense)
    {
        SplittrNotification.ExpenseStatusChanged += SplittrNotification_ExpenseStatusChanged;
    }

    ~ExpenseVobj()
    {
        SplittrNotification.ExpenseStatusChanged -= SplittrNotification_ExpenseStatusChanged;
    }

    private void SplittrNotification_ExpenseStatusChanged(ExpenseStatusChangedEventArgs obj)
    {
        if (obj?.StatusChangedExpense?.ExpenseUniqueId.Equals(ExpenseUniqueId) is not true)
        {
            return;
        }
        ExpenseStatus = obj.StatusChangedExpense.ExpenseStatus;
        CategoryId = obj.StatusChangedExpense.CategoryId;
    }

    #region INotifyPropertyChanged Region

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        _ = UiService.RunOnUiThread(() =>
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        });
    }

    #endregion

}