using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterRotator : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private GameObject _character;
    private Vector3 _mousePos;

    private bool _isDown;

    public void OnPointerDown(PointerEventData eventData)
    {
        _isDown = true;
    }

    private void Update()
    {
        if (_isDown)
        {
            _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //_character.transform.rotation = new Quaternion(0, Input.mousePosition.y, 0, 0);
            _character.transform.position = new Vector3(-0.09f, _mousePos.y, 2.9f);
        }
    }

}
