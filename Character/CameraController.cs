using UnityEngine;

namespace Game.Character
{
    public class CameraController : MonoBehaviour
    {
        public Transform focus;
        public float scrollScale = 1.1f;
        public float minScale = 5;
        public float maxScale = 20;
        public float smoothTime = 2;

        private Vector3 _offset;

        private void Awake()
        {
            _offset = focus.position - transform.position;
        }

        private void Update()
        {
            _offset.z = Mathf.Min(Mathf.Max(_offset.z + -Input.mouseScrollDelta.y * scrollScale, minScale), maxScale);
            transform.position = Vector3.Lerp(transform.position, focus.position - _offset, Time.deltaTime * smoothTime);
        }
    }
}