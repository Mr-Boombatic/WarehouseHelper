using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WarehouseHelper;
using WarehouseHelper.Veiw;

namespace WarehouseHelper.VeiwModel
{
    class MainWindowVeiwModel : INotifyPropertyChanged
    {
        public UserControl selectedwarehouse;
        public UserControl SelectedWarehouse
        {
            get { return selectedwarehouse; }
            set
            {
                selectedwarehouse = value;
                OnPropertyChanged("SelectedWarehouse");
            }
        }

        private RelayCommand selectWarehouse;
        public RelayCommand SelectStoneWarehouseCommand
        {
            get
            {
                return (selectWarehouse = new RelayCommand(obj =>
                {
                    SelectedWarehouse = new StoneWarehouse();
                }));
            }
        }
        public RelayCommand SelectSlabWarehouseCommand
        {
            get
            {
                return (selectWarehouse = new RelayCommand(obj =>
                {
                    SelectedWarehouse = new SlabWearehouse();
                }));
            }
        }

        public RelayCommand SelectProсessingSlabWarehouseCommand
        {
            get
            {
                return (selectWarehouse = new RelayCommand(obj =>
                {
                    SelectedWarehouse = new ProcessingSlabWarehouse();
                }));
            }
        }

        public RelayCommand SelectProductWarehouseCommand
        {
            get
            {
                return (selectWarehouse = new RelayCommand(obj =>
                {
                    SelectedWarehouse = new Veiw.Product();
                }));
            }
        }

        public RelayCommand SellCommand
        {
            get
            {
                return (selectWarehouse = new RelayCommand(obj =>
                {
                    SelectedWarehouse = new Trade();
                }));
            }
        }

        public MainWindowVeiwModel()
        {
            SelectedWarehouse = new StoneWarehouse();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
