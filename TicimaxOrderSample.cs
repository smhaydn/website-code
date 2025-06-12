/*
 * NuGet paketleri:
 *   dotnet add package CsvHelper
 *   dotnet add package ClosedXML
 *
 * Bu program Ticimax Siparis Servisi
 * ornek kullanimi icindir.
 */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CsvHelper;
using ClosedXML.Excel;

namespace TicimaxOrderSample
{
    // Basit sipariş ve ürün modelleri
    public class OrderItem
    {
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string Variant { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public class Order
    {
        public int ID { get; set; }
        public string OrderNumber { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string BillingAddress { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string CargoCompany { get; set; } = string.Empty;
        public string CargoTracking { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Note { get; set; } = string.Empty;
        public List<OrderItem> Items { get; set; } = new();
    }

    class Program
    {
        // Ticimax web servis yetki kodu
        static string uyeKodu = "WCL2EVYCZ0EW0HGO5U62NF72LJZ1DP";
        static string serviceUrl = "https://www.lolaofshine.com/Servis/SiparisServis.svc";

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Ticimax siparişleri çekiliyor...\n");

            var start = DateTime.Today.AddDays(-7);      // son 7 gün
            var end = DateTime.Today.AddDays(1);

            var orders = await GetOrdersAsync(start, end);

            foreach (var order in orders)
            {
                PrintOrder(order);
            }

            // İsteğe bağlı kaydetme
            SaveToCsv(orders, "orders.csv");
            SaveToExcel(orders, "orders.xlsx");

            Console.WriteLine("\nTamamlandı.");
        }

        // Siparişleri SOAP servisten çeker
        static async Task<List<Order>> GetOrdersAsync(DateTime start, DateTime end)
        {
            string soap = $@"<?xml version=\"1.0\" encoding=\"utf-8\"?>
<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">
  <soap:Body>
    <SelectSiparis xmlns=\"http://tempuri.org/\">
      <uyeKodu>{uyeKodu}</uyeKodu>
      <filtre>
        <SiparisTarihiBas>{start:yyyy-MM-ddTHH:mm:ss}</SiparisTarihiBas>
        <SiparisTarihiSon>{end:yyyy-MM-ddTHH:mm:ss}</SiparisTarihiSon>
        <EntegrasyonAktarildi>-1</EntegrasyonAktarildi>
        <OdemeDurumu>-1</OdemeDurumu>
        <SiparisDurumu>-1</SiparisDurumu>
      </filtre>
      <sayfalama>
        <BaslangicIndex>0</BaslangicIndex>
        <KayitSayisi>50</KayitSayisi>
        <SiralamaDeger>ID</SiralamaDeger>
        <SiralamaYonu>DESC</SiralamaYonu>
      </sayfalama>
    </SelectSiparis>
  </soap:Body>
</soap:Envelope>";

            string xml = await PostSoapAsync("SelectSiparis", soap);
            if (string.IsNullOrEmpty(xml)) return new();
            return await ParseOrdersAsync(xml);
        }

        // Bir siparişe ait ürünleri çeker
        static async Task<List<OrderItem>> GetOrderItemsAsync(int orderId)
        {
            string soap = $@"<?xml version=\"1.0\" encoding=\"utf-8\"?>
<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">
  <soap:Body>
    <SelectSiparisUrun xmlns=\"http://tempuri.org/\">
      <uyeKodu>{uyeKodu}</uyeKodu>
      <siparisId>{orderId}</siparisId>
      <iptalEdilmisUrunler>false</iptalEdilmisUrunler>
    </SelectSiparisUrun>
  </soap:Body>
</soap:Envelope>";

            string xml = await PostSoapAsync("SelectSiparisUrun", soap);
            return ParseOrderItems(xml);
        }

        // SOAP post helper
        static async Task<string> PostSoapAsync(string action, string body)
        {
            try
            {
                using var client = new HttpClient();
                var content = new StringContent(body, Encoding.UTF8, "text/xml");
                content.Headers.Add("SOAPAction", $"http://tempuri.org/ISiparisServis/{action}");
                var resp = await client.PostAsync(serviceUrl, content);
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SOAP hata: {ex.Message}");
                return string.Empty;
            }
        }

        // Gelen order xml'ini parse eder ve her siparişin ürünlerini de alır
        static async Task<List<Order>> ParseOrdersAsync(string xml)
        {
            var orders = new List<Order>();
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns", "http://tempuri.org/");

            var orderNodes = doc.SelectNodes("//ns:WebSiparis", nsmgr);
            if (orderNodes == null) return orders;

            foreach (XmlNode o in orderNodes)
            {
                var ord = new Order();
                ord.ID = int.Parse(o.SelectSingleNode("ns:ID", nsmgr)?.InnerText ?? "0");
                ord.OrderNumber = o.SelectSingleNode("ns:SiparisKodu", nsmgr)?.InnerText ?? string.Empty;
                if (DateTime.TryParse(o.SelectSingleNode("ns:SiparisTarihi", nsmgr)?.InnerText, out DateTime dt))
                    ord.Date = dt;
                ord.CustomerName = (o.SelectSingleNode("ns:UyeAdi", nsmgr)?.InnerText + " " + o.SelectSingleNode("ns:UyeSoyadi", nsmgr)?.InnerText).Trim();
                ord.Phone = o.SelectSingleNode("ns:UyeTelefon", nsmgr)?.InnerText ?? string.Empty;
                ord.Email = o.SelectSingleNode("ns:Mail", nsmgr)?.InnerText ?? string.Empty;
                ord.BillingAddress = o.SelectSingleNode("ns:FaturaAdres/ns:Adres", nsmgr)?.InnerText ?? string.Empty;
                ord.ShippingAddress = o.SelectSingleNode("ns:TeslimatAdres/ns:Adres", nsmgr)?.InnerText ?? string.Empty;
                ord.PaymentType = o.SelectSingleNode("ns:OdemeTipi", nsmgr)?.InnerText ?? string.Empty;
                ord.PaymentStatus = o.SelectSingleNode("ns:OdemeDurumu", nsmgr)?.InnerText ?? string.Empty;
                ord.CargoCompany = o.SelectSingleNode("ns:KargoFirma", nsmgr)?.InnerText ?? string.Empty;
                ord.CargoTracking = o.SelectSingleNode("ns:KargoTakipNo", nsmgr)?.InnerText ?? string.Empty;
                ord.Note = o.SelectSingleNode("ns:SiparisNotu", nsmgr)?.InnerText ?? string.Empty;
                if (decimal.TryParse(o.SelectSingleNode("ns:ToplamTutar", nsmgr)?.InnerText, out decimal tut))
                    ord.TotalAmount = tut;

                ord.Items = await GetOrderItemsAsync(ord.ID);
                orders.Add(ord);
            }

            return orders;
        }

        // Ürün listesi parse
        static List<OrderItem> ParseOrderItems(string xml)
        {
            var items = new List<OrderItem>();
            if (string.IsNullOrEmpty(xml)) return items;

            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
            nsmgr.AddNamespace("ns", "http://tempuri.org/");

            var nodes = doc.SelectNodes("//ns:WebSiparisUrun", nsmgr);
            if (nodes == null) return items;

            foreach (XmlNode n in nodes)
            {
                var it = new OrderItem();
                it.ProductName = n.SelectSingleNode("ns:UrunAdi", nsmgr)?.InnerText ?? string.Empty;
                it.SKU = n.SelectSingleNode("ns:StokKodu", nsmgr)?.InnerText ?? string.Empty;
                it.Variant = n.SelectSingleNode("ns:VaryantAdi", nsmgr)?.InnerText ?? string.Empty;
                it.Quantity = (int)Decimal.Parse(n.SelectSingleNode("ns:Adet", nsmgr)?.InnerText ?? "0");
                if (decimal.TryParse(n.SelectSingleNode("ns:BirimFiyat", nsmgr)?.InnerText, out decimal bf))
                    it.Price = bf;
                if (decimal.TryParse(n.SelectSingleNode("ns:Tutar", nsmgr)?.InnerText, out decimal tt))
                    it.Total = tt;
                items.Add(it);
            }
            return items;
        }

        // Sipariş bilgilerini ekrana yazdır
        static void PrintOrder(Order o)
        {
            Console.WriteLine($"Sipariş: {o.OrderNumber} - {o.Date:dd.MM.yyyy}");
            Console.WriteLine($"Müşteri : {o.CustomerName}  Tel:{o.Phone}  E-posta:{o.Email}");
            Console.WriteLine($"Fatura  : {o.BillingAddress}");
            Console.WriteLine($"Teslimat: {o.ShippingAddress}");
            Console.WriteLine($"Ödeme   : {o.PaymentType} / {o.PaymentStatus}");
            Console.WriteLine($"Kargo   : {o.CargoCompany} TakipNo:{o.CargoTracking}");
            Console.WriteLine($"Toplam  : {o.TotalAmount:C}");
            if (!string.IsNullOrEmpty(o.Note))
                Console.WriteLine($"Not     : {o.Note}");

            Console.WriteLine("Ürünler:");
            Console.WriteLine($"{"Ürün",-30} {"Sku",-15} {"Varyant",-15} {"Adet",-5} {"Fiyat",-10} {"Tutar",-10}");
            foreach (var item in o.Items)
            {
                Console.WriteLine($"{item.ProductName,-30} {item.SKU,-15} {item.Variant,-15} {item.Quantity,-5} {item.Price,-10:C} {item.Total,-10:C}");
            }
            Console.WriteLine(new string('-', 80));
        }

        // CSV çıktı
        static void SaveToCsv(List<Order> orders, string path)
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(FlattenOrders(orders));
        }

        // Excel çıktı
        static void SaveToExcel(List<Order> orders, string path)
        {
            using var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Orders");
            var flat = FlattenOrders(orders);
            ws.Cell(1, 1).InsertTable(flat);
            wb.SaveAs(path);
        }

        // CSV ve Excel için düz liste hazırlar
        static List<dynamic> FlattenOrders(List<Order> orders)
        {
            var list = new List<dynamic>();
            foreach (var o in orders)
            {
                foreach (var it in o.Items)
                {
                    list.Add(new
                    {
                        OrderNo = o.OrderNumber,
                        Date = o.Date,
                        Customer = o.CustomerName,
                        Phone = o.Phone,
                        Email = o.Email,
                        Billing = o.BillingAddress,
                        Shipping = o.ShippingAddress,
                        Payment = o.PaymentType,
                        PaymentStatus = o.PaymentStatus,
                        Cargo = o.CargoCompany,
                        Tracking = o.CargoTracking,
                        Total = o.TotalAmount,
                        Product = it.ProductName,
                        SKU = it.SKU,
                        Variant = it.Variant,
                        Quantity = it.Quantity,
                        Price = it.Price,
                        LineTotal = it.Total,
                        Note = o.Note
                    });
                }
            }
            return list;
        }
    }
}
