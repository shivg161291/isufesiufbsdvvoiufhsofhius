using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class Card : MonoBehaviour, IPointerClickHandler
{
    public int Id; // unique id for matching (pair id)
    public Sprite FaceSprite;
    public Sprite BackSprite;
    public float FlipDuration = 0.25f;

    public bool IsMatched { get; private set; }
    public bool IsFaceUp { get; private set; }

    Image image;
    RectTransform rt;
    GameManager manager;

    void Awake()
    {
        image = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    public void Initialize(int id, Sprite face, Sprite back, GameManager gm)
    {
        Id = id;
        FaceSprite = face;
        BackSprite = back;
        manager = gm;
        IsMatched = false;
        IsFaceUp = false;
        image.sprite = BackSprite;
        transform.localRotation = Quaternion.identity;
        gameObject.SetActive(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsMatched) return;
        if (IsFaceUp) return;
        // request flip
        manager.RequestFlip(this);
    }

    public void MarkMatched()
    {
        IsMatched = true;
    }

    public void ForceSetFace(bool faceUp)
    {
        StopAllCoroutines();
        IsFaceUp = faceUp;
        image.sprite = faceUp ? FaceSprite : BackSprite;
        transform.localRotation = Quaternion.identity;
    }

    public void PlayFlipAnimation(System.Action onComplete = null)
    {
        StartCoroutine(FlipCoroutine(onComplete));
    }

    IEnumerator FlipCoroutine(System.Action onComplete)
    {
        manager?.Audio.PlayFlip();
        float half = FlipDuration / 2f;
        float t = 0f;
        // first half rotate to 90
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            float angle = Mathf.Lerp(0, 90, p);
            transform.localEulerAngles = new Vector3(0, angle, 0);
            yield return null;
        }
        // swap sprite
        IsFaceUp = !IsFaceUp;
        image.sprite = IsFaceUp ? FaceSprite : BackSprite;

        // second half rotate 90->0 (but mirrored to 0)
        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / half);
            float angle = Mathf.Lerp(90, 0, p);
            transform.localEulerAngles = new Vector3(0, angle, 0);
            yield return null;
        }
        transform.localEulerAngles = Vector3.zero;
        onComplete?.Invoke();
    }
}
