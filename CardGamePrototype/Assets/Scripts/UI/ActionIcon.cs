using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionIcon : MonoBehaviour
{
    public Sprite ActiveIcon, InActiveIcon;
    public Image IconImage;
    public ParticleSystem PushOutParticles;
    public ParticleSystem HighlightParticles;
    private bool active;

    public bool Active
    {
        get => active; 
        set
        {
            active = value;
            if (active)
                OnReactivate();
            else
                OnDisactivate();
        }
    }

    private void OnDisactivate()
    {
        HighlightParticles.Stop();
        IconImage.sprite = InActiveIcon;
        PushOutParticles.Stop();
        PushOutParticles.Play();
    }

    private void OnReactivate()
    {
        HighlightParticles.Play();
        IconImage.sprite = ActiveIcon;
        PushOutParticles.Stop();
        PushOutParticles.Play();

    }

}
