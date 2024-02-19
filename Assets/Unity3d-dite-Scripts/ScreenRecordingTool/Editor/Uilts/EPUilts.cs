using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace YF.Art.EdWindowUilts
{ 
    /// <summary>
    /// 自定义的一些EditorPrefs的操作
    /// </summary>
    public sealed class EPUilts
    {   
        /// <summary>
        /// 强制转换类型 K 转 T
        /// </summary>
        /// <param name="_attr"></param>
        /// <typeparam name="T">目标类型</typeparam>
        /// <typeparam name="K">源类型</typeparam>
        /// <returns>T 类型的数据 如果失败则返回T的默认值(default(T))</returns>
        public static T UKObj2TObj<T,K>(K _attr)
        {
            try
            {
                return (T)Convert.ChangeType(_attr,typeof(T));
            }
            catch
            {
                Debug.LogError(typeof(K).ToString() + "<=>" + typeof(T).ToString() +"转换失败！");
                return default(T);
            }
        }
        // ===============================================================================================
        /// <summary>
        /// GetEditorPrefs 泛类型合并get
        /// </summary>
        /// <param name="_key"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>T value</returns>
        public static T GetEP<T>(string _key) 
        {
            if (EditorPrefs.HasKey(_key))
            {
                switch(typeof(T).ToString()){
                    case "System.Int32":
                        return (T)Convert.ChangeType(EditorPrefs.GetInt(_key),typeof(T));
                    case "System.String":
                        return (T)Convert.ChangeType(EditorPrefs.GetString(_key),typeof(T));                
                    case "System.Single":
                        return (T)Convert.ChangeType(EditorPrefs.GetFloat(_key),typeof(T));                   
                    case "System.Boolean":
                        return (T)Convert.ChangeType(EditorPrefs.GetBool(_key),typeof(T));
                    default:
                        return default(T);
                }
            }
            return default(T);
        }


        // /// <summary>
        // /// default(T) int 0;string "";bool False; float 0;
        // /// </summary>
        // /// <param name="_key"></param>
        // /// <param name="_porp"></param>
        // /// <typeparam name="T"></typeparam>
        // /// <returns></returns>
        // public static T SetVorDefEP<T>(string _key,T _porp) 
        // {
            
        //     if(!default(T).Equals(EPUilts.GetEP<T>(_key))){
        //         EPUilts.SetEP(_key,_porp);
        //     }
        //     else{
        //         Debug.LogWarning( _key + " 未发生改变");
        //     }
        //     return EPUilts.GetEP<T>(_key);
        // }
        /// <summary>
        /// default(T) int 0;string "";bool False; float 0;
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_defporp"></param>
        /// <param name="_inporp"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T SetVorDefEP<T>(string _key,T _defporp,T _inporp) 
        {
            if (!EditorPrefs.HasKey(_key)){
                if (_inporp != null){
                    EPUilts.SetEP(_key,_inporp);
                }
                else{
                    EPUilts.SetEP(_key,_defporp);
                }
            }
            else if (_inporp == null || _inporp.Equals(default(T))){
                if(EPUilts.GetEP<T>(_key).Equals(default(T))){
                    EPUilts.SetEP(_key,_defporp);
                }                
            }
            else if(!_inporp.Equals(EPUilts.GetEP<T>(_key))){
                EPUilts.SetEP(_key,_inporp);
            }
            else{
                // Debug.LogWarning( _key + " 未发生改变");
            }
            return EPUilts.GetEP<T>(_key);
        }        

        /// <summary>
        /// Set EditorPrefs 中 key的值
        /// </summary>
        /// <param name="_key"></param>
        /// <param name="_porp"></param>
        public static void SetEP<T>(string _key,T _porp) 
        {
            switch(typeof(T).ToString()){
                case "System.Int32":                
                    EditorPrefs.SetInt(_key, UKObj2TObj<int,T>(_porp));
                    break;
                case "System.String":
                    EditorPrefs.SetString(_key,UKObj2TObj<string,T>(_porp));
                     break;
                case "System.Single":
                    EditorPrefs.SetFloat(_key,UKObj2TObj<float,T>(_porp));
                    break;                 
                case "System.Boolean":
                    EditorPrefs.SetBool(_key,UKObj2TObj<bool,T>(_porp));
                    break;
                default:
                    break;                
            }
        }   
        public static void DelEP(string _key){
            EditorPrefs.DeleteKey(_key);
        }
    }    
}
