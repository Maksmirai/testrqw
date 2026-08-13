using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media.Imaging;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Vkotk.Context;
using Vkotk.Models;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using Avalonia.Controls.ApplicationLifetimes; // Для доступа к пути приложения
namespace Vkotk;

public partial class AddProductWindow : Window
{
    public int countPhoto = 0;
    
    public static NeondbContext _context = new NeondbContext() ;
    
    public List<Imageproduct> img = new List<Imageproduct>();
    public AddProductWindow()
    { 
        AvaloniaXamlLoader.Load(this);
    }

    private void ButtonAdd_OnClick(object? sender, RoutedEventArgs e)
    {
       Product product = new Product();
       var a = this.FindControl<TextBox>("TextBoxNameProduct").Text;
       if (a != null && a != " " && a != "")
          product.Nameproduct = a;
       try
       {   
           var b = this.FindControl<TextBox>("CountProduct").Text;
           if (b == null || b == " " || b == "")
                product.Countproduct = 1 ;  
           else product.Countproduct = int.Parse(b); } 
          
       catch (Exception exception)
       {
           MessageBoxManager.GetMessageBoxStandard(
                   ButtonEnum.Ok.ToString(),
                   // Заголовок окна
                   "Количество не может быть нецелым числом или иметь буквы"
                   // Основной текст
                   ).ShowWindowDialogAsync(this);
       }
       
       var d = this.FindControl<TextBox>("Seriescode").Text;
       if (d != null && d != "" && d != " ")
           product.Codeproduct = d;
       
       var c =  this.FindControl<TextBox>("CommentProduct").Text;
       if(c == null || c == "" || c == " ")
           product.Commentproduct = "Недостатков не обнаружено";
       else
           product.Commentproduct = c;
       
       

       try
       {
           _context.Products.Add(product);
           _context.SaveChanges();
           int idprod = _context.Products.OrderByDescending(x => x.Idproduct).FirstOrDefault().Idproduct;
           if (countPhoto != 0)
           {
               List<Imageproduct> idImg = _context.Imageproducts.OrderByDescending(x => x.Idimage).Take(countPhoto).ToList();
               for (int i = 0; i < countPhoto; i++)
               {
                   Productimage image = new Productimage();
                   image.Idproduct = idprod;
                   image.Idimage = idImg[i].Idimage;
                   _context.Productimages.Add(image);
               }

               _context.SaveChanges();
           }
           
       }
       catch (Exception exception)
       {
           MessageBoxManager.GetMessageBoxStandard(
               ButtonEnum.Ok.ToString(),
               // Заголовок окна
               "Ошибка подключения к сети"
               // Основной текст
           ).ShowWindowDialogAsync(this);
       }
       
       
       MainWindow mainWindow = new MainWindow();
       mainWindow.Show();
       this.Close();
       //Сделать дату автоматичесукой в sql
    }

    private async void ButtonAddImage_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        // Настраиваем фильтр только для изображений и разрешаем множественный выбор
        var options = new FilePickerOpenOptions
        {
            Title = "Выберите изображения",
            AllowMultiple = true, // <--- Разрешаем выбор нескольких файлов!
        };
        // Открываем диалог выбора файла
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);
        if (!files.Any()) return; // Пользователь отменил выбор или не выбрал файлы
        
        NeondbContext _context = new NeondbContext();
        try
        {
            // Определяем путь к папке "Image" внутри вашего проекта
            string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string imagesFolderPath = Path.Combine(projectDirectory, "image");

            
            
            foreach (var file in files) // Обрабатываем каждый выбранный файл
            {
                // Формируем уникальное имя нового файла на основе GUID и оригинального расширения
                string originalExtension = Path.GetExtension(file.Name);
                string uniqueFilename = $"{Guid.NewGuid()}{originalExtension}";
                string destinationPath = Path.Combine(imagesFolderPath, uniqueFilename);
                
                // Копируем файл напрямую из хранилища в нашу директорию
                using (Stream source = await file.OpenReadAsync()) 
                using (Stream dest = new FileStream(destinationPath, FileMode.Create))
                {
                    await source.CopyToAsync(dest);
                }
                string[] path = destinationPath.Split("\\");
                string fileName = path.Last();
                
                // После копирования файла
                using Stream fileStream = new FileStream(destinationPath, FileMode.Open);
                fileStream.Dispose(); // Важно закрыть поток после создания Bitmap!
                
                //Console.WriteLine($"Файл успешно сохранён по адресу: {destinationPath}");
                
                Imageproduct image = new Imageproduct();
                image.Pathimage = fileName;
                img.Add(image);
                _context.Imageproducts.Add(image);
                countPhoto++;
            }
            this.FindControl<ListBox>("ListBoxImage").ItemsSource = img;
                _context.SaveChanges();
            MessageBoxManager.GetMessageBoxStandard(
                "Успех!",
                 $"Сохранено {files.Count} изображений.").ShowAsync();
        }
        catch (Exception ex)
        {
            MessageBoxManager.GetMessageBoxStandard(
                "Ошибка!",
                 $"Не удалось сохранить файлы: {ex.Message}").ShowAsync();
        }
    }
    

    private void ButtonDelImage_OnClick(object? sender, RoutedEventArgs e)
    {
        if (img.Count > 0)
        {
            try
            {
                foreach (var image in img)
                {
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"Image\\{image.Pathimage}");
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                this.FindControl<ListBox>("ListBoxImage").ItemsSource = null;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            
            
        }
        countPhoto = 0;
    }

    private void ButtonBackMenu_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}