using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Slimes
{
    public class SlimeCreationController : MonoBehaviour
    {
        [SerializeField]
        private GameObject slimePrefab;

        private GameObject currentSlime;

        // Start is called before the first frame update
        void Start()
        {
            InstantiateSlime();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                InstantiateSlime();

            }
        }

        private void InstantiateSlime()
        {
            if (currentSlime != null) { Destroy(currentSlime); }

            currentSlime = Instantiate(slimePrefab);
            currentSlime.GetComponent<SlimeCreator>().CreateSlime();
        }
    }
}