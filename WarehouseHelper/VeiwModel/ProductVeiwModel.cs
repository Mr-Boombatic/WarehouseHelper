using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace WarehouseHelper.VeiwModel
{
    class ProductVeiwModel
    {
        StoneСompanyContext db;
        public ArrayList PreviewProductWarehouse { get; set; } = new ArrayList();
        public ArrayList ProductsSold { get; set; } = new ArrayList();
        public ObservableCollection<Slab> SlabWarehouse { get; set; } = new ObservableCollection<Slab>();
        public Product AddedProduct { get; set; } = new Product() { Date = DateTime.Now.Date };
        public Slab SelectedSlab { get; set; }

        private RelayCommand addProductCommand;
        public RelayCommand AddProductCommand
        {
            get
            {
                return addProductCommand ?? (addProductCommand = new RelayCommand(obj =>
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "product",
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);


                            var product = new Product()
                            {
                                Date = AddedProduct.Date,
                                Worker = AddedProduct.Worker,
                                Length = AddedProduct.Length,
                                Width = AddedProduct.Width,
                                Height = AddedProduct.Height,
                                SlabId = ((Slab)obj).SlabId,
                                Shift = AddedProduct.Shift,
                                Name = AddedProduct.Name,
                                ContractNumber = null
                            };

                            var json = JsonConvert.SerializeObject(product);
                            var body = Encoding.UTF8.GetBytes(json);

                            var properties = channel.CreateBasicProperties();
                            properties.Persistent = true;

                            channel.BasicPublish(exchange: "",
                                                 routingKey: "product",
                                                 basicProperties: properties,
                                                 body: body);
                        }
                    }
                    //db.Products.Add(new Product()
                    //{
                    //    Date = AddedProduct.Date,
                    //    Worker = AddedProduct.Worker,
                    //    Length = AddedProduct.Length,
                    //    Width = AddedProduct.Width,
                    //    Height = AddedProduct.Height,
                    //    SlabId = ((Slab)obj).SlabId,
                    //    Shift = AddedProduct.Shift,
                    //    Name = AddedProduct.Name,
                    //    ContractNumber = null
                    //});
                    //db.SaveChanges();
                }));
            }
        }

        public ProductVeiwModel()
        {
            db = new StoneСompanyContext();
            db.Products.Load();
            db.Slabs.Load();
            db.Stones.Load();

            foreach (var product in db.Products)
            {
                var type = (from m in db.Stones.Local.ToArray()
                            where (m.StoneId == product.SlabId.Split(new char[] { '/' })[0])
                            select m.Type).First();

                PreviewProductWarehouse.Add(new
                {
                    Type = type,
                    SlabId = product.SlabId,
                    Name = product.Name,
                    Length = product.Length.ToString(),
                    Width = product.Width.ToString(),
                    Height = product.Height.ToString(),
                    Shift = product.Shift ? "День" : "Ночь",
                    Worker = product.Worker,
                    Square = product.Length * product.Width,
                    Date = product.Date,
                    Cost = product.Cost,
                    Perimeter = product.Length * product.Width * 2 + product.Length * product.Height * 2 + product.Width * product.Height * 2
                });
            }

            SlabWarehouse = db.Slabs.Local.ToObservableCollection();
        }
    }
}
