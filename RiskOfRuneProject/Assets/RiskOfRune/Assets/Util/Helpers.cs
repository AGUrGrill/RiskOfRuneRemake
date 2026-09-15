using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Networking;

public static class Helpers
{
    #region Item Defs
    // 99: ALL, 0: Tier 1, 1: Tier 2, 2: Tier, 3: Tier Boss
    public static List<ItemDef> GetItems(int tierIndex)
    {
        List<ItemDef> items = new List<ItemDef>();
        for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
        {
            ItemDef item = ItemCatalog.GetItemDef(i);

            if (item == null || item.ContainsTag(ItemTag.WorldUnique)) continue;

            if (tierIndex == 99) items.Add(item);
            else if (tierIndex == 0 && item.tier == ItemTier.Tier1) items.Add(item);
            else if (tierIndex == 1 && item.tier == ItemTier.Tier2) items.Add(item);
            else if (tierIndex == 2 && item.tier == ItemTier.Tier3) items.Add(item);
            else if (tierIndex == 3 && item.tier == ItemTier.Lunar) items.Add(item);
            else if (tierIndex == 4 && item.tier == ItemTier.Boss) items.Add(item);
            else if (tierIndex == 5 && item.tier == ItemTier.NoTier) items.Add(item);
            else if (tierIndex == 6 && item.tier == ItemTier.VoidTier1) items.Add(item);
            else if (tierIndex == 7 && item.tier == ItemTier.VoidTier2) items.Add(item);
            else if (tierIndex == 8 && item.tier == ItemTier.VoidTier3) items.Add(item);
            else if (tierIndex == 9 && item.tier == ItemTier.VoidBoss) items.Add(item);
            else if (tierIndex == 10 && item.tier == ItemTier.AssignedAtRuntime) items.Add(item);
        }
        return items;
    }
    public static List<ItemDef> GetAllPermenantItemsFromInventory(Inventory inv)
    {
        List<ItemDef> items = new List<ItemDef>();
        for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
        {
            ItemDef item = ItemCatalog.GetItemDef(i);
            if (item == null) continue;
            if (inv.GetItemCountPermanent(i) > 0) items.Add(item);
        }
        return items;
    }
    public static List<ItemDef> GetAllTempItemsFromInventory(Inventory inv)
    {
        List<ItemDef> items = new List<ItemDef>();
        for (ItemIndex i = 0; i < (ItemIndex)ItemCatalog.itemCount; i++)
        {
            ItemDef item = ItemCatalog.GetItemDef(i);
            if (item == null) continue;
            if (inv.GetItemCountTemp(i) > 0) items.Add(item);
        }
        return items;
    }
    #endregion

    #region Buff Defs
    // 99: ALL, 0: Buffs?, 1: Debuffs, 2: Affixs
    public static List<BuffDef> GetBuffs(int type)
    {
        List<BuffDef> buffs = new List<BuffDef>();
        FieldInfo[] fields = typeof(RoR2Content.Buffs).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            if (field.GetValue(null) is BuffDef buff)
            {
                if (buff.isHidden) continue;
                if (type == 0 && !buff.isElite && !buff.isDebuff) buffs.Add(buff);
                else if (type == 1 && buff.isDebuff) buffs.Add(buff);
                else if (type == 2 && buff.isElite) buffs.Add(buff);
                else if (type == 99) buffs.Add(buff);
            }
        }
        fields = typeof(DLC1Content.Buffs).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            if (field.GetValue(null) is BuffDef buff)
            {
                if (buff.isHidden) continue;
                if (type == 0 && !buff.isElite && !buff.isDebuff) buffs.Add(buff);
                else if (type == 1 && buff.isDebuff) buffs.Add(buff);
                else if (type == 2 && buff.isElite) buffs.Add(buff);
                else if (type == 99) buffs.Add(buff);
            }
        }
        fields = typeof(DLC2Content.Buffs).GetFields(BindingFlags.Public | BindingFlags.Static);
        foreach (var field in fields)
        {
            if (field.GetValue(null) is BuffDef buff)
            {
                if (buff.isHidden) continue;
                if (type == 0 && !buff.isElite && !buff.isDebuff) buffs.Add(buff);
                else if (type == 1 && buff.isDebuff) buffs.Add(buff);
                else if (type == 2 && buff.isElite) buffs.Add(buff);
                else if (type == 99) buffs.Add(buff);
            }
        }
        /*
        foreach (var buff in buffs)
        {
            Debug.Log("BuffNameTest: " + buff);
        }
        */
        return buffs;
    }
    #endregion

    #region Make Prefabs
    public static void CreateEffectPrefab(GameObject obj, bool isFollower)
    {
        var effect = obj.GetComponent<EffectComponent>();
        if (!effect) effect = obj.AddComponent<EffectComponent>();
        effect.applyScale = isFollower;
        effect.effectIndex = EffectIndex.Invalid;
        effect.parentToReferencedTransform = isFollower;
        effect.positionAtReferencedTransform = isFollower;

        ContentAddition.AddEffect(obj);
    }
    public static void AddEffectPrefabToContentAddition(GameObject obj)
    {
        if (!obj.GetComponent<NetworkIdentity>()) obj.AddComponent<NetworkIdentity>();
        if (!obj.GetComponent<EffectComponent>()) obj.AddComponent<EffectComponent>();
        ContentAddition.AddEffect(obj);
    }
    public static void CreateNetworkedObjectPrefab(GameObject obj)
    {
        if (!obj.GetComponent<NetworkIdentity>()) obj.AddComponent<NetworkIdentity>();
        PrefabAPI.RegisterNetworkPrefab(obj);
        ContentAddition.AddNetworkedObject(obj);
    }
    public static void CreateNetworkedProjectilePrefab(GameObject obj)
    {
        if (!obj.GetComponent<ProjectileController>()) obj.AddComponent<ProjectileController>();
        if (!obj.GetComponent<ProjectileSimple>()) obj.AddComponent<ProjectileSimple>();
        if (!obj.GetComponent<NetworkIdentity>()) obj.AddComponent<NetworkIdentity>();
        PrefabAPI.RegisterNetworkPrefab(obj);
        ContentAddition.AddProjectile(obj); 
    }
    /// <summary>
    /// Loads a prefab from RoR2 addressable assets, clones it without awakening it, applies a modifier function to the clone, then performs a second InstantiateClone operation to freeze the modified version into a new named prefab.
    /// </summary>
    public static GameObject ModifyVanillaPrefab(string addressablePath, string newName, bool shouldNetwork, System.Func<GameObject, GameObject> modifierCallback)
    {
        var origObj = Addressables.LoadAssetAsync<GameObject>(addressablePath)
            .WaitForCompletion()
            .InstantiateClone("Temporary Setup Prefab", false);
        var newObj = modifierCallback(origObj);
        var newObjPrefabified = newObj.InstantiateClone(newName, shouldNetwork);
        GameObject.Destroy(origObj);
        GameObject.Destroy(newObj);
        return newObjPrefabified;
    }

    // Using Nuxlar's Sound Solution, cause im too dum for this
    public static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
    {
        NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
        networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
        networkSoundEventDef.eventName = eventName;

        ContentAddition.AddNetworkSoundEventDef(networkSoundEventDef);

        return networkSoundEventDef;
    }
    #endregion

    #region Extras
    public static void GetAllComponentNames(GameObject obj)
    {
        foreach (var component in obj.GetComponents<Component>())
        {
            Debug.Log(obj + ": " + component);
            /*
            foreach (var componentChild in component.GetComponents<Component>())
            {
                Debug.Log(component + ": " + componentChild);
            }
            */
        }
    }

    public static void GetAllTransformNames(GameObject obj)
    {
        foreach (Transform child in obj.GetComponentsInChildren<Transform>())
        {
            Debug.Log(child.parent.name + ": " + child.name);
        }
    }
    #endregion

    #region Interactable UI
    // Thank you to viliger for this code from Shrine of Repair
    /*
    public static void AddPersistentListener(this UnityEvent<MPButton, PickupDef> unityEvent, UnityAction<MPButton, PickupDef> action)
    {
        unityEvent.m_PersistentCalls.AddListener(new PersistentCall
        {
            m_Target = action.Target as UnityEngine.Object,
            m_TargetAssemblyTypeName = UnityEventTools.TidyAssemblyTypeName(action.Method.DeclaringType.AssemblyQualifiedName),
            m_MethodName = action.Method.Name,
            m_CallState = UnityEventCallState.RuntimeOnly,
            m_Mode = PersistentListenerMode.EventDefined,
        });
    }

    public static void AddPersistentListener(this UnityEvent<int> unityEvent, UnityAction<int> action)
    {
        unityEvent.m_PersistentCalls.AddListener(new PersistentCall
        {
            m_Target = action.Target as UnityEngine.Object,
            m_TargetAssemblyTypeName = UnityEventTools.TidyAssemblyTypeName(action.Method.DeclaringType.AssemblyQualifiedName),
            m_MethodName = action.Method.Name,
            m_CallState = UnityEventCallState.RuntimeOnly,
            m_Mode = PersistentListenerMode.EventDefined,
        });
    }

    public static void AddPersistentListener(this UnityEvent<Interactor> unityEvent, UnityAction<Interactor> action)
    {
        unityEvent.m_PersistentCalls.AddListener(new PersistentCall
        {
            m_Target = action.Target as UnityEngine.Object,
            m_TargetAssemblyTypeName = UnityEventTools.TidyAssemblyTypeName(action.Method.DeclaringType.AssemblyQualifiedName),
            m_MethodName = action.Method.Name,
            m_CallState = UnityEventCallState.RuntimeOnly,
            m_Mode = PersistentListenerMode.EventDefined,
        });
    }
    #endregion

    #region Elite Gradient
    // Code extracted from Nuxlar's MoreElites
    public static Texture2D CreateGradientTexture(Color32[] colors, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.wrapMode = TextureWrapMode.Clamp;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Calculate the horizontal position as a value between 0 and 1
                float t = (float)x / (width - 1);

                // Determine which colors to interpolate between
                float scaledT = t * (colors.Length - 1);
                int colorIndex = Mathf.FloorToInt(scaledT);
                float lerpFactor = scaledT - colorIndex;

                // Ensure the last color is not out of bounds
                if (colorIndex >= colors.Length - 1)
                {
                    colorIndex = colors.Length - 2;
                    lerpFactor = 1.0f;
                }

                // Interpolate between the two colors
                Color32 color = LerpColor32(colors[colorIndex], colors[colorIndex + 1], lerpFactor);

                // Set the pixel color
                texture.SetPixel(x, y, color);
            }
        }

        // Apply changes to the texture
        texture.Apply();
        //DeltaruneMod.DeltarunePlugin.malachiteOverlayMat.SetTexture("_RemapTex", texture);

        return texture;
    }

    public static Color32 LerpColor32(Color32 colorA, Color32 colorB, float t)
    {
        byte r = (byte)Mathf.Lerp(colorA.r, colorB.r, t);
        byte g = (byte)Mathf.Lerp(colorA.g, colorB.g, t);
        byte b = (byte)Mathf.Lerp(colorA.b, colorB.b, t);
        byte a = (byte)Mathf.Lerp(colorA.a, colorB.a, t);
        return new Color32(r, g, b, a);
    }
    */

    #endregion


    #region UTIL CODE FROM THINKINVIS
    /// <summary>
    /// Returns a list of enemy TeamComponents given an ally team (to ignore while friendly fire is off) and a list of ignored teams (to ignore under all circumstances).
    /// </summary>
    /// <param name="allyIndex">The team to ignore if friendly fire is off.</param>
    /// <param name="ignore">Additional teams to always ignore.</param>
    /// <returns>A list of all TeamComponents that match the provided team constraints.</returns>
    public static List<TeamComponent> GatherEnemies(TeamIndex allyIndex, params TeamIndex[] ignore)
    {
        var retv = new List<TeamComponent>();
        bool isFF = FriendlyFireManager.friendlyFireMode != FriendlyFireManager.FriendlyFireMode.Off;
        var scan = ((TeamIndex[])Enum.GetValues(typeof(TeamIndex))).Except(ignore);
        foreach (var ind in scan)
        {
            if (isFF || allyIndex != ind)
                retv.AddRange(TeamComponent.GetTeamMembers(ind));
        }
        return retv;
    }

    /// <summary>
    /// Iterates towards the root of a GameObject, including jumping through EntityLocators.
    /// </summary>
    /// <param name="target">The GameObject to search for the 'true' root of.</param>
    /// <param name="maxSearch">The maximum amount of recursion to go through.</param>
    /// <returns>Null if the given object was null; the most top-level object with the given constraints otherwise.</returns>
    public static GameObject GetRootWithLocators(GameObject target, int maxSearch = 5)
    {
        if (!target) return null;
        GameObject scan = target;
        for (int i = 0; i < maxSearch; i++)
        {
            if (scan.TryGetComponent<EntityLocator>(out var eloc) && eloc.entity)
            {
                scan = eloc.entity;
                continue;
            }

            var next = scan.transform.root;
            if (next && next.gameObject != scan)
                scan = next.gameObject;
            else
                return scan;
        }
        return scan;
    }
    #endregion
}
