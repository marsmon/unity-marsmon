using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YF.Art.FXRenderStudio
{
    [System.Serializable]
    public class ScreenRecordingToolNAME
    {
    public static string NAME_windowTitle{
            get
            {
                switch(Application.systemLanguage)
                {
                    case SystemLanguage.Chinese:
                        return "U3d序列图导出";
                    default:
                        return "U3d to Sequence Image Exporter";
                }
            }
        }

        public string NAME_currectFrame;
        public string NAME_selectedCamera;
        // public string NAME_setFormat;
        // public string NAME_isEnableAlpha;
        public string NAME_setFormatDescribution;
        public string NAME_setResolution;
        // public string NAME_setDefaultSize;
        public string NAME_setCurrectSize;
        public string NAME_setFileName;
        public string NAME_setSavePath;
        public string NAME_takeScreenShot;
        public string NAME_takeScreenShotDescribution;
        public string NAME_setEnableSequence;
        public string NAME_setFrame;
        public string NAME_setFrameRange;
        public string NAME_startExport;
        public string NAME_startExportDescribution;
        public string NAME_openFolder;
        public string NAME_credit;

        public void init()
        {
            NAME_currectFrame = "当前帧";
            NAME_selectedCamera = "选择相机";
            // NAME_setFormat = "设置格式";
            // NAME_isEnableAlpha = "是否启用透明通道";
            NAME_setFormatDescribution = "特效材质注意2点:\n 1.只有Mobile/Particle中的Shader才能显示出来.\n 2.贴图导出如果有黑底，请将图片的Alpha is Transparent设置正确并勾选。";
            NAME_setResolution = "设置分辨率";
            // NAME_setDefaultSize = "默认尺寸";
            NAME_setCurrectSize = "当前屏幕尺寸";
            NAME_setFileName = "文件名称";
            NAME_setSavePath = "保存位置";
            NAME_takeScreenShot = "截取当前面画";
            NAME_takeScreenShotDescribution = "对当前画面进行单张截图,不在运行模式下也可以截图。";
            NAME_setEnableSequence = "是否启用导出序列图";
            NAME_setFrame = "设置帧率";
            NAME_setFrameRange = "设置起始帧";
            NAME_startExport = "开始导出序列图";
            NAME_startExportDescribution = "该模式必须进入播放模式。";
            NAME_openFolder = "打开导出文件夹";
            // switch (Application.systemLanguage)
            // {
            //     case SystemLanguage.Chinese:
                    
            //         // NAME_credit = "update:2020/02/13 unity3d 2019.1.11f1";
            //         // NAME_credit = "如有任何疑问请联系99U:225367(李红伟) update:2017/10/11";
            //         break;
            //     default:
            //         NAME_currectFrame = "Currect Frame";
            //         NAME_selectedCamera = "Select Camera";
            //         // NAME_setFormat = "Set Format";
            //         // NAME_isEnableAlpha = "Enabled Alpha";
            //         NAME_setFormatDescribution = "Special material attention 2 points: \n 1., only the Shader in Mobile/Particle can show,.\n 2. map export, if there is black bottom, please put the pictures of Alpha, is, Transparent settings correctly, and tick.";
            //         NAME_setResolution = "Resolution";
            //         // NAME_setDefaultSize = "Default Size";
            //         NAME_setCurrectSize = "Set To Screen Size ";
            //         NAME_setFileName = "File Name";
            //         NAME_setSavePath = "Save Path";
            //         NAME_takeScreenShot = "Take Screenshot";
            //         NAME_takeScreenShotDescribution = "On the current screen for a single shot, not in the play mode can also be screenshots.";
            //         NAME_setEnableSequence = "Enabled Sequence Export";
            //         NAME_setFrame = "Set Frame";
            //         NAME_setFrameRange = "Set Range";
            //         NAME_startExport = "Start Export Sequence";
            //         NAME_startExportDescribution = "If you want to export sequence image, please check the \"Enabled Sequence Export\" button, and then click the \"Start Export Sequence\" button or direct click run button.";
            //         NAME_openFolder = "Open Folder";
            //         break;
            // }
        }
    }
}