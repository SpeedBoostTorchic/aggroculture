using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour {

    [Header("Post Processing")]
    public PostProcessingProfile currentProfile;    //The profile currently being displayed
    public PostProcessingProfile baseProfile;       //An unaltered profile
    public PostProcessingProfile timeStop;
    public PostProcessingProfile[] weatherProfiles;

    public AudioClip[] jingles;

    public float normSpeed;

    [Header("Character Specific")]
    //Booleans determine which powers are active and which aren't
    public bool norm;           //Norm: Chromatic Abberation, Color Grading (Low Saturation), Motion Blur (High)
    public AudioClip[] normSounds;
    public Image characterIcon;
    public Sprite[] characterPortraits;


    //Updates all of the post processing effects
    public void updatePPEffects()
    {
        baseProfile = weatherProfiles[weather];

        //Current settings displayed here:
        PostProcessingProfile tempProf = currentProfile;

        //Norm's time slowing power...
        if (norm)
        {
            enablePPEffects(ref tempProf, ref timeStop);
            incrementPPSliders(ref tempProf, ref timeStop, normSpeed, 0.1f);
        }
        else
        {
            //Disables certain elements only if they are equal
            if (comparePP(ref tempProf, ref baseProfile))
                enablePPEffects(ref tempProf, ref baseProfile);
            
            incrementPPSliders(ref tempProf, ref baseProfile, normSpeed, 0.1f);
        }

        //The current profile is then changed here:
        currentProfile = tempProf;
    }

    //Sets relevant PP effects to enabled
    public void enablePPEffects(ref PostProcessingProfile tar, ref PostProcessingProfile orig)
    {
        tar.bloom.enabled = orig.bloom.enabled;
        tar.vignette.enabled = orig.vignette.enabled;
        tar.chromaticAberration.enabled = orig.chromaticAberration.enabled;
        tar.grain.enabled = orig.grain.enabled;
        tar.colorGrading.enabled = orig.colorGrading.enabled;
        tar.motionBlur.enabled = orig.motionBlur.enabled;
    }

    //Increments the relevant sliders
    public void incrementPPSliders(ref PostProcessingProfile tar, ref PostProcessingProfile orig, float speed, float thresh)
    {
        //Current settings displayed here:
        BloomModel.Settings curBloom = tar.bloom.settings;
        VignetteModel.Settings curVig = tar.vignette.settings;
        ChromaticAberrationModel.Settings curChrome = tar.chromaticAberration.settings;
        GrainModel.Settings curGrain = tar.grain.settings;
        ColorGradingModel.Settings curColor = tar.colorGrading.settings;
        MotionBlurModel.Settings curMotion = tar.motionBlur.settings;

        //If Bloom is Enabled...
        if (tar.bloom.enabled)
        {
            //Increments intesity...
            //--------------------------------------------------------------------------------------
            if (curBloom.bloom.intensity < orig.bloom.settings.bloom.intensity - thresh)
            {
                curBloom.bloom.intensity += speed * Time.unscaledDeltaTime;
            }
            else if (curBloom.bloom.intensity > orig.bloom.settings.bloom.intensity + thresh)
            {
                curBloom.bloom.intensity -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curBloom.bloom.intensity = orig.bloom.settings.bloom.intensity;
            }

            //Increments gamma threshold...
            //--------------------------------------------------------------------------------------
            if (curBloom.bloom.threshold < orig.bloom.settings.bloom.threshold - thresh)
            {
                curBloom.bloom.threshold += speed * Time.unscaledDeltaTime;
            }
            else if (curBloom.bloom.threshold > orig.bloom.settings.bloom.threshold + thresh)
            {
                curBloom.bloom.threshold -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curBloom.bloom.threshold = orig.bloom.settings.bloom.threshold;
            }

            //Increments soft knee...
            //--------------------------------------------------------------------------------------
            if (curBloom.bloom.softKnee < orig.bloom.settings.bloom.softKnee - thresh)
            {
                curBloom.bloom.softKnee += speed * Time.unscaledDeltaTime;
            }
            else if (curBloom.bloom.softKnee > orig.bloom.settings.bloom.softKnee + thresh)
            {
                curBloom.bloom.softKnee -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curBloom.bloom.softKnee = orig.bloom.settings.bloom.softKnee;
            }

            //Increments bloom radius...
            //--------------------------------------------------------------------------------------
            if (curBloom.bloom.radius < orig.bloom.settings.bloom.radius - thresh)
            {
                curBloom.bloom.radius += speed * Time.unscaledDeltaTime;
            }
            else if (curBloom.bloom.radius > orig.bloom.settings.bloom.radius + thresh)
            {
                curBloom.bloom.radius -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curBloom.bloom.radius = orig.bloom.settings.bloom.radius;
            }

            //Sets anti-flicker to match target
            curBloom.bloom.antiFlicker = orig.bloom.settings.bloom.antiFlicker;
        }

        //If Vignette is enabled...
        if (tar.vignette.enabled)
        {
            //Changes color and location to match instantly. Assumes classic Vignette
            curVig.color = orig.vignette.settings.color;
            curVig.center = orig.vignette.settings.center;
            curVig.rounded = orig.vignette.settings.rounded;
            curVig.roundness = orig.vignette.settings.roundness;

            //Increments intensity...
            //--------------------------------------------------------------------------------------
            if (curVig.intensity < orig.vignette.settings.intensity - thresh)
            {
                curVig.intensity += speed * 2 * Time.unscaledDeltaTime;
            }
            else if (curVig.intensity > orig.vignette.settings.intensity + thresh)
            {
                curVig.intensity -= speed * 2 *  Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curVig.intensity = orig.vignette.settings.intensity;
            }

            //Increments smoothness...
            //--------------------------------------------------------------------------------------
            if (curVig.smoothness < orig.vignette.settings.smoothness - thresh)
            {
                curVig.smoothness += speed * Time.unscaledDeltaTime;
            }
            else if (curVig.smoothness > orig.vignette.settings.smoothness + thresh)
            {
                curVig.smoothness -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curVig.smoothness = orig.vignette.settings.smoothness;
            }
        }

        //If Chromatic Abberation is enabled...
        if (tar.chromaticAberration.enabled)
        {
            //Changes the base texture to match
            if(orig.chromaticAberration.settings.spectralTexture != null)
                curChrome.spectralTexture = orig.chromaticAberration.settings.spectralTexture;

            //Increments intensity
            //--------------------------------------------------------------------------------------
            if (curChrome.intensity < orig.chromaticAberration.settings.intensity - thresh)
            {
                curChrome.intensity += speed * Time.unscaledDeltaTime;
            }
            else if (curChrome.intensity > orig.chromaticAberration.settings.intensity + thresh)
            {
                curChrome.intensity -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curChrome.intensity = orig.chromaticAberration.settings.intensity;
            }
        }

        //If Grain is enabled...
        if (tar.grain.enabled)
        {
            //Sets grain color to match
            curGrain.colored = orig.grain.settings.colored;

            //Increments Grain Size...
            //--------------------------------------------------------------------------------------
            if (curGrain.size < orig.grain.settings.size - thresh)
            {
                curGrain.size += speed * Time.unscaledDeltaTime;
            }
            else if (curGrain.size > orig.grain.settings.size + thresh)
            {
                curGrain.size -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curGrain.size = orig.grain.settings.size;
            }

            //Increments intensity...
            //--------------------------------------------------------------------------------------
            if (curGrain.intensity < orig.grain.settings.intensity - thresh)
            {
                curGrain.intensity += speed * Time.unscaledDeltaTime;
            }
            else if (curGrain.intensity > orig.grain.settings.intensity + thresh)
            {
                curGrain.intensity -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curGrain.intensity = orig.grain.settings.intensity;
            }

        }

        //If Color Grading is enabled...
        if (tar.colorGrading.enabled)
        {
            //Sets the color mapping method to the same one the new profile uses...
            curColor.tonemapping = orig.colorGrading.settings.tonemapping;

            //Increments Post Exposure...
            //--------------------------------------------------------------------------------------
            if (curColor.basic.postExposure < orig.colorGrading.settings.basic.postExposure - thresh)
            {
                curColor.basic.postExposure += speed * Time.unscaledDeltaTime;
            }
            else if (curColor.basic.postExposure > orig.colorGrading.settings.basic.postExposure + thresh)
            {
                curColor.basic.postExposure -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curColor.basic.postExposure = orig.colorGrading.settings.basic.postExposure;
            }


            //Increments temperature...
            //--------------------------------------------------------------------------------------
            if (curColor.basic.temperature < orig.colorGrading.settings.basic.temperature - thresh)
            {
                curColor.basic.temperature += speed * Time.unscaledDeltaTime;
            }
            else if (curColor.basic.temperature > orig.colorGrading.settings.basic.temperature + thresh)
            {
                curColor.basic.temperature -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curColor.basic.temperature = orig.colorGrading.settings.basic.temperature;
            }

            //Increments tint...
            //--------------------------------------------------------------------------------------
            if (curColor.basic.tint < orig.colorGrading.settings.basic.tint - thresh)
            {
                curColor.basic.tint += speed * Time.unscaledDeltaTime;
            }
            else if (curColor.basic.tint > orig.colorGrading.settings.basic.tint + thresh)
            {
                curColor.basic.tint -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curColor.basic.tint = orig.colorGrading.settings.basic.tint;
            }

            //Increments saturation...
            //--------------------------------------------------------------------------------------
            if (curColor.basic.saturation < orig.colorGrading.settings.basic.saturation - thresh)
            {
                curColor.basic.saturation += speed * Time.unscaledDeltaTime;
            }
            else if (curColor.basic.saturation > orig.colorGrading.settings.basic.saturation + thresh)
            {
                curColor.basic.saturation -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curColor.basic.saturation = orig.colorGrading.settings.basic.saturation;
            }

            //Increments contrast...
            //--------------------------------------------------------------------------------------
            if (curColor.basic.contrast < orig.colorGrading.settings.basic.contrast - thresh)
            {
                curColor.basic.contrast += speed * Time.unscaledDeltaTime;
            }
            else if (curColor.basic.contrast > orig.colorGrading.settings.basic.contrast + thresh)
            {
                curColor.basic.contrast -= speed * Time.unscaledDeltaTime;
            }
            //If the value is between thesholds, sets equal to target value
            else
            {
                curColor.basic.contrast = orig.colorGrading.settings.basic.contrast;
            }
        }

        //If motion blur is enabled...
        if (tar.motionBlur.enabled)
        {
            //Increments Shutter Angle...
            //--------------------------------------------------------------------------------------
            if (curMotion.shutterAngle < orig.motionBlur.settings.shutterAngle - (thresh * 2))
            {
                curMotion.shutterAngle += speed * 3 * Time.unscaledDeltaTime;
            }
            else if (curMotion.shutterAngle > orig.motionBlur.settings.shutterAngle + (thresh * 2))
            {
                curMotion.shutterAngle -= speed * 3 * Time.unscaledDeltaTime;
            }
            else
            {
                curMotion.shutterAngle = orig.motionBlur.settings.shutterAngle;
            }

            //Increments Frame Blending...
            //--------------------------------------------------------------------------------------
            if (curMotion.frameBlending < orig.motionBlur.settings.frameBlending - (thresh * 2))
            {
                curMotion.frameBlending += speed * 2 * Time.unscaledDeltaTime;
            }
            else if (curMotion.frameBlending > orig.motionBlur.settings.frameBlending + (thresh * 2))
            {
                curMotion.frameBlending -= speed * 2 * Time.unscaledDeltaTime;
            }
            else
            {
                curMotion.frameBlending = orig.motionBlur.settings.frameBlending;
            }
        }

        //Settings are then transferred to the target profile...
        tar.bloom.settings = curBloom;
        tar.vignette.settings = curVig;
        tar.chromaticAberration.settings = curChrome;
        tar.grain.settings = curGrain;
        tar.colorGrading.settings = curColor;
        tar.motionBlur.settings = curMotion;
    }
    
    public void incrementPPSliders(ref PostProcessingProfile tar, ref PostProcessingProfile orig)
    {
        incrementPPSliders(ref tar, ref orig, 1f, 0.1f);
    }

    //Compares too PP, to see if relevant sections are equal - used for gradual changing of active sections
    public bool comparePP(ref PostProcessingProfile tar, ref PostProcessingProfile orig)
    {
        //equal defaults to true
        bool equal = true;

        //First checks for bloom...
        if (tar.bloom.enabled && !orig.bloom.enabled)
        {
            //Bloom intensity...
            if(tar.bloom.settings.bloom.intensity != orig.bloom.settings.bloom.intensity)
            {
                return false;
            }
            if (tar.bloom.settings.bloom.threshold != orig.bloom.settings.bloom.threshold)
            {
                return false;
            }
            if (tar.bloom.settings.bloom.softKnee  != orig.bloom.settings.bloom.softKnee )
            {
                return false;
            }
            if (tar.bloom.settings.bloom.radius != orig.bloom.settings.bloom.radius)
            {
                return false;
            }
        }

        //Then checks for Vignette...
        if(tar.vignette.enabled && !orig.vignette.enabled)
        {
            if(tar.vignette.settings.intensity != orig.vignette.settings.intensity)
            {
                return false;
            }
        }

        //Then checks for Chromatic Abberation...
        if(tar.chromaticAberration.enabled && !orig.chromaticAberration.enabled)
        {
            if(tar.chromaticAberration.settings.intensity != orig.chromaticAberration.settings.intensity)
            {
                return false;
            }
        }

        //Then checks for grain...
        if(tar.grain.enabled && !orig.grain.enabled)
        {
            if(tar.grain.settings.size != orig.grain.settings.size)
            {
                return false;
            }
            if (tar.grain.settings.intensity  != orig.grain.settings.intensity )
            {
                return false;
            }
        }

        //Finally, checks for color grading
        if(tar.colorGrading.enabled && !orig.colorGrading.enabled)
        {
            if(tar.colorGrading.settings.basic.postExposure != orig.colorGrading.settings.basic.postExposure)
            {
                return false;
            }
            if (tar.colorGrading.settings.basic.temperature  != orig.colorGrading.settings.basic.temperature )
            {
                return false;
            }
            if (tar.colorGrading.settings.basic.saturation  != orig.colorGrading.settings.basic.saturation )
            {
                return false;
            }
            if (tar.colorGrading.settings.basic.contrast != orig.colorGrading.settings.basic.contrast)
            {
                return false;
            }
            if (tar.colorGrading.settings.basic.tint != orig.colorGrading.settings.basic.tint)
            {
                return false;
            }
        }

        //Returns equal
        return equal;
    }

    public void changeCutIn(int pNum)
    {
        //Changes to first player's portrait
        if(pNum == 1)
        {
            characterIcon.sprite = characterPortraits[p1.charNum];
        }
        //Changes to second player's portrait
        else if (pNum == 2)
        {
            characterIcon.sprite = characterPortraits[p2.charNum];
        }
    }
}
