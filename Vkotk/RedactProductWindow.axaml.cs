using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using Vkotk.Context;
using Vkotk.Models;

namespace Vkotk;

public partial class RedactProductWindow : Window
{
    public int id;
    NeondbContext _context = NeondbContext.GetContext();
    public RedactProductWindow(Product product)
    {
        AvaloniaXamlLoader.Load(this);
        id = product.Idproduct;
        this.FindControl<TextBox>("TextBoxNameProduct").Text  = product.Nameproduct;
        this.FindControl<TextBox>("CountProduct").Text = product.Countproduct.ToString();
        this.FindControl<TextBox>("Seriescode").Text = product.Codeproduct.ToString();
        this.FindControl<TextBox>("CommentProduct").Text = product.Commentproduct;
        //if (product.Productimages.Count > 0)
          //  ListBoxImage.SelectedItems = product.Productimages.ToList();
        
        
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
            ).ShowWindowDialogAsync(this);
        }
       
       
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
        //Сделать дату автоматичесукой в sql
    }

    private void ButtonBackMenu_OnClick(object? sender, RoutedEventArgs e)
    {
        MainWindow mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }
}