using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LoadOnDemand_Event
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Viewtypecombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedValue = this.schedulerViewType.SelectedValue.ToString();
            if (selectedValue == "TimelineDay" || selectedValue == "TimelineWeek" || selectedValue == "TimelineWorkWeek" || selectedValue == "TimelineMonth")
            {
                this.scheduler.DisplayDate = DateTime.Now.Date;
            }
            else
                this.scheduler.DisplayDate = DateTime.Now.Date.AddHours(9);
        }
    }
}
