using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace YF.Art.EdWindowUilts
{ 
    // ==============================================================================================
    /// <summary>
    /// Editor 窗口 通过EditorPrefs记录 InstanceID
    /// 此类通过记录的id 而产生的一些操作
    /// </summary>
    public class EdwinUilts
    {   
       
        /// <summary>
        /// EditorPrefs 获取ID 然后 InstanceIDToObject 
        /// </summary>
        /// <param name="_key"></param>
        /// <returns> UnityEngine.Object </returns>
        public static UnityEngine.Object EPInstanceIDToObject(string _key)
        {
            //EditorUtility.InstanceIDToObject
            if(!EditorPrefs.HasKey(_key)){
                return null;
            }
            // (T)Convert.ChangeType(EditorPrefs.GetInt(_key),typeof(T));
            int instanceid = EPUilts.GetEP<int>(_key);
            if(instanceid == 0){
                return null;
            }            
            return EditorUtility.InstanceIDToObject(instanceid) as UnityEngine.Object ;
        }
        /// <summary>
        /// 通过EditorPrefs获取OBJ，判断与当前的是否是同对象
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool GetObjectEquals<T>(string _key,T _obj) where T : UnityEngine.Object
        {
            return _obj.GetInstanceID() == EPUilts.GetEP<int>(_key);
        }
        /// <summary>
        /// _isreplace true 通过EditorPrefs中的InstanceID 获取 对象。 
        /// _isreplace false 修改 EditorPrefs。int中的InstanceID。
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_obj"></param>
        /// <param name="_isreplace">默认是true</param>
        /// <typeparam name="T"></typeparam>
        public static void SetObject<T>(string _key,ref T _obj,bool _isreplace = true ) where T : UnityEngine.Object
        {
            if (_isreplace){
                if(!GetObjectEquals(_key,_obj)){
                    _obj = EPUilts.UKObj2TObj<T,UnityEngine.Object>(EPInstanceIDToObject(_key));
                }
            }
            else
            {   
                if(!GetObjectEquals(_key,_obj)){                    
                    EPUilts.SetEP(_key,_obj.GetInstanceID());
                }
            }
        }
    }  
}
