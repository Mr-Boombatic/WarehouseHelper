﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WarehouseHelper.Veiw
{
    /// <summary>
    /// Interaction logic for CreatingSlab.xaml
    /// </summary>
    public partial class SlabWearehouse : UserControl
    {
        public SlabWearehouse()
        {
            InitializeComponent();
            DataContext = new VeiwModel.SlabVeiwModel();
        }
    }
}
