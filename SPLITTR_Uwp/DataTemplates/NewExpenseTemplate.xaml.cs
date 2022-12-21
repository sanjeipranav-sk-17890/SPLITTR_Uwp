using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SPLITTR_Uwp.ViewModel.Models;
using System.ServiceModel.Channels;
using Windows.UI;
using Microsoft.Toolkit.Uwp.UI;
using SPLITTR_Uwp.Core.ExtensionMethod;
using SPLITTR_Uwp.Core.ModelBobj;
using SPLITTR_Uwp.Core.Utility;
using Color = Windows.UI.Color;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace SPLITTR_Uwp.DataTemplates
{
    public sealed partial class NewExpenseTemplate : UserControl,INotifyPropertyChanged
    {
        
        private SolidColorBrush _expenseTextBoxColor;
        
        private SolidColorBrush _parsingSuccessColor = new SolidColorBrush(Color.FromArgb(255, 34, 139, 34));

        private SolidColorBrush _parsingFailedColor = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));

        private ExpenseBobj ExpenseObj
        {
            get => DataContext as ExpenseBobj;
        }

        private SolidColorBrush ExpenseTextBoxColor
        {
            get => _expenseTextBoxColor;
            set => SetField(ref _expenseTextBoxColor, value);
        }

        private string UserInitial
        {
            get
            {
                var initial = ExpenseObj?.UserDetails.UserName.GetUserInitial();
                return initial ?? "_";
            }
        }
        

        public NewExpenseTemplate()
        {
            this.InitializeComponent();
            this.DataContextChanged += (s, e) =>
            {
                Bindings.Update();
                OnPropertyChanged(nameof(UserInitial));
            };
            
            ExpenseTextBoxColor = _parsingSuccessColor;

        }

        
        bool _isInternalTextChange = true;

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
                ExpenseTextBoxColor = _parsingSuccessColor ;
                ExpenseObj.ExpenseAmount = expenseAmount;
            }
            else// if parsing failed default 0.0 will only be assigned
            {
                ExpenseTextBoxColor = _parsingFailedColor;

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
}
