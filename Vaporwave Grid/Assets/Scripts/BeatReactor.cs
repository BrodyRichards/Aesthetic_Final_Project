using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatReactor : MonoBehaviour
{
    [Range(0f, 0.1f)]
    public float additive;

    [Range(0f, 0.1f)]
    public float dampValue;

    private Bloom bloom;


    // Start is called before the first frame update
    void Start()
    {
        bloom = GetComponent<Bloom>();
    }

    // Update is called once per frame
    void Update()
    {
        int numPartitions = 1;
        float[] aveMag = new float[numPartitions];
        float partitionIndx = 0;
        int numDisplayedBins = AudioPeer.numBins / 2; //NOTE: we only display half the spectral data because the max displayable frequency is Nyquist (at half the num of bins)

        for (int i = 0; i < numDisplayedBins; i++)
        {
            if (i < numDisplayedBins * (partitionIndx + 1) / numPartitions)
            {
                aveMag[(int)partitionIndx] += AudioPeer.spectrumData[i] / (AudioPeer.numBins / numPartitions);
            }
            else
            {
                partitionIndx++;
                i--;
            }
        }

        // scale and bound the average magnitude.
        for (int i = 0; i < numPartitions; i++)
        {
            aveMag[i] = 0.5f + aveMag[i] * 100;
            if (aveMag[i] > 100)
            {
                aveMag[i] = 100;
            }
        }

        float mag = aveMag[0];

        if (mag >= 1.15f)
        {
            Debug.Log("Increment");
            if(bloom.softThreshold < 0.5)
            {
                bloom.softThreshold += additive;
            }
        }else if (bloom.softThreshold >= 0.3f)
        {
            bloom.softThreshold -= dampValue;
        }
    }
}
