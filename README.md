# website-code

Bu repoda Ticimax servislerini kullanan ornek kodlar bulunur.

Yeni eklenen **TicimaxOrderSample.cs** dosyasi, siparisleri SOAP uzerinden cekip
konsola yazan ve CSV/Excel'e kaydeden basit bir ornek uygular. Dosyayi bir
`Console App` projesine ekleyip asagidaki NuGet paketlerini yukledikten sonra
calistirabilirsiniz:

```
dotnet add package CsvHelper
dotnet add package ClosedXML
```

`uyeKodu` degiskeninde WS yetki kodunuzu tanimlayarak calistirabilirsiniz.
