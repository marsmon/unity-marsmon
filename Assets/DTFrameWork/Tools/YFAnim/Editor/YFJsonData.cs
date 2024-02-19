#if UNITY_2020_1_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
using Newtonsoft.Json.Linq;
using System.Linq;
using System.IO;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using System.Reflection;

namespace YF.Art
{
    public class YFJsonData
    {
        [MenuItem("Custom/aaa")]
        static void aaaa()
        {  
            // string[] collection = AssetDatabase.FindAssets("YFAnimController t:Script"); 
            // foreach (var item in collection)
            // {
            //     Debug.Log(AssetDatabase.GUIDToAssetPath(item).Replace("_fun",""));
            // }

        }
        [MenuItem("Custom/Update YF Anim Json Sprite")]
        static void UpdateSettings()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is Texture2D)
                {
                    CreatYFAnimJson(obj);
                }
                AssetDatabase.Refresh();
            }
        }
        [MenuItem("Custom/Clear History Slices")]
        static void ClearHistorySlices()
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is Texture2D)
                {
                    RemoveData(obj);
                }
                AssetDatabase.Refresh();
            }
        }

        static void RemoveData(UnityEngine.Object obj)
        {
            Texture2D img = (Texture2D)obj;
            ISpriteEditorDataProvider dataProvider = GetSpriteEditorDataProvider(img);

            RemoveSpriteAll(dataProvider);
            dataProvider.Apply();
            var assetImporter = dataProvider.targetObject as AssetImporter;
            assetImporter.SaveAndReimport();
            EditorUtility.SetDirty(obj);

        }

        static void CreatYFAnimJson(UnityEngine.Object obj)
        {
            string p = AssetDatabase.GetAssetPath(obj);
            string Jsonpath = p.Replace(".png", ".json");

            if (!File.Exists(Jsonpath))
            {
                Debug.LogError("未找到同名的json文件!");
                return;
            }

            Texture2D img = (Texture2D)obj;
            SpriteRect[] rects = getSheetInfo(img, Jsonpath);
            // Debug.Log(rects.Length);
            var factory = new SpriteDataProviderFactories();
            factory.Init();
            ISpriteEditorDataProvider dataProvider = GetSpriteEditorDataProvider(img);

            var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            var oldIds = spriteNameFileIdDataProvider.GetNameFileIdPairs();


            SpriteNameFileIdPair[] ids = generateSpriteIds(oldIds, rects);

            dataProvider.SetSpriteRects(rects);
            spriteNameFileIdDataProvider.SetNameFileIdPairs(ids);
            dataProvider.Apply();
            var assetImporter = dataProvider.targetObject as AssetImporter;
            assetImporter.SaveAndReimport();
            EditorUtility.SetDirty(obj);

#if UNITY_2023_1_OR_NEWER
                    // workaround for bug IN-59357
                    if (!oldIds.Any())
                    {
                        Dbg.Log("delayed reimport: " + importer.assetPath);
                        texturesToReimport.Add(importer.assetPath);
                        if (texturesToReimport.Count() == 1)
                        {
                            EditorApplication.delayCall += reimport;
                        }
                    }
#endif
        }


        private static SpriteNameFileIdPair[] generateSpriteIds(IEnumerable<SpriteNameFileIdPair> oldIds,
                                                                SpriteRect[] sprites)
        {
            SpriteNameFileIdPair[] newIds = new SpriteNameFileIdPair[sprites.Length];

            for (int i = 0; i < sprites.Length; i++)
            {
                sprites[i].spriteID = idForName(oldIds, sprites[i].name);
                newIds[i] = new SpriteNameFileIdPair(sprites[i].name, sprites[i].spriteID);
            }

            return newIds;
        }
        private static GUID idForName(IEnumerable<SpriteNameFileIdPair> oldIds, string name)
        {
            foreach (SpriteNameFileIdPair old in oldIds)
            {
                if (old.name == name)
                {
                    return old.GetFileGUID();
                }
            }
            return GUID.Generate();
        }
        private static ISpriteEditorDataProvider GetSpriteEditorDataProvider(Texture2D importer)
        {
            var dataProviderFactories = new SpriteDataProviderFactories();
            dataProviderFactories.Init();
            var dataProvider = dataProviderFactories.GetSpriteEditorDataProviderFromObject(importer);
            dataProvider.InitSpriteEditorDataProvider();
            return dataProvider;
        }

        static SpriteRect[] getSheetInfo(Texture2D importer, string Jsonpath)//,string path
        {
            List<SpriteRect> metadata = new List<SpriteRect>();
            Vector2Int size = Vector2Int.zero;
            size.x = importer.width;
            size.y = importer.height;
            metadata.Clear();
            TextAsset go = (TextAsset)AssetDatabase.LoadAssetAtPath<TextAsset>(Jsonpath);
            JObject data = JObject.Parse(go.text);
            JObject mcdata = JObject.Parse(data["mc"].ToString());
            JObject resdata = JObject.Parse(data["res"].ToString());
            foreach (var item in mcdata)
            {
                // frames
                JArray farme = JArray.Parse(JObject.Parse(item.Value.ToString())["frames"].ToString());
                foreach (var item1 in farme)
                {
                    JObject tt = JObject.Parse(item1.ToString());
                    JObject tmp = JObject.Parse(resdata[tt["res"].ToString()].ToString());
                    int x = (int)tmp["x"];
                    int w = (int)tmp["w"];
                    int h = (int)tmp["h"];
                    int y = size.y - h - (int)tmp["y"];

                    float ofx0 = (float)tt["x"] * -1.0f / (float)w;
                    float ofy0 = (h - (float)tt["y"] * -1.0f) / (float)h;

                    var newSprite = new SpriteRect()
                    {
                        alignment = SpriteAlignment.Custom,
                        pivot = new Vector2(ofx0, ofy0),
                        name = tt["res"].ToString(),
                        // spriteID = GUID.Generate(),
                        rect = new Rect(x, y, w, h)
                    };
                    metadata.Add(newSprite);
                }
            }
            return metadata.ToArray();
        }

        static void RemoveSpriteAll(ISpriteEditorDataProvider dataProvider)
        {
            var spriteRects = dataProvider.GetSpriteRects().ToList();
            spriteRects.Clear();
            dataProvider.SetSpriteRects(spriteRects.ToArray());
#if UNITY_2021_2_OR_NEWER
            // Note: This section is only for Unity 2021.2 and newer
            // Get all the existing SpriteName & FileId pairs and look for the Sprite with the selected name
            var spriteNameFileIdDataProvider = dataProvider.GetDataProvider<ISpriteNameFileIdDataProvider>();
            var nameFileIdPairs = spriteNameFileIdDataProvider.GetNameFileIdPairs().ToList();
            nameFileIdPairs.Clear();
            spriteNameFileIdDataProvider.SetNameFileIdPairs(nameFileIdPairs);
            // End of Unity 2021.2 and newer section
#endif
            // Apply the changes
            dataProvider.Apply();
        }
    }
}
#endif
