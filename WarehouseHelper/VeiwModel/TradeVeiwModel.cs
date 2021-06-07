using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace WarehouseHelper.VeiwModel
{
    class TradeVeiwModel
    {
        StoneСompanyContext db;
        public ObservableCollection<PreviewProduct> ProductSold { get; set; } = new ObservableCollection<PreviewProduct>();
        public ObservableCollection<PreviewProduct> RemainingInStock { get; set; } = new ObservableCollection<PreviewProduct>();
        public PreviewProduct SelectedProduct { get; set; }

        private RelayCommand addForSaleCommand;
        public RelayCommand AddForSaleCommand
        {
            get
            {
                return addForSaleCommand ?? (addForSaleCommand = new RelayCommand(obj =>
                {
                    ProductSold.Add(SelectedProduct);
                    RemainingInStock.Remove(SelectedProduct);
                }));
            }
        }

        public TradeVeiwModel()
        {
            db = new StoneСompanyContext();
            db.Products.Load();

            foreach (var product in db.Products)
                RemainingInStock.Add(new PreviewProduct()
                {
                    Name = product.Name,
                    SlabId = product.SlabId,
                    Volume = product.Width * product.Length * product.Height,
                    Square = product.Width * product.Length * 2 + 2 * product.Length * product.Height+ 2 * product.Height,
                    Cost = product.Cost
                });
        }
    }

    class PreviewProduct
    {
        public string Name { get; set; }
        public string SlabId { get; set; }
        public double Volume { get; set; }
        public double Square { get; set; }
        public decimal Cost { get; set; }
    }
}
