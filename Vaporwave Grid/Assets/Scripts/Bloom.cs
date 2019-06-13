using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TUTORIAL TAKEN FROM: https://catlikecoding.com/unity/tutorials/advanced-rendering/bloom/

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Bloom : MonoBehaviour
{
    public Shader bloomShader;

    Material bloomMat;

    [Range(1, 6)]
    public int iterations = 1;

    [Range(0, 10)]
    public float intensity = 1;

    [Range(1, 10)]
    public float threshold = 1;

    [Range(0, 1)]
    public float softThreshold = 0.5f;

    RenderTexture[] textures = new RenderTexture[6];

    const int BoxDownPrefilterPass = 0;
    const int BoxDownPass = 1;
    const int BoxUpPass = 2;
    const int ApplyBloomPass = 3;
    const int DebugBloomPass = 4;

    public bool debug;

    void OnRenderImage (RenderTexture source, RenderTexture dest)
    {
        if(bloomMat == null)
        {
            bloomMat = new Material(bloomShader);
            bloomMat.hideFlags = HideFlags.HideAndDontSave;
        }

        float knee = threshold * softThreshold;
        Vector4 filter;
        filter.x = threshold;
        filter.y = filter.x - knee;
        filter.z = 2f * knee;
        filter.w = 0.25f / (knee + 0.00001f);
        bloomMat.SetVector("_Filter", filter);
        bloomMat.SetFloat("_Intensity", Mathf.GammaToLinearSpace(intensity));

        int width = source.width / 2;
        int height = source.height / 2;
        RenderTextureFormat format = source.format;

        RenderTexture currentDest = textures[0] = RenderTexture.GetTemporary(
            width, height, 0, format);

        Graphics.Blit(source, currentDest, bloomMat, BoxDownPrefilterPass);
        RenderTexture currentSource = currentDest;

        int i = 1;
        for (; i < iterations; i++)
        {
            width /= 2;
            height /= 2;
            if(height < 2)
            {
                break;
            }

            currentDest = textures[i] = RenderTexture.GetTemporary(
                width, height, 0, format);
            Graphics.Blit(currentSource, currentDest, bloomMat, BoxDownPass);
            //RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }

        for(i -= 2; i >= 0; i--)
        {
            currentDest = textures[i];
            textures[i] = null;
            Graphics.Blit(currentSource, currentDest, bloomMat, BoxUpPass);
            RenderTexture.ReleaseTemporary(currentSource);
            currentSource = currentDest;
        }

        //Graphics.Blit(currentSource, dest, bloomMat, BoxUpPass);
        if (debug)
        {
            Graphics.Blit(currentSource, dest, bloomMat, DebugBloomPass);
        }
        else
        {
            bloomMat.SetTexture("_SourceTex", source);
            Graphics.Blit(currentSource, dest, bloomMat, ApplyBloomPass);
        }

        RenderTexture.ReleaseTemporary(currentDest);
    }
}
