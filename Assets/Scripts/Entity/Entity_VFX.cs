
using System.Collections;
using UnityEngine;

public class Entity_VFX : MonoBehaviour
{
    
    private SpriteRenderer sr;
    [Header("On Damage VFX")]
    [SerializeField] private Material OnDamageMaterial;
    [SerializeField] private float onDamagefxDuration = .2f;
    private Material originalMaterial;
    private Coroutine onDamageVfxcoroutine;

    private void Awake()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = sr.material;
    }

    public void PlayOnDamageVfx()
    {
        if (onDamageVfxcoroutine != null)
        {
            StopCoroutine(onDamageVfxcoroutine);
        }

        onDamageVfxcoroutine = StartCoroutine(OnDamageVfxCo());
    }
    
    private IEnumerator OnDamageVfxCo()
    {
        sr.material = OnDamageMaterial;
        
        yield return new WaitForSeconds(onDamagefxDuration);
        sr.material = originalMaterial;
    }
}
