using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
namespace YF.Art
{
    [ExecuteInEditMode]
    public class SkillToolsRoot_Role : SkillToolsRoot
    {
        private bool IsChangeState = false;
        private string stateName = "Idle";
        private string dirName = "Right";
        private SkeletonAnimation[] RoleData = new SkeletonAnimation[3];
        /// <summary>
        /// Start is called on the frame when a script is enabled just before
        /// any of the Update methods is called the first time.
        /// </summary>
        void Start()
        {
            this.RoleData = this.GetComponentsInChildren<SkeletonAnimation>();
            foreach (SkeletonAnimation item in this.RoleData)
            {
                if (item == null) continue;
                // item.AnimationState.Complete += completeEvent;
                item.skeleton.SetSlotsToSetupPose();
                item.AnimationState.SetAnimation(0, this.CombinState("Idle", "Right"), true);
            }
        }
        public override void _Update()
        {
            this._UpdateState();
        }


        public override void setDirIndex(int index, float Far = 0)
        {
            if (this.dirindex != index)
            {
                IsChangeState = true;
            }
            this.dirindex = index;
        }
        protected override void setDir()
        {
            switch (this.dirindex)
            {
                case 0:
                    dirName = "Up";
                    this.invDir();
                    break;
                case 1:
                case 2:
                case 3:
                    dirName = "Right";
                    this.invDir();
                    break;
                case 4:
                    dirName = "Down";
                    this.invDir();
                    break;
                case 5:
                case 6:
                case 7:
                    dirName = "Right";
                    this.invDir(-1);
                    break;
                default:
                    this.invDir();
                    break;
            }
        }
        public void invDir(float value = 1)
        {
            Vector3 s = this.transform.localScale;
            s.x = value;
            this.transform.localScale = s;
        }

        public virtual string CombinState(string _stateName, string _dirName)
        {
            return _dirName + "_" + _stateName;
        }
        public virtual string CombinState()
        {
            return this.dirName + "_" + this.stateName;
        }

        public virtual void ChangeState(string _stateName)
        {
            if (this.IsChangeState)
            {
                if (string.IsNullOrEmpty(_stateName))
                {
                    return;
                }
                if (this.CombinState(_stateName, this.dirName) == this.CombinState())
                {
                    this.IsChangeState = false;
                    return;
                }
                this.stateName = _stateName;
            }
        }

        protected virtual void _UpdateState()
        {
            if (this.transform.localToWorldMatrix != Matrix4x4.identity)
            {
                this.transform.position = Vector3.zero;
                this.transform.rotation = Quaternion.identity;
                this.transform.localScale = Vector3.one;
            }
            this.setDir();
            if (this.IsChangeState)
            {
                foreach (SkeletonAnimation item in this.RoleData)
                {
                    if (item == null) continue;
                    item.skeleton.SetSlotsToSetupPose();
                    item.AnimationState.SetAnimation(0, this.CombinState(), true);
                }
                this.IsChangeState = false;
            }
        }

        // public void completeEvent(Spine.TrackEntry trackEntry)
        // {
        //     if(this.IsChangeState)
        // }
    }
}