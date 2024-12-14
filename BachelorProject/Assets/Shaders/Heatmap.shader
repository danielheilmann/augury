Shader "Unlit/Heatmap"
{
    // This only works for quads though...
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            float4 colors[6]; //colors for point ranges
            float pointRanges[6]; //ranges of values used to determine color values

            float _Hits[32 * 3]; //passed-in array of pointRanges; 3 floats per point (x, y, intensity)
            int _HitCount = 0;

            void initialize()
            {
                colors[0] = float4(0, 0, 0, 1);
                colors[1] = float4(0, 1, 0.5, 1);
                colors[2] = float4(0, 1, 0, 1);
                colors[3] = float4(1, 1, 0, 1);
                colors[4] = float4(1, 0, 0, 1);

                pointRanges[0] = 0.0;
                pointRanges[1] = 0.25;
                pointRanges[2] = 0.5;
                pointRanges[3] = 0.75;
                pointRanges[4] = 1.0;

                // Hardcoded testing hit
                // _HitCount = 2;
                //
                // _Hits[0] = 0;
                // _Hits[1] = 0;
                // _Hits[2] = 10;
                //
                // _Hits[3] = 1;
                // _Hits[4] = 1;
                // _Hits[5] = 3;                
            }

            float3 getHeatForPixel(float weight)
            {
                if (weight <= pointRanges[0])
                return colors[0];
                if (weight >= pointRanges[4])
                return colors[4];

                for (int i = 1; i < 5; i++)
                {
                    if (weight < pointRanges[i]) //if weight is between this point and the point before its range
                    {
                        float dist_from_lower_point = weight - pointRanges[i - 1];
                        float size_of_point_range = pointRanges[i] - pointRanges[i - 1];

                        float ratio_over_lower_point = dist_from_lower_point / size_of_point_range;

                        //now with ratio or percentage (0-1) into the point range, multiply color ranges to get color

                        float3 color_range = colors[i] - colors[i - 1];

                        float3 color_contribution = color_range * ratio_over_lower_point;

                        float3 new_color = colors[i - 1] + color_contribution;
                        return new_color;
                    }
                }
                return colors[0];
            }

            float distsq(float2 a, float2 b)
            {
                float area_of_effect_size = 1.0f;
                float d = pow(max(0.0, 1.0 - distance(a, b) / area_of_effect_size), 2.0);

                return d;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                initialize();

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 uv = i.uv;
                uv = uv * 4.0 - float2(2, 2); //Change uv coordinate range from "0 to 1" to "-2 to 2", so that (0,0) is the center of the plane.

                float totalWeight = 0;
                for (int h = 0; h < _HitCount; h++)
                {
                    float2 work_pt = float2(_Hits[h * 3], _Hits[h * 3 + 1]);
                    float pt_intensity = _Hits[h * 3 + 2];

                    totalWeight += 0.5 * distsq(uv, work_pt) * pt_intensity;
                }

                float3 heat = getHeatForPixel(totalWeight);

                return col + float4(heat, 0.2);
            }
            ENDCG
        }
    }
}