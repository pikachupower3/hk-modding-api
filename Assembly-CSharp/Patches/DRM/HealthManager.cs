﻿using System.Collections;
using MonoMod;
//We don't care about XML docs for these as they are being patched into the original code
#pragma warning disable 1591
#pragma warning disable CS0108

namespace Modding.Patches
{
    public partial class HealthManager : global::HealthManager
    {
        [MonoModIgnore]
        public bool isDead;
        
        ///This may be used by mods to find new enemies. Check this isDead flag to see if they're already dead
        [MonoModReplace]
        protected IEnumerator CheckPersistence()
        {
            yield return null;
            //We insert the hook here because I think some enemys' FSMs need 1 frame to mark the "isDead" bool for things that it thinks should be dead.
            isDead = ModHooks.Instance.OnEnableEnemy( gameObject, isDead );
            if( this.isDead )
            {
                base.gameObject.SetActive( false );
            }
            yield break;
        }
    }
}
