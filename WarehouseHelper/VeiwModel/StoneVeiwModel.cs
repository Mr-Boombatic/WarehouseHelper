using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Data;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace WarehouseHelper.VeiwModel
{
    public class StoneVeiwModel
    {
        StoneСompanyContext db;
        public ArrayList StoneWarehouse { get; set; } = new ArrayList();
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();
        public Order SelectedOrder { get; set; }
        public Car Car { get; set; }
        public decimal CarCost
        {
            get { return Car.Cost; }
            set
            {
                Car.Cost = value;
                Mediator.RecountCost();
            }
        }

        Mediator Mediator;


        private RelayCommand addStonesToWarehouseCommand;
        public RelayCommand AddStonesToWarehouseCommand
        {
            get
            {
                return addStonesToWarehouseCommand ?? (addStonesToWarehouseCommand = new RelayCommand(obj =>
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "Stone",
                                                durable: false,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);



                            foreach (var order in Orders) 
                            {
                                var json = JsonConvert.SerializeObject(order);
                                var body = Encoding.UTF8.GetBytes(json);

                                channel.BasicPublish(exchange: "",
                                        routingKey: "Stone",
                                        basicProperties: null,
                                        body: body);
                            }

                            var end = Encoding.UTF8.GetBytes("END");

                            channel.BasicPublish(exchange: "",
                                    routingKey: "Stone",
                                    basicProperties: null,
                                    body: end);
                        }
                    }
                }));
            }
        }

        private RelayCommand changeCountRowCommand;
        public RelayCommand AddRowCommand
        {
            get
            {
                return (changeCountRowCommand = new RelayCommand(obj =>
                {
                    Orders.Add(new Order(Mediator));
                    Mediator.RecountCost();
                }));
            }
        }

        public RelayCommand DeleteRowCommand
        {
            get
            {
                return (changeCountRowCommand = new RelayCommand(obj =>
                {
                    Orders.Remove(SelectedOrder);
                    Mediator.RecountCost();
                }));
            }
        }

        public StoneVeiwModel()
        {
            db = new StoneСompanyContext();
            db.Stones.Load();
            db.Cars.Load();

            foreach (var stone in db.Stones.Local)
            {
                StoneWarehouse.Add(new
                {
                    StoneId = stone.StoneId,
                    Volume = stone.Length * stone.Height * stone.Width,
                    Square = stone.Length * stone.Width * 2 + stone.Length * stone.Height * 2 + stone.Width * stone.Height * 2,
                    StoneCost = stone.PricePerCube * (decimal)(stone.Length * stone.Height * stone.Width),
                    Perimeter = (stone.Length + stone.Width) * 2 + (stone.Length + stone.Height) * 2 + (stone.Width + stone.Height) * 2
                });
            }

            Car = db.Cars.Count() != 0 ? new Car() { CarId = db.Cars.OrderBy(c => c).Last().CarId + 1, Date = DateTime.Now.Date, Cost = 0 }
                : new Car() { CarId = 1, Date = DateTime.Now.Date, Cost = 0 };
            Mediator = new ManagerMediator(Car);
            Orders.Add(new Order(Mediator));
        }
    }

    public class Order : Colleague, INotifyPropertyChanged
    {

        private float width;
        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                ChangeStoneVolume();
            }
        }

        private float length;
        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                ChangeStoneVolume();
            }
        }

        private float height;
        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                ChangeStoneVolume();
            }
        }

        private float volume;
        public float Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                RecountCost();
                OnPropertyChanged("Volume");
            }
        }

        private decimal cost;
        public decimal Cost
        {
            get { return cost; }
            set
            {
                cost = value;
                OnPropertyChanged("Cost");
            }
        }

        public decimal pricePerCube;
        public decimal PricePerCube
        {
            get { return pricePerCube; }
            set
            {
                pricePerCube = value;
                RecountCost();
            }
        }
        public string Type { get; set; }
        public string StoneId { get; set; }

        private void ChangeStoneVolume()
        {
            if (Length != 0 && Height != 0 && Width != 0)
                Volume = Length * Height * Width;
        }

        public Order(Mediator mediator) : base(mediator)
        {
            mediator.AddOrderedStone(this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    abstract public class Colleague
    {
        protected Mediator mediator;

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public virtual void RecountCost(decimal carCost = 0)
        {
            mediator.RecountCost();
        }

        public virtual void AddOrderedStone(Order order)
        {
            mediator.AddOrderedStone(order);
        }
    }

    abstract public class Mediator
    {
        public abstract void RecountCost();
        public abstract void AddOrderedStone(Order ordere);
    }

    class ManagerMediator : Mediator
    {
        private List<Order> OrderedStones = new List<Order>();
        private Car Car;
        public override void RecountCost()
        {
            float overallVolume = OrderedStones.Select(stone => stone.Length * stone.Width * stone.Height).Sum();

            foreach (var stone in OrderedStones)
            {

                stone.Cost = (decimal)(stone.Volume / (overallVolume / 100)) / 100 * Car.Cost + (decimal)(stone.Length * stone.Width * stone.Height) * stone.PricePerCube;
            }
        }
        public override void AddOrderedStone(Order orderedStone)
        {
            OrderedStones.Add(orderedStone);
        }

        public ManagerMediator(Car car)
        {
            this.Car = car;
        }
    }
}

