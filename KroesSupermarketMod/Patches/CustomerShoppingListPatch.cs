using HarmonyLib;
using TMPro;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KroesSupermarketMod.Patches
{
    [HarmonyPatch]
    internal class CustomerShoppingListPatch
    {
        private static GameObject shoppingListUI;
        private static NPC_Info currentTargetNPC;

        // References for dynamic live-updating
        private static Transform shoppingListContentParent;
        private static Transform cartListContentParent;
        private static LayoutElement shoppingScrollViewLayout;
        private static LayoutElement cartScrollViewLayout;

        private static int lastKnownToBuyCount = -1;
        private static int lastKnownCarryingCount = -1;

        [HarmonyPatch(typeof(PlayerNetwork), "Update")]
        [HarmonyPostfix]
        private static void UpdatePostfix(PlayerNetwork __instance)
        {
            if (shoppingListUI != null)
            {
                // Close UI on ESC key
                if (Input.GetKeyDown(KeyCode.Escape))
                {
                    CloseShoppingList();
                    return;
                }

                // Check for dynamic live changes in products to buy or carrying lists
                if (currentTargetNPC != null)
                {
                    int currentToBuyCount = currentTargetNPC.productsIDToBuy != null ? currentTargetNPC.productsIDToBuy.Count : 0;
                    int currentCarryingCount = currentTargetNPC.productsIDCarrying != null ? currentTargetNPC.productsIDCarrying.Count : 0;

                    if (currentToBuyCount != lastKnownToBuyCount || currentCarryingCount != lastKnownCarryingCount)
                    {
                        RefreshBothLists();
                    }
                }
                return;
            }

            // Raycast click detection to open UI
            if (Input.GetMouseButtonDown(0) && __instance.equippedItem == 0)
            {
                RaycastHit hit;
                int layerMask = LayerMask.GetMask("Default");
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10f, layerMask))
                {
                    if (hit.transform.CompareTag("Interactable") && hit.transform.gameObject.name.Equals("HitTrigger"))
                    {
                        if (hit.transform.parent != null)
                        {
                            Transform npcAgent = hit.transform.parent;
                            NPC_Info npcInfo = npcAgent.GetComponent<NPC_Info>();

                            if (npcInfo != null && npcInfo.isCustomer)
                            {
                                CustomCameraController controller = Camera.main.GetComponent<CustomCameraController>();
                                if (controller != null)
                                {
                                    controller.isInCameraEvent = true;
                                }

                                currentTargetNPC = npcInfo;
                                shoppingListUI = CreateShoppingListUI(npcInfo);
                                Cursor.lockState = CursorLockMode.None;
                                Cursor.visible = true;
                            }
                        }
                    }
                }
            }
        }

        private static readonly string[] names = new string[]
        {
            "Jensen Ackles",
            "Jared Padalecki",
            "Misha Collins",
            "Mark Sheppard",
            "Jeffrey Dean Morgan",
            "Jim Beaver",
            "Genevieve Padalecki",
            "Alexander Calvert",
            "Ruth Connell",
            "Danneel Ackles",
            "Mark Pellegrino",
            "Samantha Smith",
            "Adrianne Palicki",
            "Rob Benedict",
            "Richard Speight Jr.",
            "Katie Cassidy",
            "Lauren Cohan",
            "Felicia Day",
            "David Haydn-Jones",
            "DJ Qualls",
            "Ty Olsson"
        };

        private static string ProductIdToName(int productID)
        {
            string key = "product" + productID.ToString();
            return LocalizationManager.instance.GetLocalizationString(key);
        }

        private static Sprite ProductIdToSprite(int productID) { return ProductListing.Instance.productsData[productID].productSprite; }

        private static Color ProductIdToColor(int productID) { return ProductListing.Instance.productsData[productID].productColor; }

        private static GameObject CreateShoppingListUI(NPC_Info npcInfo)
        {
            string customerName = names[npcInfo.NPCID % names.Length];
            Transform contentParent = GameCanvas.Instance.transform;

            // 1. Root Container
            GameObject rootGo = new GameObject("ShoppingListUI", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            rootGo.transform.SetParent(contentParent, false);

            RectTransform rootRect = rootGo.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);

            VerticalLayoutGroup rootLayout = rootGo.GetComponent<VerticalLayoutGroup>();
            rootLayout.childControlWidth = true;
            rootLayout.childControlHeight = true;
            rootLayout.childForceExpandWidth = true;
            rootLayout.childForceExpandHeight = false;
            rootLayout.spacing = 12;

            ContentSizeFitter rootFitter = rootGo.GetComponent<ContentSizeFitter>();
            rootFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            rootFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // 2. Header Row Container (Customer Name + Red Close Button)
            GameObject headerRowGo = new GameObject("HeaderRow", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            headerRowGo.transform.SetParent(rootGo.transform, false);

            LayoutElement headerRowLayout = headerRowGo.GetComponent<LayoutElement>();
            headerRowLayout.preferredWidth = 1000;
            headerRowLayout.preferredHeight = 65;

            HorizontalLayoutGroup headerRowGroup = headerRowGo.GetComponent<HorizontalLayoutGroup>();
            headerRowGroup.childControlWidth = true;
            headerRowGroup.childControlHeight = true;
            headerRowGroup.childForceExpandWidth = false;
            headerRowGroup.childForceExpandHeight = true;
            headerRowGroup.childAlignment = TextAnchor.MiddleLeft;
            headerRowGroup.spacing = 12;

            // 2a. Extended Name Panel
            GameObject namePanelGo = new GameObject("NamePanel", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            namePanelGo.transform.SetParent(headerRowGo.transform, false);

            Image namePanelBg = namePanelGo.GetComponent<Image>();
            namePanelBg.color = Color.white;

            LayoutElement namePanelLayout = namePanelGo.GetComponent<LayoutElement>();
            namePanelLayout.flexibleWidth = 1f;
            namePanelLayout.preferredHeight = 65;

            HorizontalLayoutGroup namePanelGroup = namePanelGo.GetComponent<HorizontalLayoutGroup>();
            namePanelGroup.childControlWidth = true;
            namePanelGroup.childControlHeight = true;
            namePanelGroup.childForceExpandWidth = true;
            namePanelGroup.childForceExpandHeight = true;
            namePanelGroup.childAlignment = TextAnchor.MiddleLeft;
            namePanelGroup.padding = new RectOffset(20, 20, 0, 0);

            GameObject headerGo = new GameObject("CustomerHeader", typeof(RectTransform), typeof(TextMeshProUGUI));
            headerGo.transform.SetParent(namePanelGo.transform, false);

            TextMeshProUGUI headerText = headerGo.GetComponent<TextMeshProUGUI>();
            headerText.text = customerName;
            headerText.fontSize = 28;
            headerText.fontStyle = FontStyles.Bold;
            headerText.color = Color.black;
            headerText.alignment = TextAlignmentOptions.Left;
            headerText.enableWordWrapping = false;

            // 2b. Red Square Close Button
            GameObject closeButtonGo = new GameObject("CloseButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));
            closeButtonGo.transform.SetParent(headerRowGo.transform, false);

            Image buttonImage = closeButtonGo.GetComponent<Image>();
            buttonImage.color = Color.red;
            buttonImage.raycastTarget = true;

            Button closeButton = closeButtonGo.GetComponent<Button>();
            closeButton.onClick.AddListener(CloseShoppingList);

            LayoutElement closeButtonLayout = closeButtonGo.GetComponent<LayoutElement>();
            closeButtonLayout.preferredWidth = 65;
            closeButtonLayout.preferredHeight = 65;
            closeButtonLayout.flexibleWidth = 0f;

            GameObject xTextGo = new GameObject("Text", typeof(RectTransform), typeof(TextMeshProUGUI));
            xTextGo.transform.SetParent(closeButtonGo.transform, false);

            RectTransform xTextRect = xTextGo.GetComponent<RectTransform>();
            xTextRect.anchorMin = Vector2.zero;
            xTextRect.anchorMax = Vector2.one;
            xTextRect.sizeDelta = Vector2.zero;

            TextMeshProUGUI xText = xTextGo.GetComponent<TextMeshProUGUI>();
            xText.text = "X";
            xText.fontStyle = FontStyles.Bold;
            xText.fontSize = 38;
            xText.color = Color.white;
            xText.alignment = TextAlignmentOptions.Center;
            xText.raycastTarget = false;

            // 3. Side-by-Side Lists Body Container
            GameObject listsBodyGo = new GameObject("ListsBody", typeof(RectTransform), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            listsBodyGo.transform.SetParent(rootGo.transform, false);

            LayoutElement listsBodyLayout = listsBodyGo.GetComponent<LayoutElement>();
            listsBodyLayout.preferredWidth = 1000;

            HorizontalLayoutGroup listsBodyGroup = listsBodyGo.GetComponent<HorizontalLayoutGroup>();
            listsBodyGroup.childControlWidth = true;
            listsBodyGroup.childControlHeight = true;
            listsBodyGroup.childForceExpandWidth = true;
            listsBodyGroup.childForceExpandHeight = false; // Prevents lists body from stretching vertically
            listsBodyGroup.spacing = 16;
            listsBodyGroup.childAlignment = TextAnchor.UpperLeft;

            // Build Column 1 (Shopping List)
            shoppingListContentParent = CreateListColumn(listsBodyGo.transform, "Shopping List", out shoppingScrollViewLayout);

            // Build Column 2 (In Cart)
            cartListContentParent = CreateListColumn(listsBodyGo.transform, "In Cart", out cartScrollViewLayout);

            // Initial Population
            RefreshBothLists();

            return rootGo;
        }

        private static Transform CreateListColumn(Transform parent, string titleText, out LayoutElement scrollViewLayout)
        {
            // Column Container
            GameObject colGo = new GameObject($"{titleText}_Column", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(LayoutElement));
            colGo.transform.SetParent(parent, false);

            LayoutElement colLayout = colGo.GetComponent<LayoutElement>();
            colLayout.flexibleWidth = 1f;

            VerticalLayoutGroup colGroup = colGo.GetComponent<VerticalLayoutGroup>();
            colGroup.childControlWidth = true;
            colGroup.childControlHeight = true;
            colGroup.childForceExpandWidth = true;
            colGroup.childForceExpandHeight = false; // Strictly control heights of children
            colGroup.spacing = 8;

            // Section Header Bar (FIXED HEIGHT)
            GameObject titleBarGo = new GameObject("TitleBar", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement));
            titleBarGo.transform.SetParent(colGo.transform, false);

            Image titleBarBg = titleBarGo.GetComponent<Image>();
            titleBarBg.color = new Color(0.2f, 0.2f, 0.2f, 1f);

            // Lock header bar height strictly to 40px using preferredHeight + minHeight
            LayoutElement titleBarLayout = titleBarGo.GetComponent<LayoutElement>();
            titleBarLayout.preferredHeight = 40;
            titleBarLayout.minHeight = 40;

            HorizontalLayoutGroup titleBarGroup = titleBarGo.GetComponent<HorizontalLayoutGroup>();
            titleBarGroup.childControlWidth = true;
            titleBarGroup.childControlHeight = false; // Prevent TMP text from forcing bar taller
            titleBarGroup.childForceExpandWidth = true;
            titleBarGroup.childForceExpandHeight = false;
            titleBarGroup.padding = new RectOffset(16, 16, 0, 0);
            titleBarGroup.childAlignment = TextAnchor.MiddleLeft;

            GameObject titleTextGo = new GameObject("TitleText", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
            titleTextGo.transform.SetParent(titleBarGo.transform, false);

            LayoutElement textLayout = titleTextGo.GetComponent<LayoutElement>();
            textLayout.preferredHeight = 40;

            TextMeshProUGUI titleTmp = titleTextGo.GetComponent<TextMeshProUGUI>();
            titleTmp.text = titleText;
            titleTmp.fontSize = 20;
            titleTmp.fontStyle = FontStyles.Bold;
            titleTmp.color = Color.white;
            titleTmp.alignment = TextAlignmentOptions.Left;

            // Scroll View Container (Dynamic height based on content size)
            GameObject scrollViewGo = new GameObject("ScrollView", typeof(RectTransform), typeof(Image), typeof(ScrollRect), typeof(Mask), typeof(LayoutElement));
            scrollViewGo.transform.SetParent(colGo.transform, false);

            Image scrollBg = scrollViewGo.GetComponent<Image>();
            scrollBg.color = Color.white;

            Mask mask = scrollViewGo.GetComponent<Mask>();
            mask.showMaskGraphic = true;

            scrollViewLayout = scrollViewGo.GetComponent<LayoutElement>();
            scrollViewLayout.preferredHeight = 40; // Starts small if empty
            scrollViewLayout.minHeight = 40;       // Small compact box when zero items

            // Viewport & Content
            GameObject viewportGo = new GameObject("Viewport", typeof(RectTransform));
            viewportGo.transform.SetParent(scrollViewGo.transform, false);

            RectTransform viewportRect = viewportGo.GetComponent<RectTransform>();
            viewportRect.anchorMin = Vector2.zero;
            viewportRect.anchorMax = Vector2.one;
            viewportRect.sizeDelta = Vector2.zero;

            GameObject contentGo = new GameObject("Content", typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            contentGo.transform.SetParent(viewportGo.transform, false);

            RectTransform contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0f, 1f);
            contentRect.sizeDelta = Vector2.zero;

            VerticalLayoutGroup contentLayout = contentGo.GetComponent<VerticalLayoutGroup>();
            contentLayout.padding = new RectOffset(12, 12, 12, 12);
            contentLayout.spacing = 8;
            contentLayout.childControlWidth = true;
            contentLayout.childControlHeight = true;
            contentLayout.childForceExpandWidth = true;

            ContentSizeFitter contentFitter = contentGo.GetComponent<ContentSizeFitter>();
            contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scrollRect = scrollViewGo.GetComponent<ScrollRect>();
            scrollRect.viewport = viewportRect;
            scrollRect.content = contentRect;
            scrollRect.horizontal = false;
            scrollRect.vertical = true;

            return contentGo.transform;
        }

        private static void RefreshBothLists()
        {
            if (currentTargetNPC == null) return;

            // Refresh Column 1: Shopping List
            if (shoppingListContentParent != null && currentTargetNPC.productsIDToBuy != null)
            {
                PopulateListContent(shoppingListContentParent, currentTargetNPC.productsIDToBuy);
                lastKnownToBuyCount = currentTargetNPC.productsIDToBuy.Count;
                AdjustScrollViewHeight(shoppingListContentParent, shoppingScrollViewLayout);
            }

            // Refresh Column 2: In Cart
            if (cartListContentParent != null && currentTargetNPC.productsIDCarrying != null)
            {
                PopulateListContent(cartListContentParent, currentTargetNPC.productsIDCarrying);
                lastKnownCarryingCount = currentTargetNPC.productsIDCarrying.Count;
                AdjustScrollViewHeight(cartListContentParent, cartScrollViewLayout);
            }
        }

        private static void PopulateListContent(Transform contentParent, List<int> productIds)
        {
            // Clear existing rows
            for (int i = contentParent.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(contentParent.GetChild(i).gameObject);
            }

            // Populate items
            for (int i = 0; i < productIds.Count; i++)
            {
                int productId = productIds[i];

                GameObject rowGo = new GameObject($"Item_{i}", typeof(RectTransform), typeof(Image), typeof(HorizontalLayoutGroup), typeof(LayoutElement), typeof(ContentSizeFitter));
                rowGo.transform.SetParent(contentParent, false);

                Image rowBg = rowGo.GetComponent<Image>();
                rowBg.color = new Color(0.95f, 0.95f, 0.95f, 1f);

                LayoutElement rowLayout = rowGo.GetComponent<LayoutElement>();
                rowLayout.minHeight = 52;

                ContentSizeFitter rowFitter = rowGo.GetComponent<ContentSizeFitter>();
                rowFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

                HorizontalLayoutGroup rowLayoutGroup = rowGo.GetComponent<HorizontalLayoutGroup>();
                rowLayoutGroup.childControlWidth = false;
                rowLayoutGroup.childControlHeight = false;
                rowLayoutGroup.childForceExpandWidth = false;
                rowLayoutGroup.childForceExpandHeight = false;
                rowLayoutGroup.childAlignment = TextAnchor.MiddleLeft;
                rowLayoutGroup.padding = new RectOffset(8, 8, 6, 6);
                rowLayoutGroup.spacing = 12;

                // Product Icon Box
                GameObject iconBoxGo = new GameObject("IconBox", typeof(RectTransform), typeof(Image), typeof(LayoutElement));
                iconBoxGo.transform.SetParent(rowGo.transform, false);

                Image iconBoxBg = iconBoxGo.GetComponent<Image>();
                iconBoxBg.color = ProductIdToColor(productId);

                LayoutElement iconBoxLayout = iconBoxGo.GetComponent<LayoutElement>();
                iconBoxLayout.preferredWidth = 42;
                iconBoxLayout.preferredHeight = 42;

                GameObject iconGo = new GameObject("ProductIcon", typeof(RectTransform), typeof(Image));
                iconGo.transform.SetParent(iconBoxGo.transform, false);

                RectTransform iconRect = iconGo.GetComponent<RectTransform>();
                iconRect.anchorMin = Vector2.zero;
                iconRect.anchorMax = Vector2.one;
                iconRect.sizeDelta = new Vector2(-6, -6);

                Image iconImage = iconGo.GetComponent<Image>();
                iconImage.sprite = ProductIdToSprite(productId);
                iconImage.preserveAspect = true;

                // Product Name Text
                GameObject nameGo = new GameObject("ProductName", typeof(RectTransform), typeof(TextMeshProUGUI), typeof(LayoutElement));
                nameGo.transform.SetParent(rowGo.transform, false);

                TextMeshProUGUI nameText = nameGo.GetComponent<TextMeshProUGUI>();
                nameText.text = ProductIdToName(productId);
                nameText.fontSize = 18;
                nameText.fontStyle = FontStyles.Bold;
                nameText.color = Color.black;
                nameText.alignment = TextAlignmentOptions.Left;
                nameText.enableWordWrapping = true;
                nameText.overflowMode = TextOverflowModes.Overflow;

                LayoutElement nameLayout = nameGo.GetComponent<LayoutElement>();
                nameLayout.preferredWidth = 360;
            }
        }

        private static void AdjustScrollViewHeight(Transform contentParent, LayoutElement layoutElement)
        {
            Canvas.ForceUpdateCanvases();
            if (layoutElement != null && contentParent != null)
            {
                RectTransform contentRect = contentParent.GetComponent<RectTransform>();
                float contentHeight = contentRect.sizeDelta.y;

                if (contentParent.childCount == 0)
                {
                    // 0 items: Minimal height box (40px)
                    layoutElement.preferredHeight = 40f;
                }
                else
                {
                    // Dynamic height based on amount of items, capped at 500px maximum
                    layoutElement.preferredHeight = Mathf.Min(contentHeight, 500f);
                }
            }
        }

        private static void CloseShoppingList()
        {
            if (shoppingListUI == null) return;

            Object.Destroy(shoppingListUI);
            shoppingListUI = null;
            currentTargetNPC = null;
            shoppingListContentParent = null;
            cartListContentParent = null;
            shoppingScrollViewLayout = null;
            cartScrollViewLayout = null;
            lastKnownToBuyCount = -1;
            lastKnownCarryingCount = -1;

            CustomCameraController controller = Camera.main.GetComponent<CustomCameraController>();
            if (controller != null)
            {
                controller.isInCameraEvent = false;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}