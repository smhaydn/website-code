using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Collections.Generic;
using System.Linq;

namespace TicimaxSiparisOtomasyonu
{
    class Program
    {
        // WS Yetki Kodunuz - Tüm yetkiler aktif!
        private static string wsYetkiKodu = "WCL2EVYCZ0EW0HGO5U62NF72LJZ1DP";
        private static string siparisServiceUrl = "https://www.lolaofshine.com/Servis/SiparisServis.svc";
        private static string urunServiceUrl = "https://www.lolaofshine.com/Servis/UrunServis.svc";
        private static string uyeServiceUrl = "https://www.lolaofshine.com/Servis/UyeServis.svc";

        static async Task Main(string[] args)
        {
            Console.WriteLine("=== TİCİMAX KAPSAMLI OTOMASYON SİSTEMİ ===");
            Console.WriteLine("🏪 Lola Of Shine - Tam Yetki Programı");
            Console.WriteLine("📅 " + DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            Console.WriteLine("=".PadRight(60, '='));

            try
            {
                while (true)
                {
                    await AnaMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ KRITIK HATA: {ex.Message}");
                Console.WriteLine("\nProgram yeniden başlatılıyor...");
                Console.ReadKey();
            }
        }

        static async Task AnaMenu()
        {
            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.WriteLine("🎯 ANA MENÜ - Tüm Yetkiler Aktif");
            Console.WriteLine("=".PadRight(60, '='));

            Console.WriteLine("\n📦 SİPARİŞ YÖNETİMİ:");
            Console.WriteLine("1.  📋 Sipariş Listesi (Son 30 Gün)");
            Console.WriteLine("2.  🔍 Sipariş Detay Sorgula");
            Console.WriteLine("3. ✏️  Sipariş Durumu Güncelle");
            Console.WriteLine("4.  📊 Sipariş İstatistikleri");

            Console.WriteLine("\n🛍️ ÜRÜN YÖNETİMİ:");
            Console.WriteLine("5.  📝 Ürün Listesi");
            Console.WriteLine("6.  ➕ Yeni Ürün Ekle");
            Console.WriteLine("7.  📈 Stok Durumu");
            Console.WriteLine("8.  💰 Fiyat Güncelleme");

            Console.WriteLine("\n👥 MÜŞTERİ YÖNETİMİ:");
            Console.WriteLine("9.  📞 Müşteri Listesi");
            Console.WriteLine("10. 🔍 Müşteri Detay");
            Console.WriteLine("11. 📧 Müşteri İletişim");

            Console.WriteLine("\n💳 ÖDEME & KARGO:");
            Console.WriteLine("12. 💳 Ödeme Tipleri");
            Console.WriteLine("13. 🚚 Kargo Seçenekleri");
            Console.WriteLine("14. 💰 Para Birimleri");

            Console.WriteLine("\n📈 RAPORLAR:");
            Console.WriteLine("15. 📊 Günlük Rapor");
            Console.WriteLine("16. 📅 Haftalık Rapor");
            Console.WriteLine("17. 📈 Aylık Rapor");

            Console.WriteLine("\n🔧 SİSTEM:");
            Console.WriteLine("18. 🔧 Sistem Testi");
            Console.WriteLine("19. ℹ️  Sistem Bilgileri");
            Console.WriteLine("20. ❌ Çıkış");

            Console.WriteLine("\n" + "=".PadRight(60, '='));
            Console.Write("Seçiminizi yapın (1-20): ");

            string secim = Console.ReadLine();

            switch (secim)
            {
                case "1": await SiparisListesi(); break;
                case "2": await SiparisDetay(); break;
                case "3": await SiparisDurumuGuncelle(); break;
                case "4": await SiparisIstatistikleri(); break;
                case "5": await UrunListesi(); break;
                case "6": await YeniUrunEkle(); break;
                case "7": await StokDurumu(); break;
                case "8": await FiyatGuncelleme(); break;
                case "9": await MusteriListesi(); break;
                case "10": await MusteriDetay(); break;
                case "11": await MusteriIletisim(); break;
                case "12": await OdemeTipleri(); break;
                case "13": await KargoSecenekleri(); break;
                case "14": await ParaBirimleri(); break;
                case "15": await GunlukRapor(); break;
                case "16": await HaftalikRapor(); break;
                case "17": await AylikRapor(); break;
                case "18": await SistemTesti(); break;
                case "19": await SistemBilgileri(); break;
                case "20":
                    Console.WriteLine("\n👋 Program sonlandırılıyor...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\n⚠️  Geçersiz seçim! (1-20 arası)");
                    await Task.Delay(1500);
                    break;
            }
        }

        // 1. Sipariş Listesi
        static async Task SiparisListesi()
        {
            Console.WriteLine("\n📋 SİPARİŞ LİSTESİ (Son 30 Gün)");
            Console.WriteLine("-".PadRight(50, '-'));

            try
            {
                DateTime baslangic = DateTime.Now.AddDays(-30);
                DateTime bitis = DateTime.Now;

                string soapXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <SelectSiparis xmlns=""http://tempuri.org/"">
            <uyeKodu>{wsYetkiKodu}</uyeKodu>
            <filtre>
                <EntegrasyonAktarildi>-1</EntegrasyonAktarildi>
                <OdemeDurumu>-1</OdemeDurumu>
                <SiparisDurumu>-1</SiparisDurumu>
                <SiparisTarihiBas>{baslangic:yyyy-MM-ddTHH:mm:ss}</SiparisTarihiBas>
                <SiparisTarihiSon>{bitis:yyyy-MM-ddTHH:mm:ss}</SiparisTarihiSon>
            </filtre>
            <sayfalama>
                <BaslangicIndex>0</BaslangicIndex>
                <KayitSayisi>20</KayitSayisi>
                <SiralamaDeger>ID</SiralamaDeger>
                <SiralamaYonu>DESC</SiralamaYonu>
            </sayfalama>
        </SelectSiparis>
    </soap:Body>
</soap:Envelope>";

                var result = await SendSoapRequest(soapXml, "SelectSiparis", siparisServiceUrl);

                if (result != null)
                {
                    ParseSiparisListesi(result);
                }
                else
                {
                    Console.WriteLine("❌ Sipariş listesi alınamadı!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        // 2. Sipariş Detay
        static async Task SiparisDetay()
        {
            Console.WriteLine("\n🔍 SİPARİŞ DETAY SORGULA");
            Console.WriteLine("-".PadRight(40, '-'));

            Console.Write("Sipariş ID'sini girin: ");
            if (!int.TryParse(Console.ReadLine(), out int siparisId))
            {
                Console.WriteLine("❌ Geçersiz sipariş ID!");
                Console.ReadKey();
                return;
            }

            try
            {
                string soapXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <SelectSiparisUrun xmlns=""http://tempuri.org/"">
            <uyeKodu>{wsYetkiKodu}</uyeKodu>
            <siparisId>{siparisId}</siparisId>
            <iptalEdilmisUrunler>false</iptalEdilmisUrunler>
        </SelectSiparisUrun>
    </soap:Body>
</soap:Envelope>";

                var result = await SendSoapRequest(soapXml, "SelectSiparisUrun", siparisServiceUrl);

                if (result != null)
                {
                    ParseSiparisDetay(result, siparisId);
                }
                else
                {
                    Console.WriteLine("❌ Sipariş detayı alınamadı!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        // 12. Ödeme Tipleri (Çalışıyor)
        static async Task OdemeTipleri()
        {
            Console.WriteLine("\n💳 ÖDEME TİPLERİ");
            Console.WriteLine("-".PadRight(40, '-'));

            try
            {
                string soapXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <GetOdemeTipleri xmlns=""http://tempuri.org/"">
            <uyeKodu>{wsYetkiKodu}</uyeKodu>
        </GetOdemeTipleri>
    </soap:Body>
</soap:Envelope>";

                var result = await SendSoapRequest(soapXml, "GetOdemeTipleri", siparisServiceUrl);

                if (result != null)
                {
                    ParseOdemeTipleri(result);
                }
                else
                {
                    Console.WriteLine("❌ Ödeme tipleri alınamadı!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Hata: {ex.Message}");
            }

            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        // 18. Sistem Testi
        static async Task SistemTesti()
        {
            Console.WriteLine("\n🔧 KAPSAMLI SİSTEM TESTİ");
            Console.WriteLine("-".PadRight(50, '-'));

            // Test 1: Sipariş Servisi
            Console.WriteLine("\n1️⃣ Sipariş Servisi Testi...");
            await TestService(siparisServiceUrl, "Sipariş");

            // Test 2: Ürün Servisi
            Console.WriteLine("\n2️⃣ Ürün Servisi Testi...");
            await TestService(urunServiceUrl, "Ürün");

            // Test 3: Üye Servisi
            Console.WriteLine("\n3️⃣ Üye Servisi Testi...");
            await TestService(uyeServiceUrl, "Üye");

            // Test 4: Yetki Kodu Testi
            Console.WriteLine("\n4️⃣ Yetki Kodu Testi...");
            await TestAuthCode();

            Console.WriteLine("\n✅ Sistem testi tamamlandı!");
            Console.WriteLine("\nDevam etmek için bir tuşa basın...");
            Console.ReadKey();
        }

        // Basit implementasyonlar (Genişletilebilir)
        static async Task SiparisDurumuGuncelle()
        {
            Console.WriteLine("\n✏️ SİPARİŞ DURUMU GÜNCELLE");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task SiparisIstatistikleri()
        {
            Console.WriteLine("\n📊 SİPARİŞ İSTATİSTİKLERİ");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task UrunListesi()
        {
            Console.WriteLine("\n📝 ÜRÜN LİSTESİ");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task YeniUrunEkle()
        {
            Console.WriteLine("\n➕ YENİ ÜRÜN EKLE");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task StokDurumu()
        {
            Console.WriteLine("\n📈 STOK DURUMU");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task FiyatGuncelleme()
        {
            Console.WriteLine("\n💰 FİYAT GÜNCELLEME");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task MusteriListesi()
        {
            Console.WriteLine("\n📞 MÜŞTERİ LİSTESİ");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task MusteriDetay()
        {
            Console.WriteLine("\n🔍 MÜŞTERİ DETAY");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task MusteriIletisim()
        {
            Console.WriteLine("\n📧 MÜŞTERİ İLETİŞİM");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task KargoSecenekleri()
        {
            Console.WriteLine("\n🚚 KARGO SEÇENEKLERİ");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task ParaBirimleri()
        {
            Console.WriteLine("\n💰 PARA BİRİMLERİ");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task GunlukRapor()
        {
            Console.WriteLine("\n📊 GÜNLÜK RAPOR");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task HaftalikRapor()
        {
            Console.WriteLine("\n📅 HAFTALIK RAPOR");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task AylikRapor()
        {
            Console.WriteLine("\n📈 AYLIK RAPOR");
            Console.WriteLine("Bu özellik geliştirilme aşamasında...");
            Console.ReadKey();
        }

        static async Task SistemBilgileri()
        {
            Console.WriteLine("\n ℹ️ SİSTEM BİLGİLERİ");
            Console.WriteLine("-".PadRight(40, '-'));
            Console.WriteLine($"🏪 Mağaza: Lola Of Shine");
            Console.WriteLine($"🔑 WS Yetki: {wsYetkiKodu.Substring(0, 12)}...***");
            Console.WriteLine($"📅 Tarih: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
            Console.WriteLine($"💻 Framework: .NET 8.0");
            Console.WriteLine($"🌐 Sipariş Servisi: {siparisServiceUrl}");
            Console.WriteLine($"📦 Ürün Servisi: {urunServiceUrl}");
            Console.WriteLine($"👥 Üye Servisi: {uyeServiceUrl}");
            Console.WriteLine("\n✅ Tüm yetkiler aktif!");
            Console.ReadKey();
        }

        // Yardımcı Fonksiyonlar
        static async Task<string> SendSoapRequest(string soapXml, string action, string serviceUrl)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(30);

                    var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");
                    content.Headers.Add("SOAPAction", $"http://tempuri.org/ISiparisServis/{action}");

                    var response = await client.PostAsync(serviceUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return await response.Content.ReadAsStringContent();
                    }
                    else
                    {
                        Console.WriteLine($"❌ HTTP Hatası: {response.StatusCode}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ SOAP İstek Hatası: {ex.Message}");
                return null;
            }
        }

        static async Task TestService(string serviceUrl, string serviceName)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(10);
                    var response = await client.GetAsync($"{serviceUrl}?wsdl");

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"   ✅ {serviceName} Servisi: Erişilebilir");
                    }
                    else
                    {
                        Console.WriteLine($"   ❌ {serviceName} Servisi: Erişim hatası ({response.StatusCode})");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ {serviceName} Servisi: {ex.Message}");
            }
        }

        static async Task TestAuthCode()
        {
            try
            {
                string soapXml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"">
    <soap:Body>
        <GetOdemeTipleri xmlns=""http://tempuri.org/"">
            <uyeKodu>{wsYetkiKodu}</uyeKodu>
        </GetOdemeTipleri>
    </soap:Body>
</soap:Envelope>";

                var result = await SendSoapRequest(soapXml, "GetOdemeTipleri", siparisServiceUrl);

                if (result != null && !result.Contains("fault"))
                {
                    Console.WriteLine("   ✅ Yetki Kodu: Geçerli ve aktif");
                }
                else
                {
                    Console.WriteLine("   ❌ Yetki Kodu: Hatalı veya geçersiz");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Yetki testi: {ex.Message}");
            }
        }

        static void ParseSiparisListesi(string xmlResponse)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsmgr.AddNamespace("ns", "http://tempuri.org/");

                // SOAP Hata kontrolü
                var faultNode = doc.SelectSingleNode("//soap:Fault", nsmgr);
                if (faultNode != null)
                {
                    var faultString = faultNode.SelectSingleNode("faultstring")?.InnerText;
                    Console.WriteLine($"❌ SOAP Hatası: {faultString}");
                    return;
                }

                var siparislerNode = doc.SelectSingleNode("//ns:SelectSiparisResponse/ns:SelectSiparisResult", nsmgr);

                if (siparislerNode != null)
                {
                    var siparisNodes = siparislerNode.SelectNodes(".//ns:WebSiparis", nsmgr);

                    if (siparisNodes != null && siparisNodes.Count > 0)
                    {
                        Console.WriteLine($"✅ {siparisNodes.Count} sipariş bulundu:\n");

                        Console.WriteLine($"{"ID",-8} {"Tarih",-12} {"Tutar",-10} {"Durum",-15} {"Müşteri",-20}");
                        Console.WriteLine("-".PadRight(70, '-'));

                        foreach (XmlNode node in siparisNodes)
                        {
                            var id = node.SelectSingleNode("ns:ID", nsmgr)?.InnerText ?? "?";
                            var tarih = DateTime.TryParse(node.SelectSingleNode("ns:SiparisTarihi", nsmgr)?.InnerText, out DateTime dt)
                                        ? dt.ToString("dd.MM.yyyy") : "?";
                            var tutar = node.SelectSingleNode("ns:ToplamTutar", nsmgr)?.InnerText ?? "0";
                            var durum = node.SelectSingleNode("ns:SiparisDurumu", nsmgr)?.InnerText ?? "?";
                            var musteri = node.SelectSingleNode("ns:AdiSoyadi", nsmgr)?.InnerText ?? "?";

                            // Tutar formatla
                            if (decimal.TryParse(tutar, out decimal tutarDecimal))
                            {
                                tutar = tutarDecimal.ToString("C");
                            }

                            Console.WriteLine($"{id,-8} {tarih,-12} {tutar,-10} {durum,-15} {musteri,-20}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("📋 Son 30 günde sipariş bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Sipariş verisi parse edilemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parse hatası: {ex.Message}");
            }
        }

        static void ParseSiparisDetay(string xmlResponse, int siparisId)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsmgr.AddNamespace("ns", "http://tempuri.org/");

                var urunlerNode = doc.SelectSingleNode("//ns:SelectSiparisUrunResponse/ns:SelectSiparisUrunResult", nsmgr);

                if (urunlerNode != null)
                {
                    var urunNodes = urunlerNode.SelectNodes(".//ns:WebSiparisUrun", nsmgr);

                    if (urunNodes != null && urunNodes.Count > 0)
                    {
                        Console.WriteLine($"\n🔍 Sipariş #{siparisId} Detayları:");
                        Console.WriteLine($"📦 {urunNodes.Count} ürün bulundu:\n");

                        Console.WriteLine($"{"Ürün",-25} {"Adet",-6} {"Tutar",-10} {"Durum",-15}");
                        Console.WriteLine("-".PadRight(60, '-'));

                        foreach (XmlNode node in urunNodes)
                        {
                            var urunAdi = node.SelectSingleNode("ns:UrunAdi", nsmgr)?.InnerText ?? "?";
                            var adet = node.SelectSingleNode("ns:Adet", nsmgr)?.InnerText ?? "0";
                            var tutar = node.SelectSingleNode("ns:Tutar", nsmgr)?.InnerText ?? "0";
                            var durum = node.SelectSingleNode("ns:DurumAd", nsmgr)?.InnerText ?? "?";

                            // Uzun ürün adını kısalt
                            if (urunAdi.Length > 22)
                                urunAdi = urunAdi.Substring(0, 19) + "...";

                            // Tutar formatla
                            if (decimal.TryParse(tutar, out decimal tutarDecimal))
                            {
                                tutar = tutarDecimal.ToString("C");
                            }

                            Console.WriteLine($"{urunAdi,-25} {adet,-6} {tutar,-10} {durum,-15}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"📦 Sipariş #{siparisId} için ürün bulunamadı.");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Sipariş detayı parse edilemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parse hatası: {ex.Message}");
            }
        }

        static void ParseOdemeTipleri(string xmlResponse)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlResponse);

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("soap", "http://schemas.xmlsoap.org/soap/envelope/");
                nsmgr.AddNamespace("ns", "http://tempuri.org/");

                var odemeTipleriNode = doc.SelectSingleNode("//ns:GetOdemeTipleriResponse/ns:GetOdemeTipleriResult", nsmgr);

                if (odemeTipleriNode != null)
                {
                    var odemeTipiNodes = odemeTipleriNode.SelectNodes(".//ns:SiparisOdemeTipleri", nsmgr);

                    if (odemeTipiNodes != null && odemeTipiNodes.Count > 0)
                    {
                        Console.WriteLine($"✅ {odemeTipiNodes.Count} ödeme tipi bulundu:\n");

                        Console.WriteLine($"{"ID",-4} {"Ödeme Tipi",-30}");
                        Console.WriteLine("-".PadRight(35, '-'));

                        foreach (XmlNode node in odemeTipiNodes)
                        {
                            var id = node.SelectSingleNode("ns:ID", nsmgr)?.InnerText ?? "?";
                            var tanim = node.SelectSingleNode("ns:Tanim", nsmgr)?.InnerText ?? "Bilinmiyor";

                            string emoji = tanim.ToLower() switch
                            {
                                var x when x.Contains("kredi") => "💳",
                                var x when x.Contains("havale") => "🏦",
                                var x when x.Contains("kapıda") => "🚪",
                                var x when x.Contains("nakit") => "💵",
                                _ => "💰"
                            };

                            Console.WriteLine($"{id,-4} {emoji} {tanim,-28}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("❌ Ödeme tipi bulunamadı!");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Ödeme tipi verisi parse edilemedi!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Parse hatası: {ex.Message}");
            }
        }
    }
}