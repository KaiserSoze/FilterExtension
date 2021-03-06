﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FilterExtensions
{
    using ConfigNodes;
    using Utility;
    using KSP.UI.Screens;

    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    class Editor : MonoBehaviour
    {
        public static Editor instance;
        public static bool subcategoriesChecked;
        public bool ready = false;
        void Start()
        {
            instance = this;
            StartCoroutine(editorInit());
        }

        /// <summary>
        /// names of all parts that shouldn't be visible to the player
        /// </summary>
        public static HashSet<string> blackListedParts;

        IEnumerator editorInit()
        {
            ready = false;

            while (PartCategorizer.Instance == null)
                yield return null;
            if (Settings.debug)
                Core.Log("Starting on Stock Filters");
            // stock filters
            // If I edit them later everything breaks
            // custom categories can't be created at this point
            // The event which most mods will be hooking into fires after this, so they still get their subCategories even though I may clear the category
            foreach (PartCategorizer.Category C in PartCategorizer.Instance.filters)
            {
                customCategory cat;
                if (Core.Instance.Categories.TryGetValue(c => c.categoryName == C.button.categoryName, out cat) && cat.type == customCategory.categoryType.Stock)
                    cat.initialise();
            }

            // custom categories
            // wait until the part menu is initialised
            while (!PartCategorizer.Ready)
                yield return null;

            // frames after the flag is set to wait before initialising. Minimum of two for things to work consistently
            for (int i = 0; i < 4; i++)
                yield return null;
            if (Settings.debug)
                Core.Log("Starting on general categories");
            
            // all FE categories
            foreach (customCategory c in Core.Instance.Categories)
            {
                if (c.type == customCategory.categoryType.New)
                    c.initialise();
            }

            // wait again so icon edits don't occur immediately and cause breakages
            for (int i = 0; i < 4; i++)
                yield return null;
            if (Settings.debug)
                Core.Log("Starting on late categories");

            // generate the set of parts to block
            if (blackListedParts == null)
            {
                #warning not known until now which parts are never visible so some completely empty subcategories may be present on the first VAB entry
                findPartsToBlock();
            }

            // this is to be used for altering subcategories in a category added by another mod
            foreach (customCategory c in Core.Instance.Categories)
            {
                if (c.type == customCategory.categoryType.Mod)
                    c.initialise();
            }

            // 
            foreach (PartCategorizer.Category c in PartCategorizer.Instance.filters)
                namesAndIcons(c);

            // Remove any category with no subCategories (causes major breakages if selected).
            for (int i = 0; i < 4; i++)
                yield return null;
            if (Settings.debug)
                Core.Log("Starting on removing categories");
            List<PartCategorizer.Category> catsToDelete = PartCategorizer.Instance.filters.FindAll(c => c.subcategories.Count == 0);
            foreach (PartCategorizer.Category cat in catsToDelete)
            {
                PartCategorizer.Instance.scrollListMain.RemoveItem(cat.button.container, true);
                PartCategorizer.Instance.filters.Remove(cat);
            }

            // make the categories visible
            if (Settings.setAdvanced)
                PartCategorizer.Instance.SetAdvancedMode();

            for (int i = 0; i < 4; i++)
                yield return null;
            if (Settings.debug)
                Core.Log("Refreshing parts list");
            setSelectedCategory();

            subcategoriesChecked = ready = true;
        }

        /// <summary>
        /// In the editor, checks all subcategories of a category and edits their names/icons if required
        /// </summary>
        public void namesAndIcons(PartCategorizer.Category category)
        {
            HashSet<string> toRemove = new HashSet<string>();
            foreach (PartCategorizer.Category c in category.subcategories)
            {
                if (Core.Instance.removeSubCategory.Contains(c.button.categoryName))
                    toRemove.Add(c.button.categoryName);
                else
                {
                    string tmp;
                    if (Core.Instance.Rename.TryGetValue(c.button.categoryName, out tmp)) // update the name first
                        c.button.categoryName = tmp;

                    RUI.Icons.Selectable.Icon icon;
                    if (Core.tryGetIcon(tmp, out icon) || Core.tryGetIcon(c.button.categoryName, out icon)) // if there is an explicit setIcon for the subcategory or if the name matches an icon
                        c.button.SetIcon(icon); // change the icon
                }
            }
            category.subcategories.RemoveAll(c => toRemove.Contains(c.button.categoryName));
        }

        /// <summary>
        /// refresh the visible subcategories to ensure all changes are visible
        /// </summary>
        public static void setSelectedCategory()
        {
            try
            {
                PartCategorizer.Category cat;
                if (Settings.categoryDefault != string.Empty)
                {
                    cat = PartCategorizer.Instance.filters.FirstOrDefault(f => f.button.categoryName == Settings.categoryDefault);
                    if (cat != null)
                        cat.button.activeButton.SetState(KSP.UI.UIRadioButton.State.True, KSP.UI.UIRadioButton.CallType.APPLICATION, null, true);
                }

                if (Settings.subCategoryDefault != string.Empty)
                {
                    // set the subcategory button
                    cat = PartCategorizer.Instance.filters.FirstOrDefault(f => f.button.activeButton.Value);
                    if (cat != null)
                    {
                        cat = cat.subcategories.FirstOrDefault(sC => sC.button.categoryName == Settings.subCategoryDefault);
                        if (cat != null)
                            cat.button.activeButton.SetState(KSP.UI.UIRadioButton.State.True, KSP.UI.UIRadioButton.CallType.APPLICATION, null, true);
                    }
                }
            }
            catch (Exception e)
            {
                Core.Log("Category refresh failed");
                Core.Log(e.InnerException);
                Core.Log(e.StackTrace);
            }
        }

        /// <summary>
        /// checks all subcats not created by FE for visibility of parts set to "category = none"
        /// </summary>
        void findPartsToBlock()
        {
            PartModuleFilter pmf;
            // all parts that may not be visible
            List<AvailablePart> partsToCheck = PartLoader.Instance.parts.FindAll(ap => ap.category == PartCategories.none
                                                                                    && !(Core.Instance.filterModules.TryGetValue(ap.name, out pmf) && pmf.hasForceAdd()));
            // Only checking the category which should be Filter by Function (should I find FbF explcitly?)
            PartCategorizer.Category mainCat = PartCategorizer.Instance.filters[0];
            // has a reference to all the subcats that FE added to the category
            customCategory customMainCat = Core.Instance.Categories.Find(C => C.categoryName == mainCat.button.categoryName);
            // loop through the subcategories. Mark FE ones as seen incase of duplication and check the shortlisted parts against other mods categories for visibility
            HashSet<string> subCatsSeen = new HashSet<string>();
            for (int i = 0; i < mainCat.subcategories.Count; i++)
            {
                PartCategorizer.Category subCat = mainCat.subcategories[i];
                // if the name is an FE subcat and the category should have that FE subcat and it's not the duplicate of one already seen created by another mod, mark it seen and move on
                if (Core.Instance.subCategoriesDict.ContainsKey(subCat.button.categoryName) && customMainCat.subCategories.Any(subItem => string.Equals(subItem.subcategoryName, subCat.button.categoryName, StringComparison.CurrentCulture)))
                    subCatsSeen.Add(subCat.button.categoryName);
                else // subcat created by another mod
                {
                    int j = 0;
                    while (j < partsToCheck.Count)
                    {
                        if (subCat.exclusionFilter.FilterCriteria.Invoke(partsToCheck[j])) // if visible
                            partsToCheck.RemoveAt(j);
                        else
                            j++;
                    }
                }
            }
            // add the blocked parts to a hashset for later lookup
            blackListedParts = new HashSet<string>();
            foreach (AvailablePart ap in partsToCheck)
                blackListedParts.Add(ap.name);
        }
    }
}
