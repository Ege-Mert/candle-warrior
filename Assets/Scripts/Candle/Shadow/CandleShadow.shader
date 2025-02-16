Shader "Custom/CandleShadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Shadow Color", Color) = (0,0,0,1)
        _FadeStart ("Fade Start", Range(0, 1)) = 0.5
        _FadeEnd ("Fade End", Range(0, 1)) = 1.0
        _Opacity ("Base Opacity", Range(0, 1)) = 0.8
        _FlickerSpeed ("Flicker Speed", Float) = 5.0
        _FlickerIntensity ("Flicker Intensity", Range(0, 1)) = 0.1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
            "IgnoreProjector"="True"
        }
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            sampler2D _MainTex;
            float4 _Color;
            float _FadeStart;
            float _FadeEnd;
            float _Opacity;
            float _FlickerSpeed;
            float _FlickerIntensity;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            float random(float2 st)
            {
                return frac(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // Calculate distance from center
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) * 2.0; // Multiply by 2 to make the fade more pronounced
                
                // Calculate fade based on distance
                float fade = 1 - smoothstep(_FadeStart, _FadeEnd, dist);
                
                // Add flicker effect
                float time = _Time.y * _FlickerSpeed;
                float flicker = random(float2(time, time)) * _FlickerIntensity;
                fade *= (1 - flicker);
                
                // Apply base opacity
                fade *= _Opacity;
                
                // Return final color with fade
                return fixed4(_Color.rgb, fade * _Color.a);
            }
            ENDCG
        }
    }
}