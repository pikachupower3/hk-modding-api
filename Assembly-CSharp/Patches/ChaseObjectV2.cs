﻿using HutongGames.PlayMaker;
using MonoMod;

// ReSharper disable All
#pragma warning disable 1591, 0108, 0169, 0649, 0414

namespace Modding.Patches
{
    [MonoModPatch("HutongGames.PlayMaker.Actions.ChaseObjectV2")]
    public class ChaseObjectV2 : HutongGames.PlayMaker.Actions.ChaseObjectV2
    {
        [MonoModIgnore]
        public FsmOwnerDefault gameObject;

        [MonoModIgnore]
        public FsmGameObject target;

        private extern void orig_DoChase();

        //Added checks for null and an attempt to fix any missing references
        //as well as a try/catch in case something goes wrong to keep the whole FSM from breaking down...
        private void DoChase()
        {
            try
            {
                if (target == null || target.Value == null)
                {
                    target = new HutongGames.PlayMaker.FsmGameObject(HeroController.instance?.proxyFSM.Fsm.GameObject);
                }

                if (gameObject == null || gameObject.GameObject == null || gameObject.GameObject.Value == null)
                {
                    if (gameObject == null)
                    {
                        gameObject = new HutongGames.PlayMaker.FsmOwnerDefault();
                        gameObject.OwnerOption = HutongGames.PlayMaker.OwnerDefaultOption.UseOwner;
                    }

                    gameObject.GameObject = new HutongGames.PlayMaker.FsmGameObject(Fsm.GameObject);
                }

                if ((gameObject == null || gameObject.GameObject == null || gameObject.GameObject.Value == null) || (target == null || target.Value == null))
                {
                    base.Finish();
                    return;
                }

                orig_DoChase();
            }
            catch (System.Exception ex)
            {
                Logger.APILogger.LogError(ex);
                base.Finish();
            }
        }
    }
}