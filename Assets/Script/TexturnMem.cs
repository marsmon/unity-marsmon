using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturnMem : MonoBehaviour
{
    private Texture2D texture2D1;
    private Texture2D texture2D2;
    private Texture2D texture2D3;
    private Texture2D texture2D4;
    private Texture2D texture2D5;
    
    
    public Material material;
    
    // Start is called before the first frame update
    void Start()
    {
        texture2D1 = Resources.Load<Texture2D>("TextureCombine/1");
        texture2D2 = Resources.Load<Texture2D>("TextureCombine/2");
        texture2D3 = Resources.Load<Texture2D>("TextureCombine/3");

        Texture2D combineTexture2D = Combine(texture2D1, texture2D2, texture2D3);
        
        texture2D1.Apply(false, true);
        texture2D2.Apply(false, true);
        texture2D3.Apply(false, true);

        material.mainTexture = combineTexture2D;
    }
    
    static  Texture2D Combine(Texture2D tex, Texture2D tex1, Texture2D tex2)
    {
        int length = 1024;
        
        var blcokBytes = 0;
        byte[] data = null;
        blcokBytes = 16; 
        data = new byte[length * length];
        //填充左上角 512*512 
        CombineBlocks(tex.GetRawTextureData(), data, 0, 512, 512, 512, 4, blcokBytes, length);
        //填充右上角 512*512
        CombineBlocks(tex1.GetRawTextureData(), data, 512, 512, 512, 512, 4, blcokBytes, length);
        //填充下方 1024*512 
        CombineBlocks(tex2.GetRawTextureData(), data, 0, 0, 1024, 512,4, blcokBytes, length);
 
        var combinedTex = new Texture2D(length, length, tex.format, false);
        Debug.Log($"combinedTex.isReadable = {combinedTex.isReadable}");
        combinedTex.LoadRawTextureData(data);
        combinedTex.Apply(false,true);
        Debug.Log($"combinedTex.isReadable = {combinedTex.isReadable}");
 
        return combinedTex;
    }
    
    
 
    static void CombineBlocks(byte[] src, byte[] dst, int dstx, int dsty, int width, int height, int block, int bytes, int length)
    {
        int dstBlockX = dstx / block; 
        int dstBlockY = dsty / block;
        int dstColBlockCount = length / block;
        int srcColBlockCount = width / block;
        int srcRowBlockCount = height / block;
        for (int i = 0; i < srcRowBlockCount; i++)
        {
            int dstByteIndex = (dstBlockX + (dstBlockY + i) * (dstColBlockCount)) * bytes;
            int srcByteIndex = i * srcColBlockCount * bytes;
            Buffer.BlockCopy(src, srcByteIndex, dst, dstByteIndex, srcColBlockCount * bytes);
        }
    }
}
