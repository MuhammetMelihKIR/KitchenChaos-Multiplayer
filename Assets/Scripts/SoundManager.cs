using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance { get; private set; }
    [SerializeField] private AudioClipRefSO audioClipRefSO;

    private float volume =1f;

    private void Awake()
    {
        Instance = this;
        volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, 1f);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnOnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnOnAnyObjectTrashed;
    }

    private void TrashCounter_OnOnAnyObjectTrashed(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefSO.trash,trashCounter.transform.position);
    }

    private void BaseCounter_OnOnAnyObjectPlacedHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;;
        PlaySound(audioClipRefSO.objectDrop,baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, EventArgs e)
    {
        PlaySound(audioClipRefSO.objectPickup,Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefSO.chop,cuttingCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
       PlaySound(audioClipRefSO.deliveryFail,deliveryCounter.transform.position);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefSO.deliverySuccess,deliveryCounter.transform.position);
    }
    private void PlaySound(AudioClip[] audioClipArray,Vector3 position,float volumeMulltiplier =1f) 
    {
        PlaySound(audioClipArray[Random.Range(0,audioClipArray.Length)],position,volumeMulltiplier);
    }

    private void PlaySound(AudioClip audioClip,Vector3 position,float volumeMulltiplier =1f) 
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMulltiplier * volume); 
    }

    public void PlayFootstepSound(Vector3 position,float volume )
    {
        PlaySound(audioClipRefSO.footstep,position,volume);
    }
    
    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefSO.warning,Vector3.zero);
    }
    public void PlayWarningSound( Vector3 position)
    {
        PlaySound(audioClipRefSO.warning,Vector3.zero);
    }

    public void ChangeVolume()
    {
        volume += 0.1f;
        if (volume > 1f)
        {
            volume = 0f;
        }
        
        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME,volume);
        PlayerPrefs.Save();
    }

    public float GetVolume()
    {
        return volume;
    }
}
