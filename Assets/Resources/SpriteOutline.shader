Shader "Custom/SpriteOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineSize ("Outline Size (pixels)", Float) = 2
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _OutlineColor;
            float _OutlineSize;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base pixel
                fixed4 main = tex2D(_MainTex, i.uv);
                if (main.a > 0.001)
                    return main * i.color;

                // Outline detection
                float2 size = _MainTex_TexelSize.xy * _OutlineSize;

                float alpha =
                    tex2D(_MainTex, i.uv + float2(size.x, 0)).a +
                    tex2D(_MainTex, i.uv + float2(-size.x, 0)).a +
                    tex2D(_MainTex, i.uv + float2(0, size.y)).a +
                    tex2D(_MainTex, i.uv + float2(0, -size.y)).a;

                if (alpha > 0)
                    return _OutlineColor;

                return 0;
            }
            ENDCG
        }
    }
}
