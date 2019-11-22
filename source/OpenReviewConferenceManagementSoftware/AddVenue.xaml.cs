using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OpenReviewConferenceManagementSoftware
{
    /// <summary>
    /// Interaction logic for AddVenue.xaml
    /// </summary>
    public partial class AddVenue : Window
    {
        public AddVenue()
        {
            InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            //utils.DatabaseConnection.AddVenue();
        }
    }
}
