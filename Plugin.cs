using BepInEx;
using HarmonyLib;
using MyBox;
using System.Collections.Generic;

namespace RestockerPaymentFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony("com.orpticon.DisableShelves");
            harmony.PatchAll();
        }
    }
    public static class RestockerPaymentFixPatch
    {
        [HarmonyPatch(typeof(EmployeeManager))]
        [HarmonyPatch("OverduePayments", MethodType.Getter)]
        public static class EmployeeManager_OverduePayments_Patch
        {
            public static void Postfix(EmployeeManager __instance, List<PlayerPaymentData> __result)
            {
                if (__instance.m_RestockersData.Count <= 0)
                {
                    return;
                }
                float num = 0f;
                foreach (int id in __instance.m_RestockersData)
                {
                    RestockerSO restockerSO = Singleton<IDManager>.Instance.RestockerSO(id);
                    num += restockerSO.DailyWage;
                }
                PlayerPaymentData item = new PlayerPaymentData
                {
                    Amount = num,
                    PaymentType = PlayerPaymentType.STAFF
                };
                __result.Add(item);
            }
        }
    }
}
