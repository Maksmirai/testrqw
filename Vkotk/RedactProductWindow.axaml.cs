using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Microsoft.EntityFrameworkCore;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Vkotk.Context;
using Vkotk.Models;


namespace Vkotk;

public partial class RedactProductWindow : Window
{
    public int id;
    
    public int countPhoto = 0;
    
    NeondbContext _context = NeondbContext.GetContext();
    
    public List<Imageproduct> img = new List<Imageproduct>();
    
    public RedactProductWindow(Product product)
    {
        AvaloniaXamlLoader.Load(this);
        id = product.Idproduct;
        this.FindControl<TextBox>("TextBoxNameProduct").Text  = product.Nameproduct;
        this.FindControl<TextBox>("CountProduct").Text = product.Countproduct.ToString();
        if(product.Codeproduct != null)
            this.FindControl<TextBox>("Seriescode").Text = product.Codeproduct.ToString();
        this.FindControl<TextBox>("CommentProduct").Text = product.Commentproduct;



        int a;
        if (product.Productimages.Count > 0)
        {   
            
            foreach (var image in product.Productimages)
            {
                a = image.IdimageNavigation.Idimage;
                img.Add(_context.Imageproducts.Where(x => x.Idimage == image.Idimage).FirstOrDefault());
            }
            this.FindControl<ListBox>("ListBoxImage").ItemsSource = img;
        }
    }

    private void ButtonRedact_OnClick(object? sender, RoutedEventArgs e)
    {
        Product product = _context.Products.Where(x => x.Idproduct == id).FirstOrDefault();
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
            NeondbContext.GetContext().SaveChanges();
        }
        catch (Exception exception)
        {
            MessageBoxManager.GetMessageBoxStandard(
                ButtonEnum.Ok.ToString(),
                // Заголовок окна
                "Ошибка подключения к сети"
                // Основной текст
            ).ShowAsync();
        }
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }



    private async void ButtonDelProduct_OnClick(object? sender, RoutedEventArgs e)
    {   
        var product = await _context.Products
            .Include(p => p.Productimages) 
            .FirstOrDefaultAsync(p => p.Idproduct == id);

        if (product != null && product.Productimages.Any())
        {
            product.Productimages.Clear(); 
            await _context.SaveChangesAsync();
        }
        List<Productimage> list = _context.Productimages.Where(x => x.Idproduct == null).ToList();
        foreach (var item in list)
        {
            _context.Productimages.Remove(item);
            _context.Imageproducts.Remove(_context.Imageproducts.Where(x => x.Idimage == item.Idimage).FirstOrDefault());
        }
        
        _context.Products.Remove(_context.Products.Where(x => x.Idproduct == id).FirstOrDefault());
        _context.SaveChanges();
        
        MessageBoxManager.GetMessageBoxStandard(
            ButtonEnum.Ok.ToString(),
            // Заголовок окна
            "Запись удалена"
            // Основной текст
        ).ShowAsync();
        
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
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
    
    private async void  ButtonAddImage_OnClick(object? sender, RoutedEventArgs e)
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
                _context.SaveChanges();
                Productimage product = new Productimage();
                product.Idimage = _context.Imageproducts.OrderByDescending(x => x.Idimage).FirstOrDefault().Idimage;
                product.Idproduct = id;
                _context.Productimages.Add(product);
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
    
    private void ButtonBackMenu_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}