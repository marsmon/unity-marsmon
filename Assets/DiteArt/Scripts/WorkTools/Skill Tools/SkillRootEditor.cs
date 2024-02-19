using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
namespace YF.Art
{
    [ExecuteInEditMode]
    public class SkillRootEditor : MonoBehaviour
    {

        public bool isspAnim = false;
        private string dirName = "Right";
        private string stateName = "Idle";


        string _dirname = "Right";
        bool isChangeState = false;
        int dirindex = 2;
        Vector3 dir = Vector3.zero;

        void Awake()
        {
            isChangeState = true;
        }
        void Update()
        {
            dir = Vector3.zero;
            if (Input.GetKey(KeyCode.A))
            {
                _dirname = "Right";
                dir.x = -1;
                dirindex = 6;
            }
            if (Input.GetKey(KeyCode.D))
            {
                _dirname = "Right";
                dir.x = 1;
                dirindex = 2;
            }
            if (Input.GetKey(KeyCode.W))
            {
                _dirname = "Up";
                dir.z = 1;
                dirindex = 0;
            }
            if (Input.GetKey(KeyCode.S))
            {
                _dirname = "Down";
                dir.z = -1;
                dirindex = 4;
            }
            dirindex = dir.z == 1 && dir.x == 1 ? 1 : dirindex;
            dirindex = dir.z == -1 && dir.x == 1 ? 3 : dirindex;
            dirindex = dir.z == 1 && dir.x == -1 ? 7 : dirindex;
            dirindex = dir.z == -1 && dir.x == -1 ? 5 : dirindex;
            this.setDir(dirindex);

            SkeletonAnimation[] sprs = this.GetComponentsInChildren<SkeletonAnimation>();
            if (sprs.Length > 0)
            {
                if (isChangeState)
                {
                    isChangeState = false;
                    foreach (var item in sprs)
                    {
                        item.Initialize(true);
                        this.Setstate(dir == Vector3.zero ? "Idle" : "Run", _dirname);
                        item.AnimationState.SetAnimation(0, dirName + "_" + stateName, true);
                    }
                }
            }
        }

        void Setstate(string name, string dir)
        {
            if (stateName != name)
            {
                stateName = name;
                isChangeState = true;
            }
            if (dirName != dir)
            {
                dirName = dir;
                isChangeState = true;
            }
        }

        public void setDir(Vector3 _dir)
        {
            if (this.isspAnim) return;
            this.transform.localEulerAngles = _dir;
        }

        public void invDir(bool isf = true)
        {
            if (!this.isspAnim) return;
            Vector3 s = this.transform.localScale;
            s.x = isf ? 1 : -1;
            this.transform.localScale = s;
        }


        public void setDir(int index)
        {
            // Debug.Log(this.gameObject.name);
            dirindex = index;
            switch (index)
            {
                case 0:
                    this.setDir(new Vector3(0, 0, 0));
                    this.invDir();
                    break;
                case 1:
                    this.setDir(new Vector3(0, 45, 0));
                    this.invDir();
                    break;
                case 2:
                    this.setDir(new Vector3(0, 90, 0));
                    this.invDir();
                    break;
                case 3:
                    this.setDir(new Vector3(0, 135, 0));
                    this.invDir();
                    break;
                case 4:
                    this.setDir(new Vector3(0, 180, 0));
                    this.invDir();
                    break;
                case 5:
                    this.setDir(new Vector3(0, 225, 0));
                    this.invDir(false);
                    break;
                case 6:
                    this.setDir(new Vector3(0, 270, 0));
                    this.invDir(false);
                    break;
                case 7:
                    this.setDir(new Vector3(0, 315, 0));
                    this.invDir(false);
                    break;
                default:
                    this.setDir(new Vector3(0, 90, 0));
                    this.invDir();
                    break;
            }
        }
    }
}