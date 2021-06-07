using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Controls;


namespace WarehouseHelper.VeiwModel
{
    public class SlabVeiwModel : INotifyPropertyChanged
    {
        StoneСompanyContext db;
        public ArrayList PreviewWarehouseSlabs { get; set; } = new ArrayList();
        public ObservableCollection<Slab> SawingStone { get; set; } = new ObservableCollection<Slab>();
        public List<Stone> WarehouseStones { get; set; } = new List<Stone>();
        public Worker Worker { get; set; } = new Worker();
        public Stone SelectedStone { get; set; }

        private RelayCommand addSlabsCommand;
        public RelayCommand AddSlabsCommand
        {
            get
            {
                return addSlabsCommand ?? (addSlabsCommand = new RelayCommand(obj =>
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.QueueDeclare(queue: "slab",
                                                durable: true,
                                                exclusive: false,
                                                autoDelete: false,
                                                arguments: null);

                            int totalCount = 0;
                            foreach (var partStone in SawingStone)
                            {
                                totalCount++;
                                var slab = new FormattedSlab()
                                {
                                    SlabId = SelectedStone.StoneId + "/" + totalCount,
                                    Width = partStone.Width,
                                    Height = partStone.Height,
                                    Length = partStone.Length,
                                    Shift = true,
                                    Date = Worker.Date,
                                    Name = Worker.Name,
                                };

                                var json = JsonConvert.SerializeObject(slab);
                                var body = Encoding.UTF8.GetBytes(json);

                                var properties = channel.CreateBasicProperties();
                                properties.Persistent = true;

                                channel.BasicPublish(exchange: "",
                                                     routingKey: "slab",
                                                     basicProperties: properties,
                                                     body: body);
                            }
                        }
                    }


                    /* Добавление на склад */
                    //int totalCount = 0;
                    //foreach (var slab in SawingStones)
                    //{
                    //    for (int i = 1; i <= slab.Count; i++)
                    //    {
                    //        totalCount++;
                    //        db.Slabs.Add(new Slab()
                    //        {
                    //            SlabId = SelectedStone.StoneId + "/" + totalCount,
                    //            Height = slab.Height,
                    //            Width = slab.Width,
                    //            Length = slab.Length,
                    //            Shift = slab.Shift,
                    //            Worker = Worker.Name,
                    //            Date = Worker.Date
                    //        });
                    //    }
                    //}

                    //db.SaveChanges();
                }));
            }
        }

        public SlabVeiwModel()
        {
            db = new StoneСompanyContext();
            db.Stones.Load();
            db.Slabs.Load();

            /* Отображение содержимого склада камней */
            WarehouseStones = db.Stones.Local.ToList();

            foreach (var slab in db.Slabs)
            {
                /* Вид слэба (камня) */

                var type = (from m in db.Stones.Local.ToArray()
                            where (m.StoneId == slab.SlabId.Split(new char[] { '/' })[0])
                            select m.Type).First();

                /* Отображение содержимого склада слэбов */
                if (slab.Processing != null)
                    PreviewWarehouseSlabs.Add(new
                    {
                        Date = slab.Date,
                        Shift = slab.Shift ? "День" : "Ночь",
                        Worker = slab.Worker,
                        StoneId = slab.SlabId.Split(new char[] { '/' })[0],
                        Width = slab.Width,
                        Height = slab.Height,
                        Length = slab.Length,
                        Square = slab.Width * slab.Length,
                        Type = type
                    });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }


    public class Worker
    {
        public DateTime Date { get; set; } = DateTime.Now.Date;
        public TextBlock Shift { get; set; }
        public string Name { get; set; }
    }

    public partial class FormattedSlab
    {
        public string SlabId { get; set; }
        public string Worker { get; set; }
        public bool Shift { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Length { get; set; }
        public DateTime Date { get; set; }
        public string Processing { get; set; }

        [NotMapped]
        public int Count { get; set; }
        [NotMapped]
        public string Name { get; set; }
    }

}