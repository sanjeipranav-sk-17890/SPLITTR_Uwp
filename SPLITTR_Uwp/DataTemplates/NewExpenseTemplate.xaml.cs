using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates;

public sealed partial class NewExpenseTemplate : UserControl
{

    private readonly SolidColorBrush _parsingSuccessColor = new SolidColorBrush(Color.FromArgb(255, 34, 139, 34));

    private readonly SolidColorBrush _parsingFailedColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

    private ExpenseBobj ExpenseObj
    {
        get => DataContext as ExpenseBobj;
    }


    private string UserInitial
    {
        get
        {
            var initial = ExpenseObj?.CorrespondingUserObj.UserName.GetUserInitial();
            return initial ?? "_";
        }
    }
        

    public NewExpenseTemplate()
    {
        InitializeComponent();
        DataContextChanged += (s, e) =>
        {
            Bindings.Update();
            UserInitialProfilePicture.Initials = UserInitial;
        };

        ExpenseAmountTextBox.BorderBrush = _parsingSuccessColor;

    }


    private bool _isInternalTextChange = true;

    private void ExpenseAmountTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isInternalTextChange)
        {
            _isInternalTextChange = !_isInternalTextChange;
            return;
        }

        var expenseInputTextBox = sender as TextBox;

        var expenseAmountText = expenseInputTextBox?.Text;

        if (double.TryParse(expenseAmountText, out var expenseAmount))//if parsing success assign to expense obj
        {
            ExpenseAmountTextBox.BorderBrush = _parsingSuccessColor ;
            ExpenseObj.StrExpenseAmount = expenseAmount;
        }
        else// if parsing failed default 0.0 will only be assigned
        {
            ExpenseAmountTextBox.BorderBrush = _parsingFailedColor;

            if (expenseInputTextBox != null)//parsing failed reset text feild to default
            {
                expenseInputTextBox.Text = "0.0";
                _isInternalTextChange = true;
            }

        }

    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}