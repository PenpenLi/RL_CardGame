using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
//using Umeng;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
//脚本需在调用购买方法之前初始化
public class Purchaser : MonoBehaviour //, IStoreListener
{
    //定义商品
    public const string product_1 = "com.lemonjamstudio.infiniteknights.gem50";//消耗商品
    public const string product_2 = "com.lemonjamstudio.infiniteknights.gem300";//消耗商品
    public const string product_3 = "com.lemonjamstudio.infiniteknights.gem800";//消耗商品
    public const string product_4 = "com.lemonjamstudio.infiniteknights.gift";//消耗商品
    public const string product_5 = "com.lemonjamstudio.infiniteknights.doublecoin";//消耗商品
    public const string product_6 = "com.lemonjamstudio.infiniteknights.fond";//关卡基金
    public const string product_7 = "com.lemonjamstudio.infiniteknights.summon";//召唤石礼包
    public const string product_8 = "com.lemonjamstudio.infiniteknights.giftdaily";//新手每日礼包
    public const string product_9 = "com.lemonjamstudio.infiniteknights.gem68";//68元钻石
    public const string product_10 = "com.lemonjamstudio.infiniteknights.gem98";//98元钻石
    public const string product_11 = "com.lemonjamstudio.infiniteknights.gem328";//328元钻石
    public const string product_12 = "com.lemonjamstudio.infiniteknights.gem648";//648元钻石
    public const string product_14 = "com.lemonjamstudio.infiniteknights.monthcard";//消耗商品

    //ios内购
    public const string product_ios_1 = "knights.gem50";//消耗商品
    public const string product_ios_2 = "knights.gem300";//消耗商品
    public const string product_ios_3 = "knights.gem800";//消耗商品
    public const string product_ios_4 = "knights.gift";//消耗商品
    public const string product_ios_5 = "knights.subscribe";
    public const string product_ios_6 = "knights.package1";//消耗商品
    public const string product_ios_7 = "knights.package2";//消耗商品
    public const string product_ios_8 = "knights.package3";//消耗商品
    public const string product_ios_9 = "knights.doublegold";//消耗商品
    public const string product_ios_10 = "knights.monthcard";//消耗商品
    public const string product_ios_11 = "knights.fond";//关卡基金
    public const string product_ios_12 = "knights.summon";//召唤石礼包
    public const string product_ios_13 = "knights.giftdaily";//新手每日礼包
    public const string product_ios_14 = "knights.gem68";//68元钻石
    public const string product_ios_15 = "knights.gem98";//98元钻石
    public const string product_ios_16 = "knights.gem328";//328元钻石
    public const string product_ios_17 = "knights.gem648";//648元钻石

    public Purchaser instance;

  //  private static IStoreController m_StoreController;          // The Unity Purchasing system.
    //private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public static string kProductIDConsumable = "consumable";
    public static string kProductIDNonConsumable = "nonconsumable";
    public static string kProductIDSubscription = "subscription";
    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";
    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

    //public RTPurchasePanel purchasePanelGP;
    //public RTPurchasePanel purchasePanelIOS;
    //public RTPurchasePanel testNullPurchasePanel;
    //public bool isInit = false;

    private string LOGTAG = "PurchaseTestCathy==";
    void Start()
    {
#if UNITY_IOS || ANDROID_GP
        //初始化购买sdk
        instance = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
#endif
    }

    public void initPurchaser()
    {
#if UNITY_IOS || ANDROID_GP
        //初始化购买sdk
        instance = this;
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }
#endif
    }

    /*
    public void InitializePurchasing()
    {
        Debug.Log(LOGTAG + "steve 初始化：" + IsInitialized());
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //添加商品ID和类型 对应定义的商品ID
#if UNITY_IOS
        builder.AddProduct(product_ios_1, ProductType.Consumable);
        builder.AddProduct(product_ios_2, ProductType.Consumable);
        builder.AddProduct(product_ios_3, ProductType.Consumable);
        builder.AddProduct(product_ios_4, ProductType.Consumable);
        builder.AddProduct(product_ios_5, ProductType.Subscription);
        builder.AddProduct(product_ios_6, ProductType.Consumable);
        builder.AddProduct(product_ios_7, ProductType.Consumable);
        builder.AddProduct(product_ios_8, ProductType.Consumable);
        builder.AddProduct(product_ios_9, ProductType.Consumable);
        builder.AddProduct(product_ios_10, ProductType.Consumable);

        builder.AddProduct(product_ios_11, ProductType.NonConsumable);
        builder.AddProduct(product_ios_12, ProductType.Consumable);
        builder.AddProduct(product_ios_13, ProductType.Consumable);
        builder.AddProduct(product_ios_14, ProductType.Consumable);
        builder.AddProduct(product_ios_15, ProductType.Consumable);
        builder.AddProduct(product_ios_16, ProductType.Consumable);
        builder.AddProduct(product_ios_17, ProductType.Consumable);
#endif
#if ANDROID_GP
        builder.AddProduct(product_1, ProductType.Consumable);
        builder.AddProduct(product_2, ProductType.Consumable);
        builder.AddProduct(product_3, ProductType.Consumable);
        builder.AddProduct(product_4, ProductType.Consumable);
        builder.AddProduct(product_5, ProductType.Consumable);
        builder.AddProduct(product_6, ProductType.NonConsumable);
        builder.AddProduct(product_7, ProductType.Consumable);
        builder.AddProduct(product_8, ProductType.Consumable);
        builder.AddProduct(product_9, ProductType.Consumable);
        builder.AddProduct(product_10, ProductType.Consumable);
        builder.AddProduct(product_11, ProductType.Consumable);
        builder.AddProduct(product_12, ProductType.Consumable);
        builder.AddProduct(product_14, ProductType.Consumable);
#endif

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);

    }
    private bool IsInitialized()
    {

        // Only say we are initialized if both the Purchasing references are set.
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }
    public void BuyConsumable()
    {
        // Buy the consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDConsumable);
    }
    public void BuyNonConsumable()
    {
        // Buy the non-consumable product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        BuyProductID(kProductIDNonConsumable);
    }
    public void BuySubscription()
    {
        // Buy the subscription product using its the general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        // Notice how we use the general product identifier in spite of this ID being mapped to
        // custom store-specific identifiers above.
        BuyProductID(kProductIDSubscription);
    }

    //购买商品调用的方法
    public void BuyProductID(string productId)
    {
#if UNITY_IOS || ANDROID_GP
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(LOGTAG + string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log(LOGTAG + "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                createPurcahseFailMsg(productId);
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log(LOGTAG + "BuyProductID FAIL. Not initialized.");
            createPurcahseFailMsg(productId);
        }
#endif
    }

    public void createPurcahseFailMsg(string productId)
    {
        GA.Event("BuyFail", productId.Replace("com.lemonjamstudio.infiniteknights", ""));
        string message = SDGameManager.T("PurchaseFail");
        SLPopoutController.CreatePopoutMessage(message, 901, null);
    }

    public void createOpFailMsg()
    {
        string message = SDGameManager.T("OP_FAIL");
        SLPopoutController.CreatePopoutMessage(message, 901, null);
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
#if UNITY_IOS || ANDROID_GP
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log(LOGTAG + "RestorePurchases FAIL. Not initialized.");
            createOpFailMsg();
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log(LOGTAG + "RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) => {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log(LOGTAG + "RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            createOpFailMsg();
            Debug.Log(LOGTAG + "RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
#endif
    }
    //  
    // --- IStoreListener
    //
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log(LOGTAG + "OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;
        Invoke("InitSuccess", 0.5f);
    }

    public void InitSuccess()
    {
        Debug.Log(LOGTAG + "Cathy Init Success");
        isInit = true;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log(LOGTAG + "OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void addChargeNum(int num)
    {
        SDDataManager.Instance.playerData.playerChargeNum += num;
    }

    //购买不同商品结束后的处理方法 对应定义的商品
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
#if UNITY_IOS || ANDROID_GP
        Debug.Log(LOGTAG + "Cathy ProcessPurchase  id:" + args.purchasedProduct.definition.id);
        if (!isInit)
        {
            Debug.Log(LOGTAG + "Cathy 处理上一次没完成的异常账单");
            //return PurchaseProcessingResult.Complete;
        }
#endif
#if UNITY_IOS
        if (String.Equals(args.purchasedProduct.definition.id, product_ios_1, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品1购买成功");
            //测试未处理完订单
            //testNullPurchasePanel.gameObject.SetActive(false);
            Debug.Log(LOGTAG + "steve 商品1购买成功 buyGem50Success");
            addChargeNum(6);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem50Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_2, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品2购买成功");
            addChargeNum(30);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem300Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_3, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品3购买成功");
            addChargeNum(60);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem800Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_4, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品4购买成功");
            addChargeNum(3);
            SDGameManager.Instance.inPurchaseStorePanelIOS.purchaseNoviceGiftSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_5, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品5购买成功");
            if (!SDDataManager.Instance.GetSubscibeState())
            {
                addChargeNum(30);
                SDGameManager.Instance.inPurchaseStorePanelIOS.purchaseVipSuccess();
            }
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_6, StringComparison.Ordinal)
                 ||String.Equals(args.purchasedProduct.definition.id, product_ios_7, StringComparison.Ordinal)
                 ||String.Equals(args.purchasedProduct.definition.id, product_ios_8, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品678购买成功");
            addChargeNum(3);
            SDGameManager.Instance.inPurchaseStorePanelIOS.isNoviceGiftPurchased = true;
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_9, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品9购买成功");
            addChargeNum(1);
            //测试未处理完订单
            //testNullPurchasePanel.gameObject.SetActive(false);
            if (SDGameManager.Instance.inPurchaseStorePanelIOS != null)
            {
                SDGameManager.Instance.inPurchaseStorePanelIOS.buyDoubleCoinSuccess();
            }
            else
            {
                Debug.Log(LOGTAG + "Cathy purchasePanel is null");
                string message = string.Format(SDGameManager.T("PurchaseSuccess"), 20);
                SLPopoutController.CreatePopoutMessage(message, 901, null);
                string currTime = SDDataManager.Instance.getMyCurrTime().ToString("yyyyMMddHHmmss");
                PlayerPrefs.SetString("doubleCoinStartTime", currTime);
                PlayerPrefs.SetInt("doubleCoinState", 1);
            }
        }else if (String.Equals(args.purchasedProduct.definition.id, product_ios_10, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品10购买成功");
            addChargeNum(30);
            SDGameManager.Instance.inPurchaseStorePanelIOS.purchaseIOSVipMonthCardSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_11, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品11购买成功");
            if (!SDDataManager.Instance.settingData.fondPurchaseState)
            {
                addChargeNum(98);
                SDGameManager.Instance.inPurchaseStorePanelIOS.buyFondSuccess();
            }
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_12, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品12购买成功");
            addChargeNum(6);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buySummonSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_13, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品13购买成功");
            addChargeNum(3);
            SDGameManager.Instance.inPurchaseStorePanelIOS.purchaseDailyNoviceGiftSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_14, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品14购买成功");
            addChargeNum(68);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem68Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_15, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品15购买成功");
            addChargeNum(98);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem98Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_16, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品16购买成功");
            addChargeNum(328);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem328Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_ios_17, StringComparison.Ordinal))
        {
            Debug.Log(LOGTAG + "steve 商品17购买成功");
            addChargeNum(648);
            SDGameManager.Instance.inPurchaseStorePanelIOS.buyGem648Success();
        }
        else
        {
            createPurcahseFailMsg(args.purchasedProduct.definition.id);
            Debug.Log(string.Format(LOGTAG + "ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }
#endif
#if ANDROID_GP
        // A consumable product has been purchased by this user.
        if (String.Equals(args.purchasedProduct.definition.id, product_1, StringComparison.Ordinal))
        {
            //GetComponent<UIButtonTapped>().ProductPurchaseSuccess("商品1购买成功");
            Debug.Log("steve 商品1购买成功");
            //商品1购买成功逻辑
            addChargeNum(6);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem50Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_2, StringComparison.Ordinal))
        {
            //GetComponent<UIButtonTapped>().ProductPurchaseSuccess("商品2购买成功");
            Debug.Log("steve 商品2购买成功");
            //商品2购买成功逻辑     
            addChargeNum(30);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem300Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_3, StringComparison.Ordinal))
        {
            Debug.Log("steve 商品3购买成功");
            //商品3购买成功逻辑
            addChargeNum(60);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem800Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_4, StringComparison.Ordinal))
        {
            Debug.Log("steve 商品4购买成功");
            //商品4购买成功逻辑
            addChargeNum(3);
            SDGameManager.Instance.inPurchaseStorePanelGP.purchaseNoviceGiftSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_5, StringComparison.Ordinal))
        {
            Debug.Log("steve 商品5购买成功");
            //商品5购买成功逻辑
            if (SDGameManager.Instance.inPurchaseStorePanelGP == null)
            {
                Debug.Log("cathy 商品5 purchasePanel 为空");
            SDGameManager.Instance.inPurchaseStorePanelGP = transform.GetComponent<RTPurchasePanel>();
            }
            addChargeNum(1);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyDoubleCoinSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_6, StringComparison.Ordinal))
        {
            addChargeNum(98);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyFondSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_7, StringComparison.Ordinal))
        {
            addChargeNum(6);
            SDGameManager.Instance.inPurchaseStorePanelGP.buySummonSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_8, StringComparison.Ordinal))
        {
            addChargeNum(3);
            SDGameManager.Instance.inPurchaseStorePanelGP.purchaseDailyNoviceGiftSuccess();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_9, StringComparison.Ordinal))
        {
            addChargeNum(68);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem68Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_10, StringComparison.Ordinal))
        {
            addChargeNum(98);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem98Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_11, StringComparison.Ordinal))
        {
            addChargeNum(328);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem328Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_12, StringComparison.Ordinal))
        {
            addChargeNum(648);
            SDGameManager.Instance.inPurchaseStorePanelGP.buyGem648Success();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, product_14, StringComparison.Ordinal))
        {
            if (!SDDataManager.Instance.GetSubscibeState())
            {
                addChargeNum(30);
                SDGameManager.Instance.inPurchaseStorePanelGP.purchaseVipSuccess();
            }
        }
        else
        {
            createPurcahseFailMsg(args.purchasedProduct.definition.id);
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }
#endif

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        Debug.Log(LOGTAG + "return PurchaseProcessingResult.Complete");
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
#if UNITY_IOS || ANDROID_GP
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(LOGTAG + string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if(failureReason == PurchaseFailureReason.DuplicateTransaction){
            Debug.Log(LOGTAG + string.Format("Steve恢复购买成功: '{0}'", product.definition.storeSpecificId));
        }
        createOpFailMsg();
#endif
    }
    */
}

