using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Mobile
{
    public class ConnectButton : MonoBehaviour, IPointerClickHandler, ISubmitHandler
    {
        public event Action OnPress;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPress?.Invoke();
        }

        public void OnSubmit(BaseEventData eventData)
        {
            OnPress?.Invoke();
        }
    }
}
