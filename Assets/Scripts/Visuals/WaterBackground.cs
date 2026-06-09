using UnityEngine;

public class WaterBackground : MonoBehaviour
{
    [SerializeField] private float _animationSpeed = 8f;
    [SerializeField] private float _scrollSpeed = 0.5f;
    [SerializeField] private int _frameCount = 50;

    private Material _mat;
    private float _scrollOffset;

    private void Awake()
    {
        _mat = GetComponent<Renderer>().material;
    }

    private void Update()
    {
        int frame = Mathf.FloorToInt(Time.time * _animationSpeed) % _frameCount;
        _scrollOffset += _scrollSpeed * Time.deltaTime;
        _mat.mainTextureOffset = new Vector2((float)frame / _frameCount, _scrollOffset);
    }
}
