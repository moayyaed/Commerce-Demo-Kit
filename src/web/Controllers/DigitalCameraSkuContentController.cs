﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EPiServer.Commerce.Catalog;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Localization;
using EPiServer.Framework.Web.Mvc;
using EPiServer.ServiceLocation;
using Mediachase.Commerce;
using Mediachase.Commerce.Inventory;
using Mediachase.Commerce.Pricing;
using OxxCommerceStarterKit.Core.Extensions;
using OxxCommerceStarterKit.Web.Business.Analytics;
using OxxCommerceStarterKit.Web.Models.Catalog;
using OxxCommerceStarterKit.Web.Models.ViewModels;

namespace OxxCommerceStarterKit.Web.Controllers
{
    [TemplateDescriptor(Inherited = true)]
    [RequireClientResources]
    public class DigitalCameraSkuContentController : CommerceControllerBase<DigitalCameraSkuContent>
    {
         private readonly ICurrentMarket _currentMarket;
        private IWarehouseInventoryService _warehouseInventoryService;
        private LocalizationService _localizationService;
        private ReadOnlyPricingLoader _readOnlyPricingLoader;
        private readonly IPriceDetailService _priceDetailService;

        public DigitalCameraSkuContentController()
			: this(ServiceLocator.Current.GetInstance<IWarehouseInventoryService>(),
			ServiceLocator.Current.GetInstance<LocalizationService>(),
			ServiceLocator.Current.GetInstance<ReadOnlyPricingLoader>(),
			ServiceLocator.Current.GetInstance<ICurrentMarket>(),
            ServiceLocator.Current.GetInstance<IPriceDetailService>()
			)
		{
		}
        public DigitalCameraSkuContentController(IWarehouseInventoryService warehouseInventoryService, LocalizationService localizationService, ReadOnlyPricingLoader readOnlyPricingLoader, ICurrentMarket currentMarket, IPriceDetailService priceDetailService)
        {
            _warehouseInventoryService = warehouseInventoryService;
            _localizationService = localizationService;
            _readOnlyPricingLoader = readOnlyPricingLoader;
            _currentMarket = currentMarket;
            _priceDetailService = priceDetailService;
        }

        
        // GET: DigitalCameraSkuContent
        public ActionResult Index(DigitalCameraSkuContent currentContent)
        {
            DigitalCameraSkuViewModel digitalCameraSkuViewModel = new DigitalCameraSkuViewModel(currentContent);
            digitalCameraSkuViewModel.PriceViewModel = GetPriceModel(currentContent);

            TrackAnalytics(digitalCameraSkuViewModel);

            digitalCameraSkuViewModel.IsSellable = IsSellable(currentContent);
            return View(digitalCameraSkuViewModel);
        }

        protected void TrackAnalytics(DigitalCameraSkuViewModel digitalCameraSkuViewModel)
        {
            // Track
            GoogleAnalyticsTracking tracking = new GoogleAnalyticsTracking(ControllerContext.HttpContext);
            tracking.ClearInteractions();

            // Track the main product view
            tracking.ProductAdd(
                digitalCameraSkuViewModel.CatalogContent.Code,
                digitalCameraSkuViewModel.CatalogContent.DisplayName,
                null,
                digitalCameraSkuViewModel.CatalogContent.Facet_Brand,
                null, null, 0,
                (double)digitalCameraSkuViewModel.CatalogContent.GetDefaultPriceAmount(_currentMarket.GetCurrentMarket()),
                0
                );

            // TODO: Track related products as impressions

            // Track action as details view
            tracking.Action("detail");
        }
    }
}