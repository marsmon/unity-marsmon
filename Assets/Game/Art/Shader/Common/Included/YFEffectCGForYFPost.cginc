
// YFEffectCGForYFPost  ..\Common\Included\YFEffectCGForYFPost.cginc
#ifndef YF_EFFECT_CG_YFPOST_DITE_INCLUDED
    #define YF_EFFECT_CG_YFPOST_DITE_INCLUDED
    #include "UnityCG.cginc"
    #include "YFProjectTeamSettings.cginc" 

    

    // [Header(Custom Bloom)] // CB=>Custom Bloom
    // [Toggle]_CBLocalBanBool("独立开关",float) = 1
    // _CBLocalThreshhold("CB Threshhold",range(0.0,1.0)) = 1
    // _CBLocalColor("CB Color",Color) = (1,1,1,1)
    // _CBLocalLight("CB 亮度",float) = 1
    #ifdef YF_POST_BLOOM 
        DTfloat _CBLocalThreshhold;
        DTfloat4 _CBLocalColor;
        DTfloat _CBLocalLight;
        fixed _CBGlobalBool;
        fixed _CBLocalBanBool;
    #endif
    
    inline DTfloat4 BloomOutfrag(float4 col){
        #ifndef YF_POST_BLOOM
            return col;
        #else 
            if(_CBLocalBanBool == 1){
                return col;
            }
            if(_CBGlobalBool == 1){
                DTfloat cc = dot(col.rgb, float3(0.2126729f,  0.7151522f, 0.0721750f)) ;
                fixed bcol = smoothstep(1.0 - max(0,_CBLocalThreshhold), 1.0 , cc ) * col.a ;
                fixed3 nc = col.rgb * bcol;
                col.rgb += nc * _CBLocalLight * _CBLocalColor.rgb; 
                return col;
            }
            else{
                return col;
            }
        #endif
    }
#endif // YF_EFFECT_CG_YFPOST_DITE_INCLUDED

