Shader "Unlit/VertexAnimation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AnimVertexTex ("AnimVertexTex", 2D) = "white" {}
        _AnimationSpeed ("AnimationSpeed", Float) = 1

        _PosMin ("PosMin", Vector) = (0, 0, 0)
        _PosMax ("PosMax", Vector) = (0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertexPos : POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                uint vertexId : SV_VertexID;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertexPos : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _AnimVertexTex;
            float4 _MainTex_ST;
            float _AnimationSpeed;
            float3  _PosMin;
            float3 _PosMax;


            v2f vert (appdata v)
            {
                
                float vertexID = v.uv2.x;
                float textureTime = _Time.y * _AnimationSpeed;
                float4 uv = float4(vertexID, textureTime, 0, 0);
                float4 currentAnimationTexturePos = tex2Dlod(_AnimVertexTex, uv);
 
                float3 finalPosition = lerp(_PosMin, _PosMax, currentAnimationTexturePos.xyz);

  
                v2f o;
                // Use position values as a standard local space coordinates
                o.vertexPos = UnityObjectToClipPos(finalPosition);
                o.uv = TRANSFORM_TEX(v.uv2, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertexPos);

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
