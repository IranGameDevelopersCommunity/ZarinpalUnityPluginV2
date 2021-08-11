using System;
using System.Collections.Generic;

namespace ZarinpalIAB
{
    public interface IZarinpalQueryProvider
    {
        void QueryPurchases(Action<List<Purchase>> succeedAction, Action<string> failedAction);
        void QuerySkuDetail(string[] skus, Action<List<ZarrinpalSkuInfo>> succeedAction, Action<string> failedAction);
    }
} 

