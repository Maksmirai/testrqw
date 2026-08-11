using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Vkotk.Context;
using Vkotk.Models;

namespace Vkotk;

public partial class AddProductWindow : Window
{
    public int countPhoto = 0;

    public List<string> listPhoto = new List<string>();

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
           NeondbContext.GetContext().Products.Add(product);
           NeondbContext.GetContext().SaveChanges();
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

    private void ButtonAddImage_OnClick(object? sender, RoutedEventArgs e)
    {
        //считать после успешного добавления фото не забыть сделать лист куда я буду записывать пути на фото
        var a  = this.FindControl<ListBox>("ListBoxImage");
        countPhoto++;
        
    }

    private void ButtonDelImage_OnClick(object? sender, RoutedEventArgs e)
    {
        countPhoto--;
    }

    private void ButtonBackMenu_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}