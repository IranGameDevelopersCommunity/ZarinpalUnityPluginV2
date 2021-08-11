
using System;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

namespace ZarinpalIAB
{
    [Serializable]
    public class ZarrinpalSkuInfo
    {
        public string ProductId;
        public string Title;
        public string Price;
        public string Type;
        public string Description;


        public string PriceCurrencyCode { get; private set; }

        public static List<ZarrinpalSkuInfo> fromJsonArray(JSONArray items)
        {
            var skuInfos = new List<ZarrinpalSkuInfo>();

            foreach (JSONNode item in items.AsArray)
            {
                ZarrinpalSkuInfo bSkuInfo = new ZarrinpalSkuInfo();
                bSkuInfo.fromJson(item.AsObject);
                skuInfos.Add(bSkuInfo);
            }

            return skuInfos;
        }

        public ZarrinpalSkuInfo() { }

        public void fromJson(JSONClass json)
        {
            Title = json["title"].Value;
            Price = json["price"].Value;
            Type = json["type"].Value;
            Description = json["description"].Value;
            ProductId = json["productId"].Value;
            PriceCurrencyCode = json["price_currency_code"].Value;
        }

        public override string ToString()
        {
            return string.Format("<BazaarSkuInfo> title: {0}, price: {1}, type: {2}, description: {3}, productId: {4}  , priceCurrencyCode : {5}",
                Title, Price, Type, Description, ProductId,PriceCurrencyCode);
        }

    }
}
