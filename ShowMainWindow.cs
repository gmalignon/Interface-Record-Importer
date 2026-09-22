using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace InterfaceRecordImporter.Commands
{
    public class ShowMainWindow 
    {
        public  void Execute(object parameter)
        {
            ((MainWindow)Application.Current.MainWindow).Show();
            CommandManager.InvalidateRequerySuggested();
        }


        public  bool CanExecute(object parameter)
        {
            Window win = ((MainWindow)Application.Current.MainWindow);
            return win != null && !win.IsVisible;
        }
    }
}
