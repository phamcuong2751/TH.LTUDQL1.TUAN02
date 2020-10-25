using Aspose.Cells;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace TH.LTUDQL1.TUAN02
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

        private void ButtonClick_ImportExcel(object sender, RoutedEventArgs e)
        {
            var Screen = new OpenFileDialog();
            string[] Column = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K" };
            if (Screen.ShowDialog() == true)
            {
                var FileExcel = new Workbook(Screen.FileName);
                var Sheets = FileExcel.Worksheets;
                var db = new MyStoreEntities();
                var _id = 1;

                foreach (var Sheet in Sheets)
                {
                    Debug.WriteLine(Sheet.Name);
                    var i = 2;

                    var Row = 3;
                    var Cell = Sheet.Cells[$"C3"];
                    var caterogy = new Category()
                    {
                        Id = _id,
                        Name = Sheet.Name
                    };
                    db.Categories.Add(caterogy);
                    db.SaveChanges();
                    _id++;

                    while (Cell.Value != null)
                    {
                        i = 2;
                        var sku = Sheet.Cells[$"{Column[i]}{Row}"].StringValue; i++;
                        var name = Sheet.Cells[$"{Column[i]}{Row}"].StringValue; i++;
                        var price = Sheet.Cells[$"{Column[i]}{Row}"].IntValue; i++;
                        var quality = Sheet.Cells[$"{Column[i]}{Row}"].IntValue; i++;
                        var description = Sheet.Cells[$"{Column[i]}{Row}"].StringValue; i++;
                        var image = Sheet.Cells[$"{Column[i]}{Row}"].StringValue; i++;

                        var product = new Product()
                        {
                            SKU = sku,
                            Name = name,
                            Price = price,
                            Quantity = quality,
                            Description = description,
                            Image = image
                        };

                        caterogy.Products.Add(product);
                        db.SaveChanges();


                        Debug.WriteLine($"{sku} - {name} - {price} - {quality} \n");
                        Row++;
                        Cell = Sheet.Cells[$"B{Row}"];
                    }

                }
                MessageBox.Show("Import Excel succesful!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                var products = db.Products.ToArray();
                loadProduct.ItemsSource = products;
            }

        }

        private void ButtonClick_UploadImage(object sender, RoutedEventArgs e)
        {
            var Screen = new OpenFileDialog();
            var Count = 0;
            if (Screen.ShowDialog() == true)
            {
                var FileName = Screen.FileName;
                var MyStr = FileName;
                string[] Substring = MyStr.Split('\\');
                var NameImage = "";
                foreach (var Str in Substring)
                    NameImage = Str;

                var Image = new BitmapImage(new Uri(FileName, UriKind.Absolute));
                var Encoder = new JpegBitmapEncoder();
                Encoder.Frames.Add(BitmapFrame.Create(Image));
                using (var Stream = new MemoryStream())
                {
                    Encoder.Save(Stream);

                    var Photo = new Photo()
                    {
                        ImageBinary = Stream.ToArray(),
                        FileImageName = NameImage
                    };
                    var db = new MyStoreEntities();
                    var PhotoFromDB = (from pt in db.Photos select new { pt.FileImageName }).ToArray();
                    foreach (var img in PhotoFromDB)
                    {
                        //Check if the image is the same with the rest of the images in the database
                        string temp = img.ToString(); //temp = "{FileNameImage = "asus01.jpg"}"
                        var position = temp.IndexOf("=");
                        var length = temp.Length;
                        var temp2 = temp.Substring(position + 2, length - 20); // Cut string get file name of image
                        if (String.Compare(temp2, NameImage, true) == 0)
                            Count++;
                    }
                    if (Count == 0)
                    {
                        db.Photos.Add(Photo);
                        db.SaveChanges();
                    }
                }
                if (Count > 0)
                    MessageBox.Show("The image could not be added, because the product image already exists!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                else
                    MessageBox.Show("Image has been added successfully to database!");
            }
            var database = new MyStoreEntities();
            var photos = (from img in database.Photos
                          join prodc in database.Products on img.FileImageName equals prodc.Image
                          select new { img.ImageBinary, prodc.Name, prodc.Quantity, prodc.Price }).ToArray();
            loadImage.ItemsSource = photos;
        }

        private void ButtonClick_LoadImage(object sender, RoutedEventArgs e)
        {
            var db = new MyStoreEntities();
            var photos = (from img in db.Photos
                          join prodc in db.Products on img.FileImageName equals prodc.Image
                          select new { img.ImageBinary, prodc.Name, prodc.Quantity, prodc.Price }).ToArray();
            loadImage.ItemsSource = photos;
        }

        private void ButtonClick_LoadData(object sender, RoutedEventArgs e)
        {
            var db = new MyStoreEntities();
            var products = db.Products.ToArray();
            loadProduct.ItemsSource = products;
        }
    }
}
