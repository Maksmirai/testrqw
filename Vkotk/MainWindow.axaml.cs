using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Vkotk.Context;
using Vkotk.Models;

namespace Vkotk;

public partial class MainWindow : Window
{   
    public static NeondbContext _context;
    public List<Product> listProduct;
    public int adminMode;
    public List<Product> prod;
    public MainWindow()
    {
        adminMode = 0;
        AvaloniaXamlLoader.Load(this);
        _context = NeondbContext.GetContext();
        listProduct = _context.Products.ToList().OrderByDescending(x =>x.Idproduct).ToList();
        var a = this.FindControl<ListBox>("ListBoxData");
        a.ItemsSource = listProduct;
    }
    
    private void ButtonAddProduct_OnClick(object? sender, RoutedEventArgs e)
    {
        AddProductWindow addProductWindow = new AddProductWindow();
        addProductWindow.Show();
        this.Close();
    }

    private void TextBoxSearch_OnKeyUp(object? sender, KeyEventArgs e)
    {
        prod = listProduct;
        var a = this.FindControl<TextBox>("TextBoxSearch").Text;
        var b = this.FindControl<ListBox>("ListBoxData");
        if (a != null || a != " " || a != "")
        {
            prod = prod.Where(x => x.Nameproduct.Contains(a)).OrderByDescending(x => x.Idproduct).ToList();
            b.ItemsSource = prod;
        }
        else
        {
            b.ItemsSource = prod.ToList().OrderByDescending(x => x.Idproduct).ToList();
        }
    }

    private void ListBoxData_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var a = (this.FindControl<ListBox>("ListBoxData").SelectedItem as Product).Idproduct;
        Product product = _context.Products.Where(x => x.Idproduct == a).FirstOrDefault();
        RedactProductWindow redactProductWindow = new RedactProductWindow(product);
        redactProductWindow.Show();
        this.Close();
    }
}