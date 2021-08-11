using System.Collections.Generic;

namespace ZarinpalIAB
{
    public delegate void StoreInitializeFailedDelegate(string error);
    
    public delegate void PurchaseSucceedDelegate(Purchase purchase);

    public delegate void PurchaseFailedDelegate(string error);
    
    public delegate void ConsumeSucceedDelegate(Purchase purchase);
    
    public delegate void ConsumeFailedDelegate(string error);
    
    public delegate void PurchaseFailedToStartDelegate(string error);

    public delegate void QueryInventorySucceededDelegate(List<Purchase> purchases, List<ZarrinpalSkuInfo> skuInfos);

    public delegate void QueryInventoryFailedDelegate(string error);

    public delegate void QuerySkuDetailsSucceededDelegate(List<ZarrinpalSkuInfo> skuInfos);

    public delegate void QuerySkuDetailsFailedDelegate(string error);

    public delegate void QueryPurchasesSucceedDelegate(List<Purchase> purchases);

    public delegate void QueryPurchasesFailedDelegate(string error);


}
