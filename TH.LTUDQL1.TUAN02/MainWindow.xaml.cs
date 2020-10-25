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
            }

        }

        private void ButtonClick_UploadImage(object sender, RoutedEventArgs e)
        {
            var Screen = new OpenFileDialog();
            if (Screen.ShowDialog() == true)
            {
                var FileName = Screen.FileName;
                var MyStr = FileName;
                string[] Substring = MyStr.Split('\\');
                var NameImage = "";
                foreach (var Str in Substring)
                {
                    NameImage = Str;
                }

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
                    db.Photos.Add(Photo);
                    db.SaveChanges();
                }
                MessageBox.Show("Image has been added successfully to database!");

            }
        }

        private void ButtonClick_LoadImage(object sender, RoutedEventArgs e)
        {
            var db = new MyStoreEntities();
            //var photo = db.Photos.ToArray();
            var photos = (from pt in db.Photos select new { pt.ImageBinary });
            loadImage.ItemsSource = photos.ToArray();
        }
    }
}
