using UnityEngine;

namespace Menu
{
    public class uiBlocker : MonoBehaviour
    {

        private static uiBlocker instance;

        void Start()
        {
            instance = this;
            Hide_Static();
        }

        void Update()
        {

        }

        public static void Show_Static()
        {
            instance.gameObject.SetActive(true);
            instance.transform.SetAsLastSibling();
        }

        public static void Hide_Static()
        {
            instance.gameObject.SetActive(false);
        }

    }
}