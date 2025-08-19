using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

class Program
{
    static void Main()
    {
        var order = new Order<HomeDelivery, int>(new HomeDelivery("Гаджиево дом 60"), "Заказ электроники");
        order.Cart.AddProduct(new FoodProduct("мясо", DateTime.Now.AddDays(2), 50, 2));
        order.Cart.AddProduct(new Electronicproduct("Компакт-диск", 2, 2000, 24));
        order.ShowOrder();

        var soon = order.Cart.FindProducts<FoodProduct>(f => f.ExpirationDate < DateTime.Now.AddDays(2));
        Console.WriteLine("\nСкоро истекает срок годности у ");
        foreach (var item in soon)
        {
            item.DisplayInfo();
        }


    }

  }







abstract class Delivery // класс будет служить для различных типов доставки класс доставка
{
    public string Address { get; protected set; } // адрес будем менять только в производных классах
    public abstract void Deliver(); // метод в дальнейшем будет служить для вывода адреса на консоль
}

class HomeDelivery : Delivery
{
    public HomeDelivery(string address)
    {
        Address = address;
    }

    public override void Deliver()
    {
        Console.WriteLine($"Доставка на дом по адресу {Address}");
    }
}


class PickPointDelivery:Delivery
{
    public string PickPoint { get; protected set; }
    
    public PickPointDelivery(string pickPointDelivery, string address)
    {
        PickPoint = pickPointDelivery;
        Address = address;
    }
    public override void Deliver()
    {
        Console.WriteLine($"Доставка в пункту выдачи {PickPoint} , адрес {Address} ");
    }

}

class ShopDelivery : Delivery
{
    public string Shop { get; protected set; }

    public ShopDelivery(string address)
    {
        Shop = address;
    }
    public override void Deliver()
    {
        Console.WriteLine($"Самовывоз из магазина, адрес {Shop}");
    }
}


abstract class Product
{
    public string Name { get; protected set; }  // наименование продукта
    private decimal price;                      // цена
    private int quantity;                       // количество

    public decimal Price
    {
        get
        {
            return price;
        }
        set
        {
            if (value < 0)
            {
                Console.WriteLine("Цена не может быть отрицательной");
            }
            else
            {
                price = value;
            }
        }
    }

    public int Quantity
    {
        get
        {
            return quantity;
        }
        set
        {
            if (value < 1)
            {
                Console.WriteLine("Количество должно быть больше 0");
            }
            else
            {
                quantity = value;
            }
        }
    }

    public decimal TotalPrice
    {
        get
        {
            return Price*Quantity;
        }
    }
     protected Product(string name, decimal price, int quantity)
    {
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public abstract void DisplayInfo();
}

class FoodProduct : Product
{
    public DateTime DateAndTime { get; protected set; }
    public DateTime ExpirationDate { get; private set; }
    public FoodProduct(string name, DateTime expiration, int quantity, decimal price): base(name, price, quantity)
    {
        ExpirationDate = expiration;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Товар {Name}, Цена {Price}, Количество {Quantity}, Дата и время {DateAndTime}");
    }
}

class Electronicproduct : Product
{
    public int Warranty { get; protected set; }

    public Electronicproduct(string name, int quantity, decimal price, int warranty) : base(name, price, quantity)
    {
        Warranty = warranty;
    }

    public override void DisplayInfo()
    {
        Console.WriteLine($"Товар: {Name}, Цена: {Price}, Кол-во: {Quantity}, Гарантия: {Warranty} мес.");
    }
}


class Cart
{
    private List<Product> products = new List<Product>();

    public void AddProduct(Product product)
    {
        products.Add(product);
    }

    public void RemoveProduct(Product product)
    {
        products.Remove(product);
    }

    public decimal GetTotalPrice()
    {
        decimal total = 0m;
        for (int i = 0; i < products.Count; i++)
        {
            total += products[i].Price;
        }
        return total; 
    }

    public void DisplayProducts()
    {
        Console.WriteLine("Содержимое корзины");
        foreach (Product product in products) 
            product.DisplayInfo();
    }

    public IEnumerable<T> FindProducts<T> (Func<T,bool> predicate) where T : Product
    {
        return products.OfType<T>().Where(predicate);
    }
}



class Order<TDelivery, Tstruct> where TDelivery : Delivery
{
    public TDelivery Delivery { get; private set; }
    public int Number { get; private set; }

    private static int globalCounter = 0;

    public string Description {  get; set; }
    public Cart Cart { get; private set; }

    public Order(TDelivery delivery, string description)
    {
       globalCounter++;
        Delivery = delivery;
        Number=globalCounter;
        Description = description;
        Cart = new Cart();
    }

    public void DiplayAdress()
    {
        Console.WriteLine($"Адрес доставки {Delivery.Address}"); // выводит адрес доставки 
    }
    public void ShowOrder()
    {
        Console.WriteLine($"\nЗаказ {Number}");
        Console.WriteLine($"Тип {Description}");
        Cart.DisplayProducts();
        Console.WriteLine($"Итого{Cart.GetTotalPrice()}");
        Delivery.Deliver();
    }
}


