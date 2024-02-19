using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace YF {
    public enum DIRECTION {
        UP,
        RIGHT_UP,
        RIGHT,
        RIGHT_DOWN,
        DOWN,
        LEFT_DOWN,
        LEFT,
        LEFT_UP,
        MIN = 0,
        MAX = 7,
    }

    public class ACTION {
        public int id;
        public string name;
        public bool loop;
        public string linkName;
        public DIRECTION direction;
        public UnityEngine.Vector3 position;

        public ACTION(int id, string name, DIRECTION direction, UnityEngine.Vector3 position, bool loop, string linkName = "") {
            this.id = id;
            this.name = CONST.DIRECTION_NAME[direction] + "_" + name;
            this.direction = direction;
            this.position = position;
            this.loop = loop;

            if (linkName.Length > 0) {
                this.linkName = CONST.DIRECTION_NAME[direction] + "_" + linkName;
            } else {
                this.linkName = "";
            }
            
        }
    }

    public class CONST {
        static public Dictionary<DIRECTION, string> DIRECTION_NAME = new Dictionary<DIRECTION, string>() {
            { DIRECTION.UP, "Up" },
            { DIRECTION.RIGHT, "Right" },
            { DIRECTION.DOWN, "Down" },
            { DIRECTION.LEFT, "Left" },
        };

        // 所有朝向的动画定义
        static public ArrayList ACTIONS = new ArrayList();
        // 动画机中动画位置 
        static private float y = 0f;

        static public void LoadActions() {
            ACTIONS.Clear();
            y = 0f;

            LoadFourDirectionActions(1, "Idle", true);
            LoadFourDirectionActions(9, "IdleRide", true);
            LoadFourDirectionActions(10, "Run", true);
            LoadFourDirectionActions(11, "Walk", true);
            LoadFourDirectionActions(19, "RunRide", true);

            // 第1套技能
            LoadFourDirectionActions(20, "Skill1_Start", false, "Skill1_Ready");
            LoadFourDirectionActions(21, "Skill1_Ready", true);
            LoadFourDirectionActions(22, "Skill1_Chant", false, "Skill1_Loop");
            LoadFourDirectionActions(23, "Skill1_Loop", true);
            // LoadFourDirectionActions(24, "Skill1_End", false);

            // 第2套技能
            LoadFourDirectionActions(30, "Skill2_Start", false, "Skill2_Ready");
            LoadFourDirectionActions(31, "Skill2_Ready", true);
            LoadFourDirectionActions(32, "Skill2_Chant", false, "Skill2_Loop");
            LoadFourDirectionActions(33, "Skill2_Loop", true);
            // LoadFourDirectionActions(34, "Skill2_End", false);

            // 第3套技能
            LoadFourDirectionActions(40, "Skill3_Start", false, "Skill3_Ready");
            LoadFourDirectionActions(41, "Skill3_Ready", true);
            LoadFourDirectionActions(42, "Skill3_Chant", false, "Skill3_Loop");
            LoadFourDirectionActions(43, "Skill3_Loop", true);
            // LoadFourDirectionActions(44, "Skill3_End", false);

            // 第4套技能
            LoadFourDirectionActions(50, "Skill4_Start", false, "Skill4_Ready");
            LoadFourDirectionActions(51, "Skill4_Ready", true);
            LoadFourDirectionActions(52, "Skill4_Chant", false, "Skill4_Loop");
            LoadFourDirectionActions(53, "Skill4_Loop", true);
            // LoadFourDirectionActions(54, "Skill4_End", false);

            // 第5套技能
            LoadFourDirectionActions(60, "Skill5_Start", false, "Skill5_Ready");
            LoadFourDirectionActions(61, "Skill5_Ready", true);
            LoadFourDirectionActions(62, "Skill5_Chant", false, "Skill5_Loop");
            LoadFourDirectionActions(63, "Skill5_Loop", true);
            // LoadFourDirectionActions(64, "Skill5_End", false);

            LoadFourDirectionActions(71, "Dead", false);
            LoadFourDirectionActions(70, "Die", true, "Dead");

            // 单朝向
            ACTIONS.Add(new ACTION(91, "Sit", DIRECTION.DOWN, new UnityEngine.Vector3(300, (int)DIRECTION.DOWN * 500 + y), true));
        }

        // 加载上下左右4个朝向数据
        static private void LoadFourDirectionActions(int id, string name, bool loop, string linkAction = "") {
            for (int direction = (int)DIRECTION.MIN; direction <= (int)DIRECTION.MAX; direction++) {
                // 跳过斜方向
                if (direction % 2 > 0) {
                    continue;
                }

                if (name.StartsWith("Skill")) {
                    // 技能id 从20开始
                    int step = (id - 20) % 10;
                    UnityEngine.Vector3 pos = new UnityEngine.Vector3(300 + ((id - 20) / 10 + 1) * 250, direction * 500 + step * 100);
                    ACTIONS.Add(new ACTION(id, name, (DIRECTION)direction, pos, loop, linkAction));
                } else {
                    ACTIONS.Add(new ACTION(id, name, (DIRECTION)direction, new UnityEngine.Vector3(300, direction * 500 + y), loop, linkAction));
                }
            }

            if (!name.StartsWith("Skill")) {
                y += 100;
            }
        }
    }
}