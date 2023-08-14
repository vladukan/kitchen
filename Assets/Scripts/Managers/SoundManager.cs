using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioClipRefsSO _audioClipRefsSO;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += OnRecipeFailed;
        CuttingCounter.OnAnyCut += OnAnyCut;
        BaseCounter.OnAnyDrop += OnAnyDrop;
        TrashCounter.OnAnyTrashed += OnAnyTrashed;
        Player.OnAnyPicked += OnPicked;
    }
    private void OnPicked(object sender, System.EventArgs e)
    {
        Player player = sender as Player;
        PlaySound(_audioClipRefsSO.ObjectPickUp, player.transform.position);
    }
    private void OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(_audioClipRefsSO.Chop, cuttingCounter.transform.position);
    }
    private void OnAnyDrop(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(_audioClipRefsSO.ObjectDrop, baseCounter.transform.position);
    }
    private void OnAnyTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(_audioClipRefsSO.Trash, trashCounter.transform.position);
    }
    private void OnRecipeSuccess(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipRefsSO.DeliverySuccess, DeliveryCounter.Instance.transform.position);
    }
    private void OnRecipeFailed(object sender, System.EventArgs e)
    {
        PlaySound(_audioClipRefsSO.DeliveryFail, DeliveryCounter.Instance.transform.position);
    }
    private void PlaySound(AudioClip[] array, Vector3 position, float volume = 1f)
    {
        PlaySound(array[Random.Range(0, array.Length)], position, volume);
    }
    private void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(clip, position, volume);
    }
    public void PlayCountDownSound()
    {
        PlaySound(_audioClipRefsSO.Warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(_audioClipRefsSO.Warning, position);
    }
    public void PlayFootSteps(Vector3 position, float volume = 1f)
    {
        PlaySound(_audioClipRefsSO.FootStep, position, volume);
    }
}
