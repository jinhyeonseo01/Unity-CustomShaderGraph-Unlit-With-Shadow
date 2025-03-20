using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ConfigPath
{
    public const string rootPath = "Assets/CustomShaderGraph/";
    
    public const string customUnlitForwardPassPath = rootPath + "Custom Tamplate/Editor/ShaderGraph/Includes/UnlitPass.hlsl";
    public const string customUnlitGBufferPassPath = rootPath + "Custom Tamplate/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl";

    public static readonly string RootPath = GetRootPath();

    public static string GetRootPath([CallerFilePath] string filePath = "")
    {
        // ���� ����� �����ڸ� ���� (������ ���)
        filePath = filePath.Replace("\\", "/");
        // ������ ��ġ�� ���丮�� ������
        string directory = System.IO.Path.GetDirectoryName(filePath);
        // directory���� Replace ����
        directory = directory.Replace("\\", "/");

        // Assets ������ �ִ� ���
        int assetsIndex = directory.IndexOf("Assets/");
        if (assetsIndex >= 0)
        {
            // "Assets" ������ ��θ� ��ȯ (��: Assets/CustomShaderGraph)
            return directory.Substring(assetsIndex) + "/";
        }

        // Packages ������ �ִ� ���
        int packagesIndex = directory.IndexOf("Packages/");
        if (packagesIndex >= 0)
        {
            // "Packages" ������ ��θ� ��ȯ (��: Packages/Unity Custom Shader Graph)
            return directory.Substring(packagesIndex) + "/";
        }

        return filePath;
    }
}
