using System;
using MonoMod;
using UnityEngine.UI;

#pragma warning disable 1591, 0649

namespace Modding.Patches
{
    [MonoModPatch("global::SetVersionNumber")]
    public class SetVersionNumber : global::SetVersionNumber
    {
        [MonoModIgnore]
        private Text textUi;

        [MonoModReplace]
        private void Start()
        {
            if (textUi != null)
            {
                string text = "1.3.3.7";
                textUi.text = text; 
            }
        }
    }
}
