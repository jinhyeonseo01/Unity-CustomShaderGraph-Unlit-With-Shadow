using System.Reflection;
using UnityEngine;
using UnityEditor.ShaderGraph;
using static Codice.CM.Common.Purge.PurgeReport;



namespace UnityEditor.ShaderGraph
{
    [Title("Input", "Lighting", "Main Light Full")]// "Lighting", 
    class MainLightFullNode : CodeFunctionNode
    {
        public MainLightFullNode()
        {
            name = "Main Light Full";
            synonyms = new string[] { "main_light" };
        }

        public override bool hasPreview { get { return false; } }

        protected override MethodInfo GetFunctionToConvert()
        {
            return GetType().GetMethod("MainLightFull", BindingFlags.Static | BindingFlags.NonPublic);
        }

        static string MainLightFull(
            [Slot(0, Binding.WorldSpacePosition)] Vector3 PositionWS,
            [Slot(1, Binding.None)] Vector1 occlusion,
            [Slot(2, Binding.None)] out Vector3 Direction,
            [Slot(3, Binding.None)] out Vector4 Color,
            [Slot(4, Binding.None)] out Vector1 DistanceAttenuation,
            [Slot(5, Binding.None)] out Vector1 ShadowAttenuation)
        {
            Direction = default;
            Color = default;
            DistanceAttenuation = default;
            ShadowAttenuation = default;

            return
@"
{
    // occlusion ���� ��� �Է����� ���� ���� ������ ���⼭�� ���� 1.0(=���� ����)���� ����

    #ifdef SHADERGRAPH_PREVIEW  // SHADERGRAPH_PREVIEW (������ ���)
        // ������� ������ ���� ���
        Color = $precision4(1, 1, 1, 1);
        Direction = $precision3(-0.707, 0.707, 0);
        ShadowAttenuation = 1;
        DistanceAttenuation = 1;
    #else 

    #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"" // Added first
    #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl""
    #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl""
    #include ""Packages/com.unity.render-pipelines.universal/ShaderLibrary/AmbientOcclusion.hlsl""

    $precision4 positionCS = TransformWorldToHClip(PositionWS);

    #if defined(_MAIN_LIGHT_SHADOWS_SCREEN) && !defined(_SURFACE_TYPE_TRANSPARENT)
        $precision4 shadowCoord = ComputeScreenPos(positionCS);
    #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
        $precision4 shadowCoord = TransformWorldToShadowCoord(PositionWS);
    #else
        $precision4 shadowCoord = $precision4(0, 0, 0, 0);
    #endif
   
    $precision2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(positionCS);
    #if !defined (LIGHTMAP_ON)
        $precision4 shadowMask = unity_ProbesOcclusion; // legacy probes�� �׸��� ����ũ(����) ���ø�
    #else
        $precision4 shadowMask = $precision4(1, 1, 1, 1); // fallback, ���� ������� ����
    #endif

    Light light = GetMainLight(shadowCoord, PositionWS, shadowMask);

    AmbientOcclusionFactor aoFactor = CreateAmbientOcclusionFactor(normalizedScreenSpaceUV, occlusion);
    #if defined(_SCREEN_SPACE_OCCLUSION) && !defined(_SURFACE_TYPE_TRANSPARENT)
        if (IsLightingFeatureEnabled(DEBUGLIGHTINGFEATUREFLAGS_AMBIENT_OCCLUSION))
        {
            light.color *= aoFactor.directAmbientOcclusion;
        }
    #endif

    Color = $precision4(light.color.xyz, 1);
    Direction = light.direction;
    ShadowAttenuation = light.shadowAttenuation;
    DistanceAttenuation = light.distanceAttenuation;

    #endif
}
";

        }
    }


}